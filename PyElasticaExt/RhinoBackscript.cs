using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Rhino.Commands;

namespace PyElasticaExt
{
    class RhinoBackscript
    {
        public static Result CreateLayer(string layer_name)
        {
            var doc = RhinoDoc.ActiveDoc;

            // Does a layer with the same name already exist?
            int layer_index = doc.Layers.Find(layer_name, true);
            if (layer_index >= 0)
            {
                return Result.Success; // Cancel
            }

            // Was a layer named entered?
            if (string.IsNullOrEmpty(layer_name))
            {
                RhinoApp.WriteLine("Layer name cannot be blank.");
                return Result.Cancel;
            }

            // Is the layer name valid?
            if (!Rhino.DocObjects.Layer.IsValidName(layer_name))
            {
                RhinoApp.WriteLine(layer_name + " is not a valid layer name.");
                return Result.Cancel;
            }

            // Add a new layer to the document
            layer_index = doc.Layers.Add(layer_name, System.Drawing.Color.Black);
            if (layer_index < 0)
            {
                RhinoApp.WriteLine("Unable to add {0} layer.", layer_name);
                return Result.Failure;
            }
            return Result.Success;
        }
        public static Result CreateSubLayer(string parent_name, string child_name)
        {
            CreateLayer(parent_name);
            var doc = RhinoDoc.ActiveDoc;

            // Does a layer with the same name already exist?
            int parent_index = doc.Layers.Find(parent_name, true);
            Rhino.DocObjects.Layer parent_layer = doc.Layers[parent_index];

            // Was a layer named entered?
            if (string.IsNullOrEmpty(child_name))
            {
                RhinoApp.WriteLine("Layer name cannot be blank.");
                return Result.Cancel;
            }

            // Is the layer name valid?
            if (!Rhino.DocObjects.Layer.IsValidName(child_name))
            {
                RhinoApp.WriteLine(child_name + " is not a valid layer name.");
                return Result.Cancel;
            }
            // Add a new layer to the document
            int child_index = doc.Layers.Find(child_name, true);
            Rhino.DocObjects.Layer child_layer = new Rhino.DocObjects.Layer();
            child_layer.ParentLayerId = parent_layer.Id;
            child_layer.Name = child_name;
            child_layer.Color = System.Drawing.Color.Red;

            child_index = doc.Layers.Add(child_layer);
            if (child_index < 0)
            {
                //RhinoApp.WriteLine(child_name + " already made.");
                return Result.Success;
            }
            return Result.Success;
        }

        public static Result CleanLayer(Rhino.DocObjects.Layer layer)
        {
            var doc = RhinoDoc.ActiveDoc;
            Rhino.DocObjects.RhinoObject[] objs = doc.Objects.FindByLayer(layer);

            if (objs == null || objs.Length == 0)
            {
                // Nothing to delete
                return Result.Success;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                doc.Objects.Delete(objs[i], quiet: true);
            }
            return Result.Success;
        }
    }
}
