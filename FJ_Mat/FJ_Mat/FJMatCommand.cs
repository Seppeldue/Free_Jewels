﻿using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.Render;
using Rhino.DocObjects;




namespace FJ_Mat
{
    public class FJMatCommand : Command
    {
        public FJMatCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJMatCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_Mat"; }
        }
        RenderMaterial FindMaterial(RhinoDoc doc, string name)
        {
            foreach (var material in doc.RenderMaterials)
            {
                if (material.Name == name)
                {
                    return material;
                }
            }

            return null;
        }

        String[] matName = new String[] { "WhiteGold_18ct", "YellowGold_18ct", "RedGold_18ct", "RodiumBlack_18ct","Silver_925","Platinum_960","Ruby","Emerald","Saphir","Paraiba","Granat","Amethyst","Morganite", "Diamond" };
        int[] r = new int[] { 230, 243, 255, 50, 242, 230, 255, 0,  0,  0,220,166,225,255 };
        int[] g = new int[] { 229, 224, 195, 50, 241, 229, 0,  99,  0,206, 84, 26,148,255 };
        int[] b = new int[] { 229, 166, 160, 50, 241, 229, 0,   0,170,255, 27,184,148,255 };
        
        double[] fjMatTrans = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.2, 0.2, 0.2, 0.2, 0.3, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2 };
        double[] fjMatShine = new double[] { 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            int currIntOption = 0;
            var gi = new GetOption();
            gi.SetCommandPrompt("Material Index");
            gi.AcceptNumber(true,true);
            var resgi = gi.Get();
            if (resgi == GetResult.Number)
                currIntOption = Convert.ToInt32(gi.Number());

            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
            go.SetCommandPrompt("Select objects to apply material");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.GetMultiple(1, 0);

            
            //Nothing to do - get out quick.
            if (go.ObjectCount == 0)
                return Result.Success;

            var rm = FindMaterial(doc, matName[currIntOption]);
            if (null == rm)
            {
                //Didn't find the material - create one and carry on.

                //Create a basic material
                var custom = new Rhino.DocObjects.Material();
                custom.Name = matName[currIntOption];
         
                if (currIntOption == 13)
                    custom.SetEnvironmentTexture(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"/Free Jewels Rhino Plug-Ins/Dia3.jpg");
                custom.CommitChanges();

                rm = RenderMaterial.CreateBasicMaterial(custom);
                rm.BeginChange(RenderContent.ChangeContexts.Program);
                rm.SetParameter("diffuse", System.Drawing.Color.Black);
                rm.SetParameter("reflectivity", 1.0);
                rm.SetParameter("reflectivity-color", System.Drawing.Color.FromArgb(r[currIntOption], g[currIntOption], b[currIntOption]));
                rm.SetParameter("transparency", fjMatTrans[currIntOption]);
                rm.SetParameter("shine", fjMatShine[currIntOption]);
                rm.SetParameter("polish-amount", fjMatShine[currIntOption]);
                rm.EndChange();

                var docMats = doc.RenderMaterials;

                docMats.Add(rm);
                
            }

            //Now we always have a material to assign, this part is easy
            for (int i = 0; i < go.ObjectCount; i++)
            {
                var obj = go.Object(i).Object();

                obj.RenderMaterial = rm;
                obj.CommitChanges();
            }

            doc.Views.Redraw();
            return Result.Success;
        }
    }
    
}
