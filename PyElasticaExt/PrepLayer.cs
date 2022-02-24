using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace PyElasticaExt
{
    public class PrepLayer : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PrepLayer class.
        /// </summary>
        public PrepLayer()
          : base("PrepLayer", "PL",
              "Prepare Layer",
              "PyElastica", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Switch", "C", "Module switch", GH_ParamAccess.item, false);
            pManager.AddTextParameter("LayerName", "LN", "Layer name to bake the objects", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("LayerID", "Ly", "Layer ID", GH_ParamAccess.item);
            pManager.AddTextParameter("Debug", "D", "Debug Output", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Complete", "C", "Completion indicator", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool C = false;
            string layer_name = "";
            string debug_string = "";
            
            if (!DA.GetData("Switch", ref C)) return;
            if (!DA.GetData("LayerName", ref layer_name)) return;

            RhinoBackscript.CreateSubLayer(
                parent_name:"_simulation",
                child_name:layer_name);

            int layer_id = RhinoDoc.ActiveDoc.Layers.FindByFullPath("_simulation::" + layer_name, -1);
            Rhino.DocObjects.Layer layer = RhinoDoc.ActiveDoc.Layers[layer_id];
            debug_string += layer_name + " created (or may already exist) - " + layer_id.ToString() + "\n";

            RhinoBackscript.CleanLayer(layer);
            debug_string += layer_name + " cleared\n";

            DA.SetData(0, layer_id);
            DA.SetData(1, debug_string);
            DA.SetData(2, true);
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
            get { return new Guid("7642E802-7286-4B0E-A0D0-344F03376639"); }
        }
    }
}