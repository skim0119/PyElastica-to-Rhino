using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Numpy;

namespace PyElasticaExt
{
    public class NumpyImport : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the NumpyImport class.
        /// </summary>
        public NumpyImport()
          : base(name: "NpzImport",
                 nickname: "NpzImport",
                 description: "Import npz file exported from numpy.",
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
            pManager.AddTextParameter("FilePath", "Pa", "Path that contains PyElastica exports", GH_ParamAccess.item);
            pManager.AddTextParameter("Group", "Gr", "Rod Group", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Periodic", "Pr", "Periodic rod (default:false)", GH_ParamAccess.item, false);
        }

        /// <summary>
        // Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("CosseratRod", "CR", "Cosserat Rod data: Position and Radius", GH_ParamAccess.list);
            pManager.AddTextParameter("Debug", "D", "Debug Output", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            throw new AccessViolationException("Not Implemented Yet");
        }
        private List<(NDarray, NDarray)> DataLoader(string path, string group, ref string debug_string)
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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.icons8_save_24.ToBitmap();

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("64DE4EF7-F817-4CDF-86D9-71AF926BE712"); }
        }
    }
}