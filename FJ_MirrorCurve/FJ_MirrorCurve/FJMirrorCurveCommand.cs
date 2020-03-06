using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_MirrorCurve
{
    public class FJMirrorCurveCommand : Command
    {
        public FJMirrorCurveCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJMirrorCurveCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_MirrorCurve"; }
        }

        // ESC Key Break
        void RhinoApp_EscapeKeyPressed(object sender, EventArgs e)
        {
            m_escape_key_pressed = true;
        }
        private bool m_escape_key_pressed = false;

        // Dynamic Draw
        void DynDrawCurve(object sender, Rhino.Input.Custom.GetPointDrawEventArgs e)
        {
            int curve_degree = points.Count;
            if (points.Count > 3)
                curve_degree = 3;
            List<Point3d> pointsDyn = new List<Point3d>(points);
            Point3d pt0 = e.CurrentPoint;
            Point3d pt0M = new Point3d(xm * pt0.X, ym * pt0.Y, pt0.Z);
            int insert = points.Count / 2;
            pointsDyn.Insert(insert, pt0);
            pointsDyn.Insert(insert + 1, pt0M);
            NurbsCurve dnc = NurbsCurve.Create(clamped, curve_degree, pointsDyn);
            e.Display.DrawCurve(dnc, System.Drawing.Color.DarkRed);
        }

        //Global
        List<Point3d> points = new List<Point3d>();
        bool clamped;
        bool front;
        double xm = -1;
        double ym = 1;

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            points.Clear();

            while (true)
            {
                m_escape_key_pressed = false;
                RhinoApp.EscapeKeyPressed += RhinoApp_EscapeKeyPressed;

                bool undo = false;
                Point3d pt0;
                Point3d pt0M;
                int insert;

                GetPoint getPoint = new GetPoint();
                getPoint.SetCommandPrompt("Please select point");

                var boolOptionAxis = new Rhino.Input.Custom.OptionToggle(front, "Front", "Side");
                getPoint.AddOptionToggle("MirrorPlane", ref boolOptionAxis);
                var boolOption = new Rhino.Input.Custom.OptionToggle(clamped, "Off", "On");
                getPoint.AddOptionToggle("Closed", ref boolOption);
                var undoOption = new Rhino.Input.Custom.OptionToggle(undo, "No", "Yes");
                getPoint.AddOptionToggle("Undo", ref undoOption);

                if (points.Count > 1)
                    getPoint.DynamicDraw += DynDrawCurve;

                getPoint.AcceptNothing(true);
                GetResult gr = getPoint.Get();
                if (gr == GetResult.Nothing) break;
                else if (gr == GetResult.Option)
                {
                    clamped = boolOption.CurrentValue;
                    front = boolOptionAxis.CurrentValue;
                    undo = undoOption.CurrentValue;
                    if (front == false)
                    {
                        xm = -1;
                        ym = 1;
                    }
                    else if (front)
                    {
                        xm = 1;
                        ym = -1;
                    }
                    if (undo)
                    {
                        points.RemoveAt(points.Count / 2);
                        points.RemoveAt(points.Count / 2);
                    }
                }
                else if (gr == GetResult.Point)
                {
                    pt0 = getPoint.Point();
                    pt0M = new Point3d(xm * pt0.X, ym * pt0.Y, pt0.Z);
                    insert = points.Count / 2;
                    points.Insert(insert, pt0);
                    points.Insert(insert + 1, pt0M);
                }

                if (m_escape_key_pressed) break;


            }

            NurbsCurve nc = NurbsCurve.Create(clamped, 3, points);

            RhinoApp.EscapeKeyPressed -= RhinoApp_EscapeKeyPressed;
            doc.Objects.AddCurve(nc);
            doc.Views.Redraw();

            return Result.Success;
        }
    }
}
