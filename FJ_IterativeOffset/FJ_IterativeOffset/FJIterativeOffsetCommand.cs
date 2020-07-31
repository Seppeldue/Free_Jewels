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
        int cornerIndex = 1;
        CurveOffsetCornerStyle[] cornerStyle = {CurveOffsetCornerStyle.None, CurveOffsetCornerStyle.Sharp, CurveOffsetCornerStyle.Round, CurveOffsetCornerStyle.Smooth, CurveOffsetCornerStyle.Chamfer };
        List<String> corners = new List<string>{"None", "Sharp", "Round", "Smooth", "Chamfer"};
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            int counter = 0;

            GetObject go = new GetObject();
            go.SetCommandPrompt("Select curves to offset.");
            go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            go.GroupSelect = true;
            go.SubObjectSelect = true;
            go.DeselectAllBeforePostSelect = true;
            go.OneByOnePostSelect = false;
            go.GetMultiple(1, 0);


            GetOption gop = new GetOption();
            var dist = new Rhino.Input.Custom.OptionDouble(distVal);
            gop.SetCommandPrompt("Set values. Press enter when done.");
            gop.AddOptionDouble("Distance", ref dist);
            int optList = gop.AddOptionList("CornerStyle", corners, cornerIndex);
            gop.AcceptNothing(true);

            while (true)
            {

                if (gop.Get() == GetResult.Nothing)
                    break;

                else if (gop.OptionIndex() == optList)
                    cornerIndex = gop.Option().CurrentListOptionIndex;

                else 
                    distVal = dist.CurrentValue;
 
            }


            

            for (int i = 0; i < go.ObjectCount; i++)
            {
                
                Rhino.DocObjects.ObjRef objref = go.Object(i);
                var curve = objref.Curve();
                if (curve == null)
                    return Result.Nothing;
                Plane plane;
                if (!curve.TryGetPlane(out plane))
                {
                    curve.DivideByCount(3, true, out Point3d[] curvePoints);
                    plane = new Plane(curvePoints[0], curvePoints[1], curvePoints[2]);
                }

                try
                {
                    var curves = curve.Offset(plane, distVal, doc.ModelAbsoluteTolerance, cornerStyle[cornerIndex]);
                    foreach (var offsetcurve in curves)
                        doc.Objects.AddCurve(offsetcurve);
                }
                catch
                {
                    counter++;
                }

            }

            if (counter > 0)
                RhinoApp.WriteLine(counter+" out of "+go.ObjectCount+" offset values were out of scope!");

            doc.Views.Redraw();
            

            // ---

            return Result.Success;
        }
    }
}
