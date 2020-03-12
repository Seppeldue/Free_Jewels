using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_18ctWG
{
    public class FJ18ctWGCommand : Command
    {
        public FJ18ctWGCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJ18ctWGCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_18ctWG"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
            go.SetCommandPrompt("Select objects to apply 18ct Whitegold");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.GetMultiple(1, 0);

            Rhino.DocObjects.Material wg18ct = new Rhino.DocObjects.Material();
            wg18ct.Reflectivity = 1;
            string wg18ctName = "18ct white gold";
            wg18ct.Name = wg18ctName;
            int wg18ctIndex = wg18ct.MaterialIndex;
            
            wg18ct.CommitChanges();

            var idwg18 = doc.Materials.Add(wg18ct);
            
            var rm = Rhino.Render.RenderMaterial.CreateBasicMaterial(doc.Materials[idwg18]);
            doc.RenderMaterials.Add(rm);

            for (int i = 0; i < go.ObjectCount; i++)
            {
                Rhino.DocObjects.ObjRef objref = go.Object(i);
                Rhino.DocObjects.RhinoObject obj = objref.Object();
                obj.RenderMaterial = rm;
                //obj.Attributes.MaterialIndex = wg18ctIndex;
                obj.CommitChanges();
                
            
            }

            doc.Views.Redraw();
            RhinoApp.WriteLine(""+wg18ctIndex);
            return Result.Success;
        }
    }
}
