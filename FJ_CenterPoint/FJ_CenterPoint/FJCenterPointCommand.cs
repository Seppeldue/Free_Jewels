using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_CenterPoint
{
    public class FJCenterPointCommand : Command
    {
        public FJCenterPointCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJCenterPointCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_CenterPoint"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            var layerCheck = doc.Layers.FindName("CenterPoints");
            if (layerCheck == null)
            {
                doc.Layers.Add("CenterPoints", System.Drawing.Color.Blue);
                layerCheck = doc.Layers.FindName("CenterPoints");
            }

            //pick objects to add center point
            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
            go.SetCommandPrompt("Select objects to add center point");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.GetMultiple(1, 0);

            //Compute center and add point to doc
            
            for (int i = 0; i < go.ObjectCount; i++)
            {
                Guid pointG;
                Rhino.DocObjects.ObjRef objref = go.Object(i);
                Rhino.DocObjects.RhinoObject obj = objref.Object();
                var objType = obj.ObjectType;
                if (objType == Rhino.DocObjects.ObjectType.Curve)
                {
                    Curve curve = objref.Curve();
                    Circle circle = new Circle();
                    if (!curve.TryGetCircle(out circle, doc.ModelAbsoluteTolerance))
                    {
                        BoundingBox bbObj = go.Object(i).Geometry().GetBoundingBox(true);
                        Point3d bbObjCenter = bbObj.Center;
                        pointG = doc.Objects.AddPoint(bbObjCenter);
                    }
                    else
                    {
                        Point3d circleCenter = circle.Center;
                        pointG = doc.Objects.AddPoint(circleCenter);
                    }
                }
                else
                {
                    BoundingBox bbObj = go.Object(i).Geometry().GetBoundingBox(true);
                    Point3d bbObjCenter = bbObj.Center;
                    pointG = doc.Objects.AddPoint(bbObjCenter);

                }
                Rhino.DocObjects.RhinoObject circObj = doc.Objects.Find(pointG);
                circObj.Attributes.LayerIndex = layerCheck.Index;
                circObj.CommitChanges();
            }
            doc.Views.Redraw();
            return Result.Success;
        }
    }
}
