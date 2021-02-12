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
                curves.Add(curve);
            }

            

            //Pave options
            List<Guid> cir_guid_list = new List<Guid>();

            if (curves.Count > 1)
            {
                Point3d po1 = curves[0].PointAtStart;
                Point3d po2 = curves[1].PointAtStart;
                LineCurve line1 = new LineCurve(po1, po2);

                double[] param1 = line1.DivideByCount(2, false);


                Circle outCircle = Circle.TryFitCircleTTT(line1, curves[0], curves[1], param1[0], param1[0], param1[0]);

                doc.Objects.AddCircle(outCircle);

                /*--
                var tweenCurves = Curve.CreateTweenCurvesWithSampling(curves[0], curves[1], 1, 30, tolerance);
                curve = tweenCurves[0];
                Point3d po1 = curve.PointAtStart;
                Point3d po2 = curves[0].PointAtStart;
                double radius = po1.DistanceTo(po2);

                double crv_length = curve.GetLength();
                double length = radius + offSetStone;
                Point3d point = curve.PointAtLength(length);
                points.Add(point);

                while (true)
                {

                    if (length > crv_length) break;


                    double u, v;
                    surface.ClosestPoint(point, out u, out v);
                    var direction = surface.NormalAt(u, v);
                    double x = direction.X;
                    double y = direction.Y;
                    double z = direction.Z;
                    Vector3d vt1 = new Vector3d(x, y, z);
                    Plane pl1 = new Plane(point, vt1);
                    Circle circle = new Circle(pl1, point, radius);
                    Guid cir_guid = doc.Objects.AddCircle(circle);
                    cir_guid_list.Add(cir_guid);

                    length += (2*radius + offSetStone);
                    double length2;
                    point = curve.PointAtLength(length);
                    curves[0].ClosestPoint(point, out length2);
                    po2 = curves[0].PointAt(length2);
                    radius = point.DistanceTo(po2);


                }
                --*/


            }

            else
            {
                while (true)
                {

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
                }
            }


            doc.Views.Redraw();

            doc.Groups.Add(cir_guid_list);

            return Result.Success;
        }
    }
}
