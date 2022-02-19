using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace PyElasticaExt
{
    public class Rendering : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Rendering class.
        /// </summary>
        public Rendering()
          : base(name:"Rendering",
                 nickname:"Re",
                 description:"Raytrace Rendering",
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
            pManager.AddBooleanParameter("Succeed", "S", "Module finished", GH_ParamAccess.item);
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
            // string debug_string = "";

            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetData(1, ref filepath)) return;
            if (!DA.GetData(2, ref timestep)) return;
            DA.SetData(0, false);

            if(!C) return; // global safe switch

            string savedLocation = filepath + timestep.ToString("D4") + ".png";

            // Rendering Script
            Rhino.RhinoApp.Wait();
            Rhino.RhinoApp.RunScript("_RefreshShade _SelAll _Enter", echo:true);
            Rhino.RhinoApp.RunScript("_Render", echo:true);
            Rhino.RhinoApp.RunScript("_-SaveRenderWindowAs \n\"" + savedLocation + "\"\n", echo:true);
            Rhino.RhinoApp.RunScript("_-CloseRenderWindow", echo:true);

            DA.SetData(0, true);
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
            get { return new Guid("C2E01C7F-8B51-47F8-8833-54F4BC89832D"); }
        }
    }
}