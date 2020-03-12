using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_MatDia
{
    public class FJMatDiaCommand : Command
    {
        public FJMatDiaCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJMatDiaCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_MatDia"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
            go.SetCommandPrompt("Select objects to apply diamond material");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.GetMultiple(1, 0);

            Rhino.DocObjects.Material matDia = new Rhino.DocObjects.Material();
            matDia.Reflectivity = 0.5;
            matDia.Transparency = 0.55;
            matDia.SetEnvironmentTexture(@"C:\Users\SEB\Desktop\Dia3.jpg");
            string matDiaName = "Diamond white";
            matDia.Name = matDiaName;
            //int wg18ctIndex = matDia.MaterialIndex;

            matDia.CommitChanges();

            var idmatDia = doc.Materials.Add(matDia);

            var rm = Rhino.Render.RenderMaterial.CreateBasicMaterial(doc.Materials[idmatDia]);
            doc.RenderMaterials.Add(rm);

            for (int i = 0; i < go.ObjectCount; i++)
            {
                Rhino.DocObjects.ObjRef objref = go.Object(i);
                Rhino.DocObjects.RhinoObject obj = objref.Object();
                obj.RenderMaterial = rm;
                obj.CommitChanges();


            }

            doc.Views.Redraw();
            return Result.Success;
        }
    }
}
