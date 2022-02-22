using System;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;
using Numpy;


namespace PyElasticaExt
{
    public class RigidSphere : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RigidSphere class.
        /// </summary>
        public RigidSphere()
          : base("RigidSphere", "Sphere",
              "Create Sphere from PyElastica data",
              "PyElastica", "Primitive")
        {
        }
        (NDarray position, NDarray radius) data = (np.empty(0), np.empty(0));

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Switch", "C", "Module switch", GH_ParamAccess.item, false);
            pManager.AddGenericParameter("RigidSphere", "RS", "Rigid Sphere data: Position and Radius", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Timestep", "T", "Timestep", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Sphere", "Sp", "Brep object of Rigid Sphere", GH_ParamAccess.item);
            pManager.AddTextParameter("Debug", "D", "Debug Output", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.
            bool C = false; // global safe switch
            string debug_string = "";
            int timestep = 0;

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetData(1, ref data)) return;
            if (!DA.GetData(2, ref timestep)) return;

            if(!C) return; // global safe switch

            // We should now validate the data and warn the user if invalid data is supplied.

            // Geometry
            // (data.position) has shape (timestep, 3, n_nodes)
            // (data.radius) has shape (timestep, n_nodes)
            Point3d center = new Point3d();
            double radius = new double();
            ParseData(data.position[timestep.ToString() + ",:,:"],
                      data.radius  [timestep.ToString() + ",:"  ],
                      ref center,
                      ref radius);

            var sphere = CreateSphere(center, radius);

            // Finally assign the spiral to the output parameter.
            debug_string += "Done\n";

            DA.SetData(0, sphere);
            DA.SetData(1, debug_string);
        }

        private void ParseData(NDarray position, NDarray radius,
            ref Point3d center, ref double rad)
        {
            var coord = position[":,0"].GetData<double>();
            center = new Point3d(coord[0], coord[1], coord[2]);
            rad = radius.GetData<double>()[0];
            return;
        }

        private Sphere CreateSphere(Point3d center, double radius)
        {
            double MTOL = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            double ATOL = RhinoDoc.ActiveDoc.ModelAngleToleranceRadians;

            Sphere sphere = new Sphere(center, radius);
            return sphere;
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
                return PyElasticaExt.Properties.Resources.icons8_sphere_30.ToBitmap();
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("18CAB5E1-46B0-49EB-94E9-2EE68C0661AA"); }
        }
    }
}