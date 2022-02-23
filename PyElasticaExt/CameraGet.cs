using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PyElasticaExt
{
    public class CameraGet : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CameraGet class.
        /// </summary>
        public CameraGet()
          : base("CameraGet", "CameraGet",
              "Get Camera object, location, and target.",
              "PyElastica", "CameraControl")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("NamedView", "View", "Name of the view", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Location", "Loc", "Camera location", GH_ParamAccess.item);
            pManager.AddPointParameter("Target", "Tar", "Camera target", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Viewport_id", "ViewID", "Viewport id", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string view_name = "";
            if (!DA.GetData("NamedView", ref view_name)) return;

            int vp_id = Rhino.RhinoDoc.ActiveDoc.NamedViews.FindByName(Name = view_name);
            var vp = Rhino.RhinoDoc.ActiveDoc.NamedViews[vp_id].Viewport;
            DA.SetData("Location", vp.CameraLocation);
            DA.SetData("Target", vp.TargetPoint);
            DA.SetData("Viewport_id", vp_id);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return PyElasticaExt.Properties.Resources.icons8_camera_identification_50.ToBitmap();
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("113311D3-259C-48BA-A1C8-CD9267024AC6"); }
        }
    }
}