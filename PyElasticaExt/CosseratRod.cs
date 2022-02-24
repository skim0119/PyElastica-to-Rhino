using Grasshopper;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Numpy;

namespace PyElasticaExt
{
    public class CosseratRod : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CosseratRod()
          : base(name: "CosseratRods",
                 nickname: "CosseratRods",
                 description: "Create Cosserat Rods",
                 category: "PyElastica",
                 subCategory: "Primitive")
        {
        }
        (NDarray position, NDarray radius) data = (np.empty(0), np.empty(0));

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
            pManager.AddBooleanParameter("Reload", "Re", "Reload switch", GH_ParamAccess.item, false);
            pManager.AddGenericParameter("CosseratRod", "CR", "Cosserat Rod data: Position and Radius", GH_ParamAccess.item);
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
            pManager.AddBrepParameter("Rod", "R", "Brep object of Cosserat Rod", GH_ParamAccess.item);
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
            bool C = false; // global safe switch
            bool reload = false; // force reload data
            string debug_string = "";
            int timestep = 0;

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetData(1, ref reload)) return;
            if (reload)
                if (!DA.GetData(2, ref data)) return;
            if (!DA.GetData(3, ref timestep)) return;

            if(!C) return; // global safe switch

            // We should now validate the data and warn the user if invalid data is supplied.

            // Geometry
            // (data.position) has shape (timestep, 3, n_nodes)
            // (data.radius) has shape (timestep, n_nodes)
            List<Point3d> node_points = new List<Point3d>();
            List<double> node_radii = new List<double>();
            ParseData(data.position[timestep.ToString() + ",:,:"],
                      data.radius  [timestep.ToString() + ",:"  ],
                      ref node_points,
                      ref node_radii);

            Curve interp_curve = CreateInterpolation(node_points);
            var pipe = CreateRod(interp_curve, node_points, node_radii);

            // Finally assign the spiral to the output parameter.
            debug_string += "Done\n";

            
            DA.SetData(0, pipe[0]);
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
                point_nodes.Add(new Point3d(coord[0], coord[1], coord[2]));
            }
            return;
        }

        private Curve CreateInterpolation(List<Point3d> node_points, int degree=3)
        {
            Curve interp_curve = Curve.CreateInterpolatedCurve(node_points, degree);
            return interp_curve;
        }

        private List<Brep> CreateRod(Curve curve, List<Point3d> points, List<double> radii)
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

            List<Brep> pipe = new List<Brep>(Brep.CreatePipe(
                rail: curve,
                railRadiiParameters: ts,
                radii: radii,
                localBlending: true,
                cap: PipeCapMode.Round,
                fitRail: false,
                absoluteTolerance: MTOL,
                angleToleranceRadians: ATOL
                ));
            return pipe;
        }

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => PyElasticaExt.Properties.Resources.icons8_snake_24.ToBitmap();

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("64D90F35-E231-4D53-9144-B58EE294A478");
    }
}