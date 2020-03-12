using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_18ctYG
{
    public class FJ18ctYGCommand : Command
    {
        public FJ18ctYGCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJ18ctYGCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_18ctYG"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
            go.SetCommandPrompt("Select objects to apply 18ct yellow gold");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.GetMultiple(1, 0);

            Rhino.DocObjects.Material yg18ct = new Rhino.DocObjects.Material();
            yg18ct.Reflectivity = 1;
            yg18ct.ReflectionColor = System.Drawing.Color.FromArgb(235, 215, 142);
            string yg18ctName = "18ct yellow gold";
            yg18ct.Name = yg18ctName;
            int wg18ctIndex = yg18ct.MaterialIndex;

            yg18ct.CommitChanges();

            var idwg18 = doc.Materials.Add(yg18ct);

            var rmyg18 = Rhino.Render.RenderMaterial.CreateBasicMaterial(doc.Materials[idwg18]);
            doc.RenderMaterials.Add(rmyg18);

            for (int i = 0; i < go.ObjectCount; i++)
            {
                Rhino.DocObjects.ObjRef objref = go.Object(i);
                Rhino.DocObjects.RhinoObject obj = objref.Object();
                obj.RenderMaterial = rmyg18;
                //obj.Attributes.MaterialIndex = wg18ctIndex;
                obj.CommitChanges();


            }

            doc.Views.Redraw();
            return Result.Success;
        }
    }
}
