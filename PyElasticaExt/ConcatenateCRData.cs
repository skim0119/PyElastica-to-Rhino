using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Numpy;

namespace PyElasticaExt
{
    public class ConcatenateCRData : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the ConcatenateData class.
        /// </summary>
        public ConcatenateCRData()
          : base(name: "ConcatenateCR", nickname:"ConcatCR",
              description:"Concatenate Cosserat Rod data in time.",
                 category: "PyElastica",
                 subCategory: "Import")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        /// <summary>
        /// Registers the initial inputs.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data 0", "Data 0", "A Data Input", GH_ParamAccess.list);
            pManager.AddGenericParameter("Data 1", "Data 1", "A Data Input", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Out", "Out", "Concatenated Data", GH_ParamAccess.list);
            pManager.AddTextParameter("D", "Debug", "Debug string", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            /*
            GH_Structure<IGH_Goo> new_list = new GH_Structure<IGH_Goo>();
            int pathNow = 0;
            for(int i = 0; i < Params.Input.Count; i++)
            {
                GH_Structure<IGH_Goo> Data = new GH_Structure<IGH_Goo>();
                DA.GetDataTree(i, out Data);
                if(Data.PathCount == 0) { continue; }
                GH_Path highPath = Data.get_Path(Data.PathCount - 1);
                int nextPath = highPath.Indices[0]+1;
                for(int j =0;j<Data.PathCount; j++)
                {
                    GH_Path Path = new GH_Path(Data.get_Path(j));
                    var items = Data.get_Branch(j);
                    Path[0] = Path[0] + pathNow;
                    List<IGH_Goo> goolist = new List<IGH_Goo>(items as List<IGH_Goo>);
                    newTree.AppendRange(goolist, Path);
                }
                pathNow += nextPath;
            }
            DA.SetDataTree(0, new_list);
            */

            string debug_string = "";

            // Read data input
            int num_params = Params.Input.Count;
            List<List<(NDarray position, NDarray radius)>> inputs = new List<List<(NDarray position, NDarray radius)>>();
            for(int i = 0; i < num_params; i++)
            {
                List<(NDarray position, NDarray radius)> data_list = new List<(NDarray, NDarray)>();
                DA.GetDataList(i, data_list);
                inputs.Add(data_list);
            }

            // Concatenate
            List<(NDarray position, NDarray radius)> total_data_list = new List<(NDarray, NDarray)>();
            int num_rod_in_group = inputs[0].Count;
            for (int i = 0; i < num_rod_in_group; ++i)
            {
                List<NDarray> position_list = new List<NDarray>();
                List<NDarray> radius_list = new List<NDarray>();
                for (int j = 0; j < num_params; j++)
                {
                    position_list.Add(inputs[j][i].position);
                    radius_list.Add(inputs[j][i].radius);

                }
                NDarray position_collection = np.concatenate(position_list.ToArray(), axis: 0);
                NDarray radius_collection = np.concatenate(radius_list.ToArray(), axis: 0);
                total_data_list.Add((position:position_collection, radius:radius_collection));

                // Debug
                debug_string += "rod type: " + i.ToString() + "\n";
                debug_string += "radii shape(" + radius_collection.shape.ToString() + ")\n" +
                                "position shape(" + position_collection.shape.ToString() + ")\n";
            }

            debug_string += "Done\n";
            DA.SetDataList(0, total_data_list);
            DA.SetData(1, debug_string);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if(side == GH_ParameterSide.Input)
            {
                return true;
            }
            return false;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if(side==GH_ParameterSide.Input && Params.Input.Count > 2)
            {
                return true;
            }
            return false;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            Param_GenericObject param = new Param_GenericObject();

            param.Name = $"Data {Params.Input.Count}";
            param.NickName = $"Data {Params.Input.Count}";
            param.Description = "A Data input";
            param.Optional = true;
            param.Access = GH_ParamAccess.list;
            return param;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            for (int i = 0; i < Params.Input.Count; i++)
            {
                Params.Input[i].MutableNickName = false;
                Params.Input[i].Name = $"Data {i}";
                Params.Input[i].NickName = $"Data {i}";
            }
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B66B62BC-55B1-46EE-A9F5-E76010789287"); }
        }
    }
}