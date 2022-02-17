using Grasshopper;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Numpy;

namespace PyElasticaExt
{
    public class PyElasticaImport : GH_Component
    {
        string testpath = "E:\\Rendering_Octopus_paper\\pickles\\curl";
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public PyElasticaImport()
          : base(name: "CosseratRodImport",
                 nickname: "CosseratRods",
                 description: "Create Cosserat Rods",
                 category: "PyElastica",
                 subCategory: "Primitive")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddBooleanParameter("Switch", "C", "Module switch", GH_ParamAccess.item, false);
            pManager.AddTextParameter("FilePath", "Pa", "Path that contains PyElastica exports", GH_ParamAccess.item, "");

            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddBooleanParameter("isBuild", "B", "Indicator if build stage is done", GH_ParamAccess.item);
            pManager.AddTextParameter("Debug", "D", "Debug Output", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Succeed", "S", "Module finished", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.
            Plane plane = Plane.WorldXY;
            double radius0 = 0.0;
            double radius1 = 0.0;
            int turns = 0;
            string filepath = "";
            bool C = false; // global safe switch

            Line line = new Line();

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetData(1, ref filepath)) return;

            if(!C) return; // global safe switch

            // We should now validate the data and warn the user if invalid data is supplied.
            if (radius0 < 0.0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Inner radius must be bigger than or equal to zero");
                return;
            }
            if (radius1 <= radius0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Outer radius must be bigger than the inner radius");
                return;
            }
            if (turns <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Spiral turn count must be bigger than or equal to one");
                return;
            }

            // We're set to create the spiral now. To keep the size of the SolveInstance() method small, 
            // The actual functionality will be in a different method:
            Curve spiral = CreateSpiral(plane, radius0, radius1, turns);

            // Finally assign the spiral to the output parameter.
            DA.SetData(0, spiral);
            DA.SetData(1, spiral);
        }
        
        Numpy.NDarray Load(string key)
        {
            var data = Numpy.np.load(testpath, allow_pickle:true);
            return new NDarray(data.self[key]);
        }

        Curve CreateSpiral(Plane plane, double r0, double r1, Int32 turns)
        {
            Line l0 = new Line(plane.Origin + r0 * plane.XAxis, plane.Origin + r1 * plane.XAxis);
            Line l1 = new Line(plane.Origin - r0 * plane.XAxis, plane.Origin - r1 * plane.XAxis);

            Point3d[] p0;
            Point3d[] p1;

            l0.ToNurbsCurve().DivideByCount(turns, true, out p0);
            l1.ToNurbsCurve().DivideByCount(turns, true, out p1);

            PolyCurve spiral = new PolyCurve();

            for (int i = 0; i < p0.Length - 1; i++)
            {
                Arc arc0 = new Arc(p0[i], plane.YAxis, p1[i + 1]);
                Arc arc1 = new Arc(p1[i + 1], -plane.YAxis, p0[i + 1]);

                spiral.Append(arc0);
                spiral.Append(arc1);
            }

            return spiral;
        }

        private void Bake(bool C, Brep B)
        {
            if (C)
            {
                Rhino.DocObjects.ObjectAttributes objectAttributes = new Rhino.DocObjects.ObjectAttributes();
                objectAttributes.LayerIndex = 2;
                Rhino.RhinoDoc.ActiveDoc.Objects.AddBrep(B, objectAttributes);
            }
        }

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => PyElasticaExt.Properties.Resources.icons8_snake_24.ToBitmap();

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("64D90F35-E231-4D53-9144-B58EE294A478");
    }
}