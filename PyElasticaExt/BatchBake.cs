using System;
using System.Diagnostics;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace PyElasticaExt
{
    public class BatchBake : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BatchBake class.
        /// </summary>
        public BatchBake()
          : base("BatchBake", "BatchBake",
              "Description",
              "PyElastica", "Rendering")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Switch", "C", "Module switch", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("LayerID", "Ly", "Layer id to bake the objects", GH_ParamAccess.item);
            pManager.AddBrepParameter("Object List", "Obj", "List of objects to bake.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Debug", "D", "Debug Output", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Complete", "C", "Completion indicator", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Stopwatch stopwatch = new Stopwatch();

            bool C = false; // global safe switch
            int layer_id = -1;
            string debug_string = "";
            List<Brep> breps = new List<Brep>();

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetData(1, ref layer_id)) return;
            if (!DA.GetDataList(2, breps)) return;

            // Check data structure and validity
            if(!C) return; // global safe switch
            DA.SetData(1, false);
            stopwatch.Start();

            debug_string += "data received: " + breps.Count + "\n";

            Rhino.DocObjects.ObjectAttributes obj_attribute = new Rhino.DocObjects.ObjectAttributes();
            obj_attribute.LayerIndex = layer_id;
            foreach(Brep br in breps)
            {
                Rhino.RhinoDoc.ActiveDoc.Objects.AddBrep(br, obj_attribute);
            }

            stopwatch.Stop();
            debug_string += "Elapsed Time: " + (stopwatch.ElapsedMilliseconds/1000.0).ToString() +  "\n";
            debug_string += "Done\n";


            DA.SetData(0, debug_string);
            DA.SetData(1, true);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.icons8_microwave_24.ToBitmap();

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2EAA041F-BAFD-454C-8116-BD909E73B49D"); }
        }
    }
}