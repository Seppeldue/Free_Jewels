using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_PaveOnLine
{
    public class FJPaveOnLineCommand : Command
    {
        public FJPaveOnLineCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJPaveOnLineCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_PaveOnLine"; }
        }


        //Globals
        bool optionBool = false;
        double diamStone = 1.0;
        double offSetStone = 0.1;
        double stoneDist = 0.1;
        Curve curve;
        Surface surface;

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            
            double tolerance = doc.ModelAbsoluteTolerance;
            List<Curve> curves = new List<Curve>();

            //Select surface

            GetObject gs = new Rhino.Input.Custom.GetObject();
            gs.SetCommandPrompt("Surface to orient on");
            gs.GeometryFilter = Rhino.DocObjects.ObjectType.Surface;
            gs.SubObjectSelect = true;
            gs.DeselectAllBeforePostSelect = true;
            gs.OneByOnePostSelect = true;
            gs.Get();
            if (gs.CommandResult() != Result.Success)
                return gs.CommandResult();

            Rhino.DocObjects.ObjRef objref_Surface = gs.Object(0);
            Rhino.DocObjects.RhinoObject obj = objref_Surface.Object();
            if (obj == null)
                return Result.Failure;
            surface = objref_Surface.Surface();

            if (surface == null)
                return Result.Failure;
            obj.Select(false);

            //Select Line(s)
            GetObject gl = new GetObject();
            gl.SetCommandPrompt("Select one or two line(s)");
            gl.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            gl.DeselectAllBeforePostSelect = true;
            gl.OneByOnePostSelect = true;
            gl.GetMultiple(1, 0);

            for (int i = 0; i < gl.ObjectCount; i++)
            {
                Rhino.DocObjects.ObjRef objref_Line = gl.Object(i);
                curve = objref_Line.Curve();
                
                Curve curveRe = curve.Rebuild(60, 3, true);
                curves.Add(curveRe);
            }

            List<Guid> cir_guid_list = new List<Guid>();

            if (curves.Count > 1)
            {
                Curve curve2 = curves[0];
                Curve curve3 = curves[1];
                if(curve2.IsClosed || curve3.IsClosed)
                {
                    Rhino.UI.Dialogs.ShowMessage("Please only select open curves for two line pave.", "Warning!");
                    return Result.Failure;
                }
                
                while (true)
                {
                    
                    cir_guid_list = new List<Guid>();
                    



                    var tweenCurves = Curve.CreateTweenCurvesWithSampling(curve2, curve3, 1, 30, tolerance);
                    Curve tCurve = tweenCurves[0];

                    //3 point circle
                    Point3d po1 = curve2.PointAtStart;
                    Point3d po2 = curve3.PointAtStart;
                    LineCurve line1 = new LineCurve(po1, po2);
                    double[] param = line1.DivideByCount(2, false);

                    double param1 = param[0];
                    double param2 = param[0];
                    double param3 = param[0];

                    Curve curve1 = line1;



                    while (true)
                    {

                        Circle outCircle = Circle.TryFitCircleTTT(curve1, curve2, curve3, param1, param2, param3);

                        
                        //circle normal to surface

                        Point3d outCircleCenter = outCircle.Center;
                        double outCircleRadius = outCircle.Radius;
                        double u, v;
                        surface.ClosestPoint(outCircleCenter, out u, out v);
                        var direction = surface.NormalAt(u, v);
                        Point3d surfCenter = surface.PointAt(u, v);
                        Plane pl1 = new Plane(surfCenter, direction);
                        Circle circle = new Circle(pl1, surfCenter, outCircleRadius - offSetStone);
                        Circle circleDist = new Circle(pl1, surfCenter, outCircleRadius + stoneDist);
                        Guid cir_guid = doc.Objects.AddCircle(circle);
                        cir_guid_list.Add(cir_guid);


                        //Cut tween curve at latest circle center
                        Point3d pointOnCurve;
                        Point3d pointOnCircle;
                        Curve circleDistCurve = circleDist.ToNurbsCurve();
                        tCurve.Domain = new Interval(0, tCurve.GetLength());
                        Curve[] splitCurves = tCurve.Split(outCircleRadius);
                        if (splitCurves is null) break;
                        tCurve = splitCurves[splitCurves.Length-1];
                        tCurve.ClosestPoints(circleDistCurve, out pointOnCurve, out pointOnCircle);

                        //Cut tween curve at latest circle border
                        double curveSplitParam;
                        tCurve.Domain = new Interval(0, tCurve.GetLength());
                        tCurve.ClosestPoint(pointOnCurve, out curveSplitParam);
                        splitCurves = tCurve.Split(curveSplitParam);
                        if (splitCurves is null) break;
                        tCurve = splitCurves[splitCurves.Length - 1];

                        //New parameter at curve1
                        double circleParam;
                        circleDistCurve.ClosestPoint(pointOnCircle, out circleParam);
                        param1 = circleParam;
                        curve1 = circleDistCurve;

                        //New parameter at curves[0]
                        double paramCurve0New;
                        curve2.ClosestPoint(pointOnCircle, out paramCurve0New);
                        Point3d pointCurve0New = curve2.PointAt(paramCurve0New);
                        double distNewPoints0 = pointOnCircle.DistanceTo(pointCurve0New);
                        param2 = paramCurve0New + distNewPoints0;

                        //New parameter at curves[1]
                        double paramCurve1New;
                        curve3.ClosestPoint(pointOnCircle, out paramCurve1New);
                        Point3d pointCurve1New = curve3.PointAt(paramCurve1New);
                        double distNewPoints1 = pointOnCircle.DistanceTo(pointCurve1New);
                        param3 = paramCurve1New + distNewPoints1;
                    }

                    doc.Views.Redraw();


                    //Options
                    var go = new GetOption();
                    go.SetCommandPrompt("Set options.");


                    var stoneOff = new Rhino.Input.Custom.OptionDouble(offSetStone);
                    var distStone = new Rhino.Input.Custom.OptionDouble(stoneDist);
                    var boolOption = new Rhino.Input.Custom.OptionToggle(false, "Off", "On");
                    

                    go.AddOptionDouble("Offset", ref stoneOff);
                    go.AddOptionDouble("Distance", ref distStone);
                    go.AddOptionToggle("Reverse", ref boolOption);

                    go.AcceptNothing(true);

                    var res = go.Get();

                    if (res == GetResult.Nothing) break;
                    if (res == GetResult.Cancel) break;

                    foreach (var gui in cir_guid_list)
                    {
                        var gu = doc.Objects.Find(gui);
                        doc.Objects.Delete(gu);
                    }



                    offSetStone = stoneOff.CurrentValue;
                    stoneDist = distStone.CurrentValue;
                    optionBool = boolOption.CurrentValue;

                    if (optionBool == true)
                    {
                        curve2.Reverse();
                        curve2 = curve2.Rebuild(60, 3, false);
                        curve3.Reverse();
                        curve3 = curve3.Rebuild(60, 3, false);
                        optionBool = false;
                    }


                }



            }

            else
            {
                while (true)
                {


                    cir_guid_list = new List<Guid>();
                    List<Point3d> points = new List<Point3d>();


                    double length = (diamStone / 2) + offSetStone;
                    double crv_length = curve.GetLength();

                    Point3d point = curve.PointAtLength(length);
                    points.Add(point);

                    while (true)
                    {
                        length += diamStone + offSetStone;
                        if (length > crv_length) break;
                        point = curve.PointAtLength(length);
                        points.Add(point);
                    }

                    foreach (var poi in points)
                    {
                        double u, v;
                        surface.ClosestPoint(poi, out u, out v);
                        var direction = surface.NormalAt(u, v);
                        double x = direction.X;
                        double y = direction.Y;
                        double z = direction.Z;
                        Vector3d vt1 = new Vector3d(x, y, z);
                        Plane pl1 = new Plane(poi, vt1);
                        Circle circle = new Circle(pl1, poi, diamStone / 2);
                        Guid cir_guid = doc.Objects.AddCircle(circle);
                        cir_guid_list.Add(cir_guid);

                    }

                    doc.Views.Redraw();

                    //Options
                    var go = new GetOption();
                    go.SetCommandPrompt("Set options.");

                    var stoneDiam = new Rhino.Input.Custom.OptionDouble(diamStone);
                    var stoneOff = new Rhino.Input.Custom.OptionDouble(offSetStone);
                    var boolOption = new Rhino.Input.Custom.OptionToggle(false, "Off", "On");

                    go.AddOptionDouble("StoneDiam", ref stoneDiam);
                    go.AddOptionDouble("Offset", ref stoneOff);
                    go.AddOptionToggle("Reverse", ref boolOption);

                    go.AcceptNothing(true);

                    var res = go.Get();

                    if (res == GetResult.Nothing) break;
                    if (res == GetResult.Cancel) break;

                    foreach (var gui in cir_guid_list)
                    {
                        var gu = doc.Objects.Find(gui);
                        doc.Objects.Delete(gu);
                    }


                    diamStone = stoneDiam.CurrentValue;
                    offSetStone = stoneOff.CurrentValue;
                    optionBool = boolOption.CurrentValue;

                    if (optionBool == true)
                        curve.Reverse();

                }
            }


            doc.Views.Redraw();

            doc.Groups.Add(cir_guid_list);

            return Result.Success;
        }
    }
}
