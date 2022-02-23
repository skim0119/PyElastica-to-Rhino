using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PyElasticaExt
{
    public class CameraLocationCapture : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CameraLocationCapture class.
        /// </summary>
        public CameraLocationCapture()
          : base("CameraLocationCapture", "CameraCapture",
              "Bake the position and the target of camera",
              "PyElastica", "CameraControl")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Capture", "C", "Capture", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool C = false;
            if (!DA.GetData("Capture", ref C)) return;

            if (!C) return; // global safe switch

            // Create layer
            RhinoBackscript.CreateSubLayer("_camera_control", "target");
            RhinoBackscript.CreateSubLayer("_camera_control", "location");

            // Create points
            var vp = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;
            Point3d loc = vp.CameraLocation;
            Point3d tar = vp.CameraTarget;

            // Bake location point
            {
                int layer_id = Rhino.RhinoDoc.ActiveDoc.Layers.FindByFullPath("_camera_control::location", -1);
                Rhino.DocObjects.ObjectAttributes obj_attribute = new Rhino.DocObjects.ObjectAttributes();
                obj_attribute.LayerIndex = layer_id;
                Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(loc, obj_attribute);
            }
            // Bake target point
            {
                int layer_id = Rhino.RhinoDoc.ActiveDoc.Layers.FindByFullPath("_camera_control::target", -1);
                Rhino.DocObjects.ObjectAttributes obj_attribute = new Rhino.DocObjects.ObjectAttributes();
                obj_attribute.LayerIndex = layer_id;
                Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(tar, obj_attribute);
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
                return PyElasticaExt.Properties.Resources.icons8_add_camera_50.ToBitmap();
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("30E7D0CE-F16D-4254-A145-49CDA53340C2"); }
        }
    }
}