using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_Prongs
{
    public class FJProngsCommand : Command
    {
        public FJProngsCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJProngsCommand Instance
        {
            get; private set;
        }


        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_Prongs"; }
        }

        // ESC Key Break
        void RhinoApp_EscapeKeyPressed(object sender, EventArgs e)
        {
            m_escape_key_pressed = true;
        }

        private bool m_escape_key_pressed = false;

        
        double prongDia = 1.0;
        int prongCount = 4;
        double hight = 0.1;
        double length = 1;
        double rotation = 0.0;
        double capHight = 1.0;
        Boolean capBool = true;
        PipeCapMode capMode = PipeCapMode.Round;

        List<Guid> ids = new List<Guid>();

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            double tolerance = doc.ModelAbsoluteTolerance;
            double angleTolerance = doc.ModelAngleToleranceRadians;

            
            List<Curve> icur = new List<Curve>();

            GetObject gcr = new Rhino.Input.Custom.GetObject();
            gcr.SetCommandPrompt("Select reference circles for prongs. (No curves)");
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

                Point3d pt0 = new Point3d(0, 0, hight);
                Point3d pt1 = new Point3d(0, 0, -(length-hight));

                double rotationRadiant = ((rotation / 180) * 3.1415);
                
                List<Point3d> curvePoints = new List<Point3d>() {pt0,pt1};
                Curve prongCurve = Curve.CreateInterpolatedCurve(curvePoints, 3);
                
                if (!capBool)
                    capMode = PipeCapMode.Flat;
                else if (capBool)
                    capMode = PipeCapMode.Round;

                Brep[] cylBrep1 = Brep.CreatePipe(prongCurve, prongDia/2, false, capMode, false, tolerance, angleTolerance);
                Brep cylBrep2 = cylBrep1[0];
                cylBrep2.Faces.RemoveAt(2);
                Brep cylBrep3 = cylBrep2.CapPlanarHoles(tolerance);

                if (capBool)
                {
                    Vector3d scaleVec = new Vector3d(0, 0, 1);
                    var scalePlane = new Plane(pt0, scaleVec);
                    var capScale = Transform.Scale(scalePlane, 1.0, 1.0, capHight);
                    var surf2 = cylBrep3.Faces[1].UnderlyingSurface();
                    NurbsSurface nurbSurf2 =  surf2.ToNurbsSurface();
                    nurbSurf2.Transform(capScale);
                    Brep capBrep = nurbSurf2.ToBrep();
                    cylBrep3.Append(capBrep);
                    cylBrep3.Faces.RemoveAt(1);
                }
              
                cylBrep3.Compact();
                cylBrep3.JoinNakedEdges(tolerance);
                string instDefCount = DateTime.Now.ToString("ddMMyyyyHHmmss");
                Brep[] brepArray = new Brep[1] { cylBrep3 };
                var prongIndex = doc.InstanceDefinitions.Add("Prong" + instDefCount, "RefProng", Point3d.Origin, brepArray);

                foreach (Curve c in icur)
                {
                    Circle circleProngs = new Circle();
                    c.TryGetCircle(out circleProngs, tolerance);
                    NurbsCurve curveProngs = circleProngs.ToNurbsCurve();
                    
                    Plane planeNew = circleProngs.Plane;
                    Vector3d plVec = planeNew.Normal;
                    
                    if (planeNew == null)
                    {
                        curveProngs.DivideByCount(3, true, out Point3d[] curveProngsPoints);
                        planeNew = new Plane(curvePoints[0], curveProngsPoints[1], curveProngsPoints[2]);
                    }

                    
                    Curve[] offsetCurveArray = curveProngs.Offset(planeNew, prongDia/4, tolerance, CurveOffsetCornerStyle.Sharp);

                    Curve offsetCurve = offsetCurveArray[0];

                    double[] segParameters = offsetCurve.DivideByCount(prongCount, true);

                    List<Point3d> prongPoints = new List<Point3d>();
                    List<Guid> idsInter = new List<Guid>();

                    foreach (double p in segParameters)
                    {
                        Point3d zen = new Point3d(0, 0, 0);
                        Point3d prongPoint = offsetCurve.PointAt(p);

                        Point3d center = circleProngs.Center;
                        Vector3d moveV = new Vector3d(prongPoint);
                        Vector3d zaxis = new Vector3d(0.0, 0.0, 1.0);
                        Plane planeOr = new Plane(zen, zaxis);
                        Plane plane = new Plane(prongPoint, plVec);

                        var transform1 = Transform.PlaneToPlane(planeOr, plane);
                        var transform2 = Transform.Rotation(rotationRadiant, plVec, center);

                        var prongA = doc.Objects.AddInstanceObject(prongIndex, transform1);
                        var prongB = doc.Objects.Transform(prongA, transform2, true);

                        ids.Add(prongB);

                    }
                    doc.Views.Redraw();
                }


                GetOption go = new Rhino.Input.Custom.GetOption();
                go.SetCommandPrompt("Set prong parameters.");
                go.AcceptNothing(true);

                var countProng = new Rhino.Input.Custom.OptionInteger(prongCount);
                var diaProng = new Rhino.Input.Custom.OptionDouble(prongDia);
                var hightProng = new Rhino.Input.Custom.OptionDouble(hight);
                var lengthProng = new Rhino.Input.Custom.OptionDouble(length);
                var rotProng = new Rhino.Input.Custom.OptionDouble(rotation);
                var capProng = new Rhino.Input.Custom.OptionToggle(capBool, "Flat","Round");
                var hightCap = new Rhino.Input.Custom.OptionDouble(capHight);

                go.AddOptionInteger("Count", ref countProng);
                go.AddOptionDouble("Diameter", ref diaProng);
                go.AddOptionDouble("Hight", ref hightProng);
                go.AddOptionDouble("Length", ref lengthProng);
                go.AddOptionDouble("Rotation", ref rotProng);
                go.AddOptionToggle("Cap", ref capProng);
                if (capBool)
                {
                    go.AddOptionDouble("Caphight", ref hightCap);
                }

                GetResult res = go.Get();

                if (m_escape_key_pressed) break;

                hight = hightProng.CurrentValue;
                length = lengthProng.CurrentValue;
                prongCount = countProng.CurrentValue;
                prongDia = diaProng.CurrentValue;
                rotation = rotProng.CurrentValue;
                capBool = capProng.CurrentValue;
                capHight = hightCap.CurrentValue;

                if (length <= 0.0) length = 1.0;
                if (prongCount <= 0) prongCount = 1;
                if (prongDia <= 0.0) prongDia = 1.0;
                if (res == GetResult.Nothing) break;

                doc.Objects.Delete(ids, true);

            }
            RhinoApp.EscapeKeyPressed -= RhinoApp_EscapeKeyPressed;

            doc.Groups.Add(ids);

            return Result.Success;
        }
    }
}
