using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_NumberObjects
{
    public class FJNumberObjectsCommand : Command
    {
        public FJNumberObjectsCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJNumberObjectsCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_NumberObjects"; }
        }

        double height = 1.0;

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var layerCheck = doc.Layers.FindName("Numbers");
            if (layerCheck == null)
            {
                doc.Layers.Add("Numbers", System.Drawing.Color.DarkRed);
                layerCheck = doc.Layers.FindName("Numbers");
            }

            //pick objects to add center point
            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
            go.SetCommandPrompt("Select objects to number");
            var heightref = new Rhino.Input.Custom.OptionDouble(height);
            go.AddOptionDouble("Text_height_mm", ref heightref);
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.GetMultiple(1, 0);

            //Compute center and add point to doc

            for (int i = 0; i < go.ObjectCount; i++)
            {
                height = heightref.CurrentValue;
                Rhino.DocObjects.ObjRef objref = go.Object(i);
                Guid objGuid = objref.ObjectId;
                BoundingBox bbObj = go.Object(i).Geometry().GetBoundingBox(true);
                Point3d bbObjCenter = bbObj.Center;

                int a = i+1;
                
                Plane plane = Plane.WorldXY;
                plane.Origin = bbObjCenter;
                var Justification = TextJustification.MiddleCenter;
                string text = a.ToString();
                var Font = "Arial";
                
                Guid num = doc.Objects.AddText(text, plane, height, Font, false, false, Justification);

                Rhino.DocObjects.RhinoObject circObj = doc.Objects.Find(num);
                circObj.Attributes.LayerIndex = layerCheck.Index;
                circObj.CommitChanges();

                int groupIndex = doc.Groups.Count;
                groupIndex++;
                string group = groupIndex.ToString();
                var gI = doc.Groups.Add(group);
                doc.Groups.AddToGroup(gI, num);
                doc.Groups.AddToGroup(gI, objGuid);
            }
            doc.Views.Redraw();
            return Result.Success;
        }
    }
}
