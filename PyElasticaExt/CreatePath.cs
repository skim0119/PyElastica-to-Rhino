using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PyElasticaExt
{
    public class CreatePath : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreatePath class.
        /// </summary>
        public CreatePath()
          : base(name:"CreatePath",
                 nickname: "Paths",
                 description: "Create paths for multiple items.",
                 category: "PyElastica",
                 subCategory: "Import")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Directory", "Dir", "Path that contains PyElastica exports", GH_ParamAccess.item);
            pManager.AddTextParameter("FileName", "File", "Each filename", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "List of combined paths.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string directory = "";
            string filename = "";
            DA.GetData(name: "Directory", ref directory);
            DA.GetData(name: "FileName", ref filename);

            string fullpath = directory + '\\' + filename;
            if (!System.IO.File.Exists(fullpath))
                throw new System.IO.DirectoryNotFoundException(fullpath+" not found.");
            DA.SetData("Path", fullpath);
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
                return PyElasticaExt.Properties.Resources.icons8_check_file_24.ToBitmap();
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6C93909B-6E4E-4E03-8626-21B3E15FC107"); }
        }
    }
}