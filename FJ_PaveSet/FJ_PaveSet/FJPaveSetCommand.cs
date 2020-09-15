using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_PaveSet
{
    public class FJPaveSetCommand : Command
    {
        public FJPaveSetCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJPaveSetCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_PaveSet"; }
        }

        // ESC Key Break
        void RhinoApp_EscapeKeyPressed(object sender, EventArgs e)
        {
            m_escape_key_pressed = true;
        }

        private bool m_escape_key_pressed = false;

        // Dynamic Draw
        void RefCircleDraw(object sender, Rhino.Input.Custom.GetPointDrawEventArgs e)
        {
            Rhino.DocObjects.RhinoObject rhobj = e.Source.Tag as Rhino.DocObjects.RhinoObject;
            if (rhobj == null)
                return;

            var centerCircle = e.CurrentPoint;
            double u, v;
            var currentPoint = e.CurrentPoint;

            surface.ClosestPoint(currentPoint, out u, out v);
            var direction = surface.NormalAt(u, v);

            double x = direction.X;
            double y = direction.Y;
            double z = direction.Z;
            Vector3d vtCurr = new Vector3d(x, y, z);
            Plane planeCircle = new Plane(currentPoint, vtCurr);
            Point3d centerCircleOff = currentPoint + vtCurr * diamStone / 2;


            Circle dynCircle = new Circle(planeCircle, currentPoint, diamStone / 2);
            e.Display.DrawCircle(dynCircle, System.Drawing.Color.Blue);
            Circle crOff1 = new Circle(planeCircle, currentPoint, (diamStone / 2) + offSetStone);
            e.Display.DrawCircle(crOff1, System.Drawing.Color.Red);
            Line dynLineNormal = new Line(currentPoint, centerCircleOff);
            e.Display.DrawLine(dynLineNormal, System.Drawing.Color.Blue);
        }

        double diamStone = 1.0;
        double offSetStone = 0.1;
        bool optionBool = false;
        bool moveBool = false;
        Surface surface;


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            List<Guid> ids = new List<Guid>();
            double tolerance = doc.ModelAbsoluteTolerance;

            GetObject gs = new Rhino.Input.Custom.GetObject();
            gs.SetCommandPrompt("Surface to orient on");
            gs.GeometryFilter = Rhino.DocObjects.ObjectType.Surface;
            gs.SubObjectSelect = true;
            gs.DeselectAllBeforePostSelect = true;
            gs.OneByOnePostSelect = true;
            gs.Get();
            if (gs.CommandResult() != Result.Success)
                return gs.CommandResult();

            Rhino.DocObjects.ObjRef objref = gs.Object(0);
            Rhino.DocObjects.RhinoObject obj = objref.Object();
            if (obj == null)
                return Result.Failure;
            surface = objref.Surface();

            if (surface == null)
                return Result.Failure;
            obj.Select(false);


            while (true)
            {
                m_escape_key_pressed = false;
                RhinoApp.EscapeKeyPressed += RhinoApp_EscapeKeyPressed;

                Point3d pt0;
                GetPoint getPointAction = new GetPoint();
                getPointAction.SetCommandPrompt("Please select insert point(s) on surface.");
                getPointAction.Constrain(surface, false);

                var stoneDiam = new Rhino.Input.Custom.OptionDouble(diamStone);
                var stoneOff = new Rhino.Input.Custom.OptionDouble(offSetStone);
                var boolOption = new Rhino.Input.Custom.OptionToggle(false, "Off", "On");
                var moveOption = new Rhino.Input.Custom.OptionToggle(false, "Off", "On");

                getPointAction.AddOptionDouble("StoneDiam", ref stoneDiam);
                getPointAction.AddOptionDouble("Offset", ref stoneOff);
                getPointAction.AddOptionToggle("Delete", ref boolOption);
                getPointAction.AddOptionToggle("Move", ref moveOption);
                getPointAction.DynamicDraw += RefCircleDraw;
                getPointAction.Tag = obj;
                getPointAction.AcceptString(true);
                getPointAction.AcceptNothing(true);
                var res = getPointAction.Get();

                if (res == GetResult.Nothing) break;
                if (m_escape_key_pressed) break;

                diamStone = stoneDiam.CurrentValue;
                offSetStone = stoneOff.CurrentValue;
                optionBool = boolOption.CurrentValue;
                moveBool = moveOption.CurrentValue;

                if (moveBool == true)
                {
                    while (true)
                    {
                        GetObject gcd = new Rhino.Input.Custom.GetObject();
                        gcd.SetCommandPrompt("Select circle(s) to move. Press enter when done");
                        gcd.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
                        gcd.SubObjectSelect = false;
                        gcd.DeselectAllBeforePostSelect = true;
                        gcd.AcceptNothing(true);
                        if (gcd.Get() == GetResult.Nothing) break;
                        gcd.Get();
                        Rhino.DocObjects.ObjRef delobjref = gcd.Object(0);
                        Rhino.DocObjects.RhinoObject delobj = delobjref.Object();
                        Curve curveRadius = delobjref.Curve();
                        Circle circleRadius = new Circle();
                        curveRadius.TryGetCircle(out circleRadius, tolerance);
                        diamStone = circleRadius.Diameter;
                        doc.Objects.Delete(delobj, true);
                        doc.Views.Redraw();

                        getPointAction.Get();
                        pt0 = getPointAction.Point();
                        double u, v;
                        surface.ClosestPoint(pt0, out u, out v);
                        var direction = surface.NormalAt(u, v);
                        double x = direction.X;
                        double y = direction.Y;
                        double z = direction.Z;
                        Vector3d vt1 = new Vector3d(x, y, z);
                        Plane pl1 = new Plane(pt0, vt1);
                        Circle cr1 = new Circle(pl1, pt0, diamStone / 2);
                        var crgu = doc.Objects.AddCircle(cr1);
                        ids.Add(crgu);
                        doc.Views.Redraw();

                    }
                    moveBool = false;
                }
                if (optionBool == true)
                {
                    while (true)
                    {
                        GetObject gcd = new Rhino.Input.Custom.GetObject();
                        gcd.SetCommandPrompt("Select circle(s) to delete. Press enter when done");
                        gcd.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
                        gcd.SubObjectSelect = false;
                        gcd.DeselectAllBeforePostSelect = true;
                        gcd.AcceptNothing(true);
                        if (gcd.Get() == GetResult.Nothing) break;
                        gcd.Get();
                        Rhino.DocObjects.ObjRef delobjref = gcd.Object(0);
                        Rhino.DocObjects.RhinoObject delobj = delobjref.Object();
                        Curve curveRadius = delobjref.Curve();
                        Circle circleRadius = new Circle();
                        curveRadius.TryGetCircle(out circleRadius, tolerance);
                        diamStone = circleRadius.Diameter;
                        doc.Objects.Delete(delobj, true);
                        doc.Views.Redraw();
                    }
                    optionBool = false;
                }
                if (res == GetResult.Point)
                {
                    pt0 = getPointAction.Point();
                    double u, v;
                    surface.ClosestPoint(pt0, out u, out v);
                    var direction = surface.NormalAt(u, v);
                    double x = direction.X;
                    double y = direction.Y;
                    double z = direction.Z;
                    Vector3d vt1 = new Vector3d(x, y, z);
                    Plane pl1 = new Plane(pt0, vt1);
                    Circle cr1 = new Circle(pl1, pt0, diamStone / 2);
                    var crgu = doc.Objects.AddCircle(cr1);
                    ids.Add(crgu);
                    doc.Views.Redraw();
                }
            }
            RhinoApp.EscapeKeyPressed -= RhinoApp_EscapeKeyPressed;
            doc.Groups.Add(ids);
            return Result.Success;
        }
    }
}