using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Numpy;

namespace PyElasticaExt
{
    public class NumpyImport : GH_Component
    {
        string testpath = "E:\\Rendering_Octopus_paper\\pickles\\curl\\octopus_arm_test.npz";
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public NumpyImport()
          : base(name: "NpzImport",
                 nickname: "NpzImport",
                 description: "Import npz file exported from numpy",
                 category: "PyElastica",
                 subCategory: "Import")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Switch", "C", "Module switch", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("timestep", "T", "Time", GH_ParamAccess.item);
            pManager.AddTextParameter("FilePath", "Pa", "Path that contains PyElastica exports", GH_ParamAccess.item, "");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Radii", "R", "Radii output", GH_ParamAccess.item);
            pManager.AddTextParameter("Debug", "D", "Debug Output", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Succeed", "S", "Module finished", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool C = false; // global safe switch
            int timestep = 0; 
            string filepath = "";
            string key = "helical_rods";

            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetData(1, ref timestep)) return;
            if (!DA.GetData(2, ref filepath)) return;

            if (!C) return; // global safe switch

            var data = Numpy.np.load(testpath, allow_pickle:true);
            NDarray position_arr = new NDarray(data.self[key+"_position_history"]);
            NDarray radius_arr = new NDarray(data.self[key+"_radius_history"]);

            Console.WriteLine(radius_arr.ToString());

            // DA.SetData(0, radius_arr[timestep]);
            DA.SetData(1, "Done");
            DA.SetData(2, true);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => PyElasticaExt.Properties.Resources.icons8_save_24.ToBitmap();

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("792A3913-9EEB-42D4-AA76-B8C204D29262"); }
        }
    }
}