using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_IterativeOffset
{
    public class FJIterativeOffsetCommand : Command
    {
        public FJIterativeOffsetCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJIterativeOffsetCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_IterativeOffset"; }
        }
        double distVal = 1.0;
        bool direction = true;
        int cornerIndex = 1;
        CurveOffsetCornerStyle[] cornerStyle = {CurveOffsetCornerStyle.None, CurveOffsetCornerStyle.Sharp, CurveOffsetCornerStyle.Round, CurveOffsetCornerStyle.Smooth, CurveOffsetCornerStyle.Chamfer };
        List<String> corners = new List<string>{"None", "Sharp", "Round", "Smooth", "Chamfer"};
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            

            GetObject go = new GetObject();
            go.SetCommandPrompt("Select curves to offset.");
            var dist = new Rhino.Input.Custom.OptionDouble(distVal);
            go.AddOptionDouble("Distance", ref dist);
            //var dir = new Rhino.Input.Custom.OptionToggle(true, "Out", "In");
            //go.AddOptionToggle("Direction", ref dir);
            
            go.AddOptionList("CornerStyle", corners, cornerIndex);
            go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            go.GroupSelect = true;
            go.SubObjectSelect = true;
            go.DeselectAllBeforePostSelect = true;
            go.OneByOnePostSelect = false;
            go.GetMultiple(1, 0);

            for (int i = 0; i < go.ObjectCount; i++)
            {
                BoundingBox bbObj = go.Object(i).Geometry().GetBoundingBox(true);
                Point3d bbObjCenter = bbObj.Center;
                
                
                Rhino.DocObjects.ObjRef objref = go.Object(i);
                var curve = objref.Curve();
                if (curve == null)
                    return Result.Nothing;

                curve.DivideByCount(3,true,out Point3d[] curvePoints);
                Plane plane = new Plane(curvePoints[0], curvePoints[1], curvePoints[2]);
                //var planeResult = plane.FitPlaneToPoints(IEnumerable < Point3d > curvePoints, out Plane curvePlane);
                Vector3d curveNormal = plane.Normal;
                distVal = dist.CurrentValue;
                cornerIndex = go.Option().CurrentListOptionIndex;
                //direction = dir.CurrentValue;

                var curves = curve.Offset(bbObjCenter, curveNormal, -distVal, doc.ModelAbsoluteTolerance, cornerStyle[cornerIndex]);

                foreach (var offsetcurve in curves)
                    doc.Objects.AddCurve(offsetcurve);
        }

            
            doc.Views.Redraw();
            

            // ---

            return Result.Success;
        }
    }
}
