using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PyElasticaExt
{
    public class RenderingRaster : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RenderingRaster class.
        /// </summary>
        public RenderingRaster()
          : base(name:"Rendering(Raster)",
                 nickname:"Re(Ra)",
                 description:"Rasterize Rendering",
                 category:"PyElastica",
                 subCategory:"Rendering")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Switch", "C", "Module switch", GH_ParamAccess.item, false);
            pManager.AddTextParameter("FilePath", "Pa", "Output Path", GH_ParamAccess.item, "");
            pManager.AddIntegerParameter("Timestep", "T", "Timestep", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Completed", "C", "Module finished", GH_ParamAccess.item);
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
            int timestep = 0;
            string debug_string = "";

            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetData(1, ref filepath)) return;
            if (!DA.GetData(2, ref timestep)) return;
            DA.SetData(0, false);

            if(!C) return; // global safe switch

            string savedLocation = filepath + timestep.ToString("D4") + ".png";

            var view = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView;


            var view_capture = new Rhino.Display.ViewCapture
            {
                Width = view.ActiveViewport.Size.Width,
                Height = view.ActiveViewport.Size.Height,
                ScaleScreenItems = false,
                DrawAxes = false,
                DrawGrid = false,
                DrawGridAxes = false,
                TransparentBackground = false 
            };


            var bitmap = view_capture.CaptureToBitmap(view);
            if (null != bitmap)
            {
                bitmap.Save(savedLocation, System.Drawing.Imaging.ImageFormat.Png);
            }

            DA.SetData(0, true);
            DA.SetData(1, debug_string);
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
                return Properties.Resources.icons8_render_24.ToBitmap();
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5CD86984-D542-44F2-BAC4-5BC1E3B8CC15"); }
        }
    }
}