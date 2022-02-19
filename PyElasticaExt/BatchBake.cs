using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
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
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Debug", "D", "Debug Output", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Succeed", "S", "Module finished", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool C = false; // global safe switch
            string debug_string = "";

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref C)) return;
            if(!C) return; // global safe switch
            /*
            # add objects
        for idx, geo_id in enumerate(geo):
            sc.doc = ghdoc
            doc_object = rs.coercerhinoobject(geo_id)
            
            geometry = doc_object.Geometry
            attributes = doc_object.Attributes
            
            sc.doc = Rhino.RhinoDoc.ActiveDoc
        
            rhino_ref = sc.doc.Objects.Add(geometry, attributes)
        
            rs.ObjectLayer(rhino_ref, layer=layer)
        
        # return outputs if you have them; here I try it for you:
        sc.doc = ghdoc
        
        rs.Command("_-ClearUndo ")
            */
            DA.SetData(0, debug_string);
            DA.SetData(1, true);
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
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2EAA041F-BAFD-454C-8116-BD909E73B49D"); }
        }
    }
}