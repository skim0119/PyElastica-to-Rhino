using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Numpy;

namespace PyElasticaExt
{
    public class NumpyImportLegacy : GH_Component
    {
        // TODO: DEBUG remove file path
        string testpath = "E:\\Rendering_Octopus_paper\\pickles\\curl\\octopus_arm_test.npz";
        /// <summary>
        /// Initializes a new instance of the NumpyImport(Legacy) class.
        /// </summary>
        public NumpyImportLegacy()
          : base(name: "NpzImport(Legacy)",
                 nickname: "NpzImport(L)",
                 description: "Import npz file exported from numpy. (Previous version)",
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
            // TODO : DEBUG remove default filepath
            pManager.AddTextParameter("FilePath", "Pa", "Path that contains PyElastica exports", GH_ParamAccess.item, "");
            pManager.AddTextParameter("Group", "Gr", "Rod Group", GH_ParamAccess.item, "helical_rods");
            pManager.AddBooleanParameter("Periodic", "Pr", "Periodic rod (default:false)", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Completed", "C", "Module finished", GH_ParamAccess.item);
            pManager.AddGenericParameter("CosseratRod", "CR", "Cosserat Rod data: Position and Radius", GH_ParamAccess.list);
            pManager.AddTextParameter("Debug", "D", "Debug Output", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool C = false; // global safe switch
            string filepath = "";
            string debug_string = "";
            string group = "helical_rods";
            bool isPeriodic = false;

            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetData(1, ref filepath)) return;
            if (!DA.GetData(2, ref group)) return;
            if (!DA.GetData(3, ref isPeriodic)) return;

            if (!C) return; // global safe switch

            var data = DataLoader(testpath, group, isPeriodic, ref debug_string);

            debug_string += "Done\n";

            DA.SetData(0, true); // indicate the completion of the module
            DA.SetDataList(1, data);
            DA.SetData(2, debug_string);

            if (data is null) { throw new AccessViolationException("failed to load data"); }
        }

        private List<(NDarray, NDarray)> DataLoader(string path, string group, bool isPeriodic, ref string debug_string)
        {
            List<(NDarray position, NDarray radius)> return_data = new List<(NDarray, NDarray)>();
            NDarray position_arr, radius_arr;
            try {
                var npz_file = Numpy.np.load(path, allow_pickle:true);
                position_arr = new NDarray(npz_file.self[group + "_position_history"]);
                radius_arr = new NDarray(npz_file.self[group + "_radius_history"]);
            }
            catch (Python.Runtime.PythonException ex)
            {
                debug_string += "Numpy file reading error. Either file does not exist or key does not exist in the file.\n";
                return null;
            }

            // If not periodic, n_node has one more element
            if (!isPeriodic)
            {
                radius_arr = np.append(
                    radius_arr,
                    np.zeros(radius_arr.shape[0], radius_arr.shape[1], 1),
                    axis: -1);
                radius_arr[":,:,1:"] += radius_arr[":,:,:-1"];
                radius_arr[":,:,1:-1"] /= 2.0;
            }

            // Make list
            int num_rod_in_group = position_arr.shape[0];
            for (int i = 0; i < num_rod_in_group; ++i)
            {
                return_data.Add((position:position_arr[i], radius:radius_arr[i]));
            }

            debug_string += "radii shape(" + radius_arr.shape.ToString() + ")\n" +
                            "position shape(" + position_arr.shape.ToString() + ")\n";

            return return_data;
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