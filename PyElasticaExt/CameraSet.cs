using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PyElasticaExt
{
    public class CameraSet : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CameraSet class.
        /// </summary>
        public CameraSet()
          : base("CameraSet", "CameraSet",
              "Set Camera location, and target.",
              "PyElastica", "CameraControl")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //pManager.AddGenericParameter("Viewport_id", "ViewID", "Viewport id", GH_ParamAccess.item);
            pManager.AddPointParameter("Location", "Loc", "Camera location", GH_ParamAccess.item);
            pManager.AddPointParameter("Target", "Tar", "Camera target", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Location", "Loc", "Camera location", GH_ParamAccess.item);
            pManager.AddPointParameter("Target", "Tar", "Camera target", GH_ParamAccess.item);
            pManager.AddBooleanParameter("C", "Completed", "Return true when all operations are done.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int vp_id = new int();
            Point3d loc = new Point3d();
            Point3d tar = new Point3d();

            DA.SetData("C", false);
            //if (!DA.GetData("Viewport_id", ref vp_id)) return;
            if (!DA.GetData("Location", ref loc)) return;
            if (!DA.GetData("Target", ref tar)) return;

            var vp = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;
            vp.SetCameraLocation(loc, true);
            vp.SetCameraDirection(tar - loc, true);

            /*
            var vp = Rhino.RhinoDoc.ActiveDoc.NamedViews[vp_id].Viewport;
            Rhino.Display.RhinoView view = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView;

            vp.IsCameraDirectionLocked = false;
            vp.IsCameraLocationLocked = false;
            vp.SetCameraLocation(loc);
            vp.SetCameraDirection(tar - loc);

            Rhino.RhinoDoc.ActiveDoc.NamedViews.Restore();
            Rhino.RhinoDoc.ActiveDoc.NamedViews.Restore(vp_id, view.ActiveViewport);
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.Redraw();
            */

            DA.SetData("Location", vp.CameraLocation);
            DA.SetData("Target", vp.CameraTarget);
            DA.SetData("C", true);
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
                return PyElasticaExt.Properties.Resources.icons8_camera_50.ToBitmap();
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4D4882E4-C351-4B52-87D2-DF41D69F39BC"); }
        }
    }
}