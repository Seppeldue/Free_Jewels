using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_PaveDrill
{
    public class FJPaveDrillCommand : Command
    {
        public FJPaveDrillCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJPaveDrillCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_PaveDrill"; }
        }

        // ESC Key Break
        void RhinoApp_EscapeKeyPressed(object sender, EventArgs e)
        {
            m_escape_key_pressed = true;
        }

        private bool m_escape_key_pressed = false;

        double drillScale = 1.0;
        double topDrill = 0.1;
        double midDrill = 0.3;
        double ancDrill = 0.4;
        double bottDrill = 1.5;
        double headDrill = 0.1;
        double ancheadDrill = 0.5;

        List<Guid> ids = new List<Guid>();

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            double tolerance = doc.ModelAbsoluteTolerance;

            List<Curve> icur = new List<Curve>();

            GetObject gcr = new Rhino.Input.Custom.GetObject();
            gcr.SetCommandPrompt("Select reference circles for drill. (No curves)");
            gcr.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            gcr.GroupSelect = true;
            gcr.SubObjectSelect = true;
            gcr.DeselectAllBeforePostSelect = true;
            gcr.OneByOnePostSelect = false;
            gcr.GetMultiple(1, 0);

            for (int ie = 0; ie < gcr.ObjectCount; ie++)
            {
                Rhino.DocObjects.ObjRef objref = gcr.Object(ie);
                Rhino.DocObjects.RhinoObject obj = objref.Object();
                if (obj == null)
                    return Result.Failure;
                Curve refcr = objref.Curve();
                if (refcr == null)
                    return Result.Failure;
                obj.Select(false);

                icur.Add(refcr);
            }

            while (true)
            {
                m_escape_key_pressed = false;
                RhinoApp.EscapeKeyPressed += RhinoApp_EscapeKeyPressed;


                List<Curve> curveDrill = new List<Curve>();

                Point3d centerTop = new Point3d(0, 0, (headDrill + topDrill) * drillScale);
                Circle circleTop = new Circle(centerTop, ancheadDrill / 2 * drillScale);
                Curve curveTop = circleTop.ToNurbsCurve();
                curveDrill.Add(curveTop);
                Point3d centerMid = new Point3d(0, 0, topDrill * drillScale);
                Circle circleMid = new Circle(centerMid, drillScale / 2);
                Curve curveMid = circleMid.ToNurbsCurve();
                curveDrill.Add(curveMid);
                Circle circleBase = new Circle(drillScale / 2);
                Curve curveBase = circleBase.ToNurbsCurve();
                curveDrill.Add(curveBase);
                Point3d centerLow = new Point3d(0, 0, -midDrill * drillScale);
                Circle circleLow = new Circle(centerLow, ancDrill / 2 * drillScale);
                Curve curveLow = circleLow.ToNurbsCurve();
                curveDrill.Add(curveLow);
                if (bottDrill != 0)
                {
                    Point3d centerBot = new Point3d(0, 0, (-midDrill - bottDrill) * drillScale);
                    Circle circleBot = new Circle(centerBot, ancDrill / 2 * drillScale);
                    Curve curveBot = circleBot.ToNurbsCurve();
                    curveDrill.Add(curveBot);
                }
                

                Brep[] cylBrep1 = Brep.CreateFromLoft(curveDrill, Point3d.Unset, Point3d.Unset, LoftType.Straight, false);
                Brep[] cylBrep2 = Brep.CreateBooleanUnion(cylBrep1, tolerance);
                Brep cylBrepFinal = cylBrep1[0];
                cylBrepFinal = cylBrepFinal.CapPlanarHoles(tolerance);


                string instDefCount = DateTime.Now.ToString("ddMMyyyyHHmmss");
                Brep[] brepArray = new Brep[1] { cylBrepFinal };
                var drillIndex = doc.InstanceDefinitions.Add("Drill" + instDefCount, "RefDrill 1mm", Point3d.Origin, brepArray);

                foreach (Curve c in icur)
                {
                    Circle circleDrill = new Circle();
                    c.TryGetCircle(out circleDrill, tolerance);
                    
                    double radiusDrill = circleDrill.Diameter;
                    Point3d center = circleDrill.Center;
                    Vector3d moveV = new Vector3d(center);
                    Vector3d zaxis = new Vector3d(0.0, 0.0, 1.0);
                    Plane planeOr = new Plane(center, zaxis);
                    Plane planeNew = circleDrill.Plane;
                    var transform1 = Transform.Translation(moveV);
                    var transform2 = Transform.Scale(center, radiusDrill);
                    var transform3 = Transform.PlaneToPlane(planeOr, planeNew);

                    var stoneA = doc.Objects.AddInstanceObject(drillIndex, transform1);
                    var stoneB = doc.Objects.Transform(stoneA, transform2, true);
                    var stoneC = doc.Objects.Transform(stoneB, transform3, true);

                    ids.Add(stoneC);

                    doc.Views.Redraw();
                }


                GetOption go = new Rhino.Input.Custom.GetOption();
                go.SetCommandPrompt("Set drill parameters.");
                go.AcceptNothing(true);

                var drillHead = new Rhino.Input.Custom.OptionDouble(headDrill);
                var drillAncHead = new Rhino.Input.Custom.OptionDouble(ancheadDrill);
                var drillTop = new Rhino.Input.Custom.OptionDouble(topDrill);
                var drillMid = new Rhino.Input.Custom.OptionDouble(midDrill);
                var drillAnc = new Rhino.Input.Custom.OptionDouble(ancDrill);
                var drillBott = new Rhino.Input.Custom.OptionDouble(bottDrill);
                var scaleDrill = new Rhino.Input.Custom.OptionDouble(drillScale);
                go.AddOptionDouble("Top", ref drillHead);
                go.AddOptionDouble("TopAngle", ref drillAncHead);
                go.AddOptionDouble("Mid", ref drillTop);
                go.AddOptionDouble("Bottom", ref drillMid);
                go.AddOptionDouble("BottomAngle", ref drillAnc);
                go.AddOptionDouble("Tail", ref drillBott);
                go.AddOptionDouble("Scale", ref scaleDrill);


                GetResult res = go.Get();

                if (m_escape_key_pressed) break;

                

                topDrill = drillTop.CurrentValue;
                midDrill = drillMid.CurrentValue;
                ancDrill = drillAnc.CurrentValue;
                bottDrill = drillBott.CurrentValue;
                drillScale = scaleDrill.CurrentValue;
                headDrill = drillHead.CurrentValue;
                ancheadDrill = drillAncHead.CurrentValue;


                if (res == GetResult.Nothing) break;

                doc.Objects.Delete(ids, true);

            }
            RhinoApp.EscapeKeyPressed -= RhinoApp_EscapeKeyPressed;

            doc.Groups.Add(ids);

            return Result.Success;
        }
    }
}
