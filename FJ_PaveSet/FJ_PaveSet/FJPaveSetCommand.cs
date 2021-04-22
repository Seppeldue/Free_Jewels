using System;
using System.Collections.Generic;
using System.Threading;
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

        // Hotkeys

        private const int VK_W = 0x57;
        private const int VK_S = 0x53;
        private const int VK_A = 0x41;
        private const int VK_D = 0x44;

        private bool w_key_pressed = false;
        private bool s_key_pressed = false;
        private bool a_key_pressed = false;
        private bool d_key_pressed = false;

        void OnRhinoKeyboardEvent(int key)
        {
            if (key == VK_W)
            {
                w_key_pressed = true;
                diamStone = diamStone + 0.05;
            }
            else if (key == VK_S)
            {
                s_key_pressed = true;
                diamStone = diamStone - 0.05;
            }
            else if (key == VK_A)
            {
                a_key_pressed = true;
                offSetStone = offSetStone - 0.025;
            }
            else if (key == VK_D)
            {
                d_key_pressed = true;
                offSetStone = offSetStone + 0.025;
            }
        }

        // Dynamic Draw
        void RefCircleDraw(object sender, Rhino.Input.Custom.GetPointDrawEventArgs e)
        {

            Point3d closestPoint;
            ComponentIndex compIndex;
            double u, v;
            Vector3d vtCurr;
            brepSurf.ClosestPoint(e.CurrentPoint,out closestPoint,out compIndex, out u, out v, 0.01,out vtCurr);
            Plane planeCircle = new Plane(e.CurrentPoint, vtCurr);
            Point3d centerCircleOff = e.CurrentPoint + vtCurr * diamStone / 2;
            Circle dynCircle = new Circle(planeCircle, e.CurrentPoint, diamStone / 2);
            e.Display.DrawCircle(dynCircle, System.Drawing.Color.Blue);
            Circle crOff1 = new Circle(planeCircle, e.CurrentPoint, (diamStone / 2) + offSetStone);
            e.Display.DrawCircle(crOff1, System.Drawing.Color.Red);
            Line dynLineNormal = new Line(e.CurrentPoint, centerCircleOff);
            e.Display.DrawLine(dynLineNormal, System.Drawing.Color.Blue);
            e.Display.Draw3dText(diamStone.ToString("F2"), System.Drawing.Color.Black, planeCircle, diamStone / 4, "Arial", false, false, Rhino.DocObjects.TextHorizontalAlignment.Center, Rhino.DocObjects.TextVerticalAlignment.Middle);
        }

        // Globals
        double diamStone = 1.0;
        double offSetStone = 0.1;
        bool optionBool = false;
        bool moveBool = false;
        Brep brepSurf;


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            Mesh meshSurf;
            List<Guid> ids = new List<Guid>();
            double tolerance = doc.ModelAbsoluteTolerance;
            const Rhino.DocObjects.ObjectType geometryFilter = Rhino.DocObjects.ObjectType.Surface |
                                                       Rhino.DocObjects.ObjectType.PolysrfFilter |
                                                       Rhino.DocObjects.ObjectType.Mesh;

            GetObject gs = new Rhino.Input.Custom.GetObject();
            gs.SetCommandPrompt("Surface to orient on");
            gs.GeometryFilter = geometryFilter;
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

            brepSurf = objref.Brep();
            if (brepSurf == null)
            {
                meshSurf = objref.Mesh();
                brepSurf = Brep.CreateFromMesh(meshSurf, true);
            }
            if (brepSurf == null)
                return Result.Failure;
            obj.Select(false);

            
            while (true)
            {
                w_key_pressed = false;
                s_key_pressed = false;
                a_key_pressed = false;
                d_key_pressed = false;

                m_escape_key_pressed = false;

                RhinoApp.EscapeKeyPressed += RhinoApp_EscapeKeyPressed;
                RhinoApp.KeyboardEvent += OnRhinoKeyboardEvent;

                Point3d pt0;
                GetPoint getPointAction = new GetPoint();
                getPointAction.SetCommandPrompt("Please select insert point(s) on surface.");
                getPointAction.Constrain(brepSurf, -1,-1, false);

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
                getPointAction.AcceptString(false);
                getPointAction.AcceptNothing(true);
                var res = getPointAction.Get();

                
                if (w_key_pressed || s_key_pressed)
                {
                    RhinoApp.KeyboardEvent -= OnRhinoKeyboardEvent;
                    w_key_pressed = false;
                    s_key_pressed = false;
                    stoneDiam.CurrentValue = diamStone;
                }
                if (a_key_pressed || d_key_pressed)
                {
                    RhinoApp.KeyboardEvent -= OnRhinoKeyboardEvent;
                    a_key_pressed = false;
                    d_key_pressed = false;
                    stoneOff.CurrentValue = offSetStone;
                }

                if (res == GetResult.Nothing) break;
                if (m_escape_key_pressed) break;

                diamStone = stoneDiam.CurrentValue;
                offSetStone = stoneOff.CurrentValue;
                optionBool = boolOption.CurrentValue;
                moveBool = moveOption.CurrentValue;

                RhinoApp.KeyboardEvent -= OnRhinoKeyboardEvent;

                if (moveBool == true)
                {
                    RhinoApp.KeyboardEvent -= OnRhinoKeyboardEvent;
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
                        Point3d closestPoint;
                        ComponentIndex compIndex;
                        double u, v;
                        Vector3d vt1;

                        brepSurf.ClosestPoint(pt0, out closestPoint, out compIndex, out u, out v, 0.01, out vt1);
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
                    RhinoApp.KeyboardEvent -= OnRhinoKeyboardEvent;
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
                    Point3d closestPoint;
                    ComponentIndex compIndex;
                    double u, v;
                    Vector3d vt1;
                    brepSurf.ClosestPoint(pt0, out closestPoint, out compIndex, out u, out v, 0.01, out vt1);
                    Plane pl1 = new Plane(pt0, vt1);
                    Circle cr1 = new Circle(pl1, pt0, diamStone / 2);
                    var crgu = doc.Objects.AddCircle(cr1);
                    ids.Add(crgu);
                    doc.Views.Redraw();
                }

            }
            RhinoApp.KeyboardEvent -= OnRhinoKeyboardEvent;
            RhinoApp.EscapeKeyPressed -= RhinoApp_EscapeKeyPressed;
            doc.Groups.Add(ids);
            return Result.Success;
        }
    }
}