using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_18ctRG
{
    public class FJ18ctRGCommand : Command
    {
        public FJ18ctRGCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJ18ctRGCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_18ctRG"; }
        }

        String[] matName = new String[] { "WhiteGold_18ct", "YellowGold_18ct", "RedGold_18ct", "Ruby" };
        int[] r = new int[] { 242,249,249,255};
        int[] g = new int[] { 241,232,187,187};
        int[] b = new int[] { 241,172,132,132};
        double[] fjMatRef = new double[] {0.9, 0.9, 0.9, 1.0};
        double[] fjMatTrans = new double[] {0.0, 0.0, 0.0, 0.5};
        double[] fjMatShine = new double[] {0.9, 0.9, 0.9, 1.0};

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Rhino.Input.Custom.OptionInteger intOption = new Rhino.Input.Custom.OptionInteger(1, 1, 99);

            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
            go.SetCommandPrompt("Select objects to apply material");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.GetMultiple(1, 0);

            go.AddOptionInteger("Material Index", ref intOption);

            int currIntOption = intOption.CurrentValue;

            int findsRM = doc.Materials.Find(matName[currIntOption], true);


            RhinoApp.WriteLine(findsRM.ToString());

            for (int i = 0; i < go.ObjectCount; i++)
            {
                
                Rhino.DocObjects.ObjRef objref = go.Object(i);
                Rhino.DocObjects.RhinoObject obj = objref.Object();

                if (findsRM == -1)
                {
                    Rhino.DocObjects.Material fjMat = new Rhino.DocObjects.Material();
                    fjMat.Reflectivity = fjMatRef[currIntOption];
                    fjMat.ReflectionColor = System.Drawing.Color.FromArgb(r[currIntOption], g[currIntOption], b[currIntOption]);
                    fjMat.Transparency = fjMatTrans[currIntOption];
                    fjMat.Shine = fjMatShine[currIntOption];
                    string fjMatName = matName[currIntOption];
                    fjMat.Name = fjMatName;
                    fjMat.CommitChanges();
                    var fjMatID = doc.Materials.Add(fjMat);

                    var fjRenderMat = Rhino.Render.RenderMaterial.CreateBasicMaterial(doc.Materials[fjMatID]);
                    doc.RenderMaterials.Add(fjRenderMat);
                    obj.RenderMaterial = fjRenderMat;
                    obj.CommitChanges();
                }
                else
                {
                    var objRm = obj.RenderMaterial;

                    if (objRm == null)
                    {
                        
                        obj.RenderMaterial = doc.RenderMaterials[0];
                        obj.CommitChanges();
                        obj.Attributes.MaterialIndex = findsRM;
                        obj.CommitChanges();
                    }
                    else
                    {
                        obj.Attributes.MaterialIndex = findsRM;
                        obj.CommitChanges();
                    }
                }
            }
            doc.Views.Redraw();
            return Result.Success;
        }
    }
}
