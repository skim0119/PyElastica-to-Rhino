using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

using Numpy;

namespace PyElasticaExt
{
    public class CosseratRodParallel : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CosseratRodParallel class.
        /// </summary>
        public CosseratRodParallel()
          : base(name: "CosseratRods(Parallel)",
                 nickname: "CosseratRods(Parallel)",
                 description: "Create Cosserat Rods",
                 category: "PyElastica",
                 subCategory: "Primitive")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddBooleanParameter("Switch", "C", "Module switch", GH_ParamAccess.item, false);
            pManager.AddGenericParameter("CosseratRod", "CR", "Cosserat Rod data: Position and Radius", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Timestep", "T", "Timestep", GH_ParamAccess.item, 0);

            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddBrepParameter("Rod", "R", "Brep object of Cosserat Rod", GH_ParamAccess.list);
            pManager.AddTextParameter("Debug", "D", "Debug Output", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.
            Stopwatch stopwatch = new Stopwatch();

            bool C = false; // global safe switch
            string debug_string = "";
            int timestep = 0;
            List<(NDarray position, NDarray radius)> data_list = new List<(NDarray, NDarray)>();
            //List<Brep> brep_list = new List<Brep>();
            System.Collections.Concurrent.ConcurrentBag<Brep> brep_list = new System.Collections.Concurrent.ConcurrentBag<Brep>();

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetDataList(1, data_list)) return;
            if (!DA.GetData(2, ref timestep)) return;

            if(!C) return; // global safe switch
            stopwatch.Start();

            // We should now validate the data and warn the user if invalid data is supplied.

            // Geometry
            // (data.position) has shape (timestep, 3, n_nodes)
            // (data.radius) has shape (timestep, n_nodes)
            /*foreach((NDarray position, NDarray radius) data in data_list)
            {
                List<Point3d> node_points = new List<Point3d>();
                List<double> node_radii = new List<double>();
                ParseData(data.position[timestep.ToString() + ",:,:"],
                          data.radius  [timestep.ToString() + ",:"  ],
                          ref node_points,
                          ref node_radii);
                Curve interp_curve = CreateInterpolation(node_points);
                Brep pipe = CreateRod(interp_curve, node_points, node_radii);
                brep_list.Add(pipe);
            }
            */
            Parallel.For(0, data_list.Count, i =>
            {
                List<Point3d> node_points = new List<Point3d>();
                List<double> node_radii = new List<double>();
                lock (data_list)
                {
                    ParseData(data_list[i].position[timestep.ToString() + ",:,:"],
                              data_list[i].radius[timestep.ToString() + ",:"],
                              ref node_points,
                              ref node_radii);
                }
                Curve interp_curve = CreateInterpolation(node_points);
                Brep pipe = CreateRod(interp_curve, node_points, node_radii);
                brep_list.Add(pipe);
            });

            // Finally assign the spiral to the output parameter.
            stopwatch.Stop();
            debug_string += "Elapsed Time: " + (stopwatch.ElapsedMilliseconds/1000.0).ToString() +  "\n";
            debug_string += "Done\n";

            DA.SetDataList(0, brep_list.ToArray());
            DA.SetData(1, debug_string);
        }

        private void ParseData(NDarray position, NDarray radius,
            ref List<Point3d> point_nodes, ref List<double> radii)
        {
            // Numpy data to points
            radii.Clear();
            radii.AddRange(radius.GetData<double>());
            int num_nodes = position.shape[1];
            for (int i = 0; i < num_nodes; ++i)
            {
                var coord = position[":," + i.ToString()].GetData<double>();
                point_nodes.Add(new Point3d(coord[0], -coord[2], coord[1]));
            }
            return;
        }

        private Curve CreateInterpolation(List<Point3d> node_points, int degree=3)
        {
            Curve interp_curve = Curve.CreateInterpolatedCurve(node_points, degree);
            return interp_curve;
        }

        private Brep CreateRod(Curve curve, List<Point3d> points, List<double> radii)
        {
            double MTOL = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            double ATOL = RhinoDoc.ActiveDoc.ModelAngleToleranceRadians;
            Interval domain = curve.Domain;
            List<double> ts = new List<double>(); // normalized (domain)
            foreach(Point3d pt in points)
            {
                double t = 0.0;
                curve.ClosestPoint(pt, out t); // get t
                ts.Add(domain.NormalizedParameterAt(t));
            }

            Brep[] pipe = Brep.CreatePipe(
                rail: curve,
                railRadiiParameters: ts,
                radii: radii,
                localBlending: true,
                cap: PipeCapMode.Round,
                fitRail: false,
                absoluteTolerance: MTOL,
                angleToleranceRadians: ATOL
                );
            return pipe[0];
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return PyElasticaExt.Properties.Resources.icons8_snake_24.ToBitmap();
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3B10688D-7F82-4288-9611-64929001CC75"); }
        }
    }
}