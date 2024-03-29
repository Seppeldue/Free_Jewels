﻿using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_Expand
{
    public class FJExpandCommand : Command
    {
        public FJExpandCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJExpandCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_Expand"; }
        }

        //Dynamic Object Draw
        void RefObjDraw(object sender, Rhino.Input.Custom.GetPointDrawEventArgs e)
        {
            double factorTemp = (scaleCenter.DistanceTo(e.CurrentPoint)) / (scaleCenter.DistanceTo(scaleRefPoint));

            if (dimensionIndex == 0)
            {
                for (int i = 0; i < goList.Count; i++)
                {
                    Rhino.Geometry.Vector3d vec = (centers[i] - scaleCenter) * (factorTemp - 1);
                    Rhino.DocObjects.RhinoObject rhobj = goList[i] as Rhino.DocObjects.RhinoObject;
                    var xform = Rhino.Geometry.Transform.Translation(vec);
                    e.Display.DrawObject(rhobj, xform);
                }
            }
            //Translate 2d
            else if (dimensionIndex == 1)
            {
                for (int i = 0; i < goList.Count; i++)
                {
                    Rhino.Geometry.Vector3d vec = (centers[i] - scaleCenter) * (factorTemp - 1);
                    Rhino.DocObjects.RhinoObject rhobj = goList[i] as Rhino.DocObjects.RhinoObject;
                    Vector3d planeNormal = plane2D.Normal;
                    if (planeNormal[0] != 0)
                        vec.X = 0;
                    else if (planeNormal[1] != 0)
                        vec.Y = 0;
                    else if (planeNormal[2] != 0)
                        vec.Z = 0;
                    var xform = Rhino.Geometry.Transform.Translation(vec);
                    e.Display.DrawObject(rhobj, xform);
                }
            }
            //Translate 1d
            else if (dimensionIndex == 2)
            {
                for (int i = 0; i < goList.Count; i++)
                {
                    Vector3d vecEach = (centers[i] - scaleCenter);
                    double vecEachLength = vecEach.Length * (factorTemp - 1);
                    Vector3d vec = (scaleRefPoint - scaleCenter);
                    double angleVec = Vector3d.VectorAngle(vecEach, vec);
                    int pol = 1;
                    if (angleVec > 1.57) pol = -1;
                    vec.Unitize();
                    vec = vec * vecEachLength * pol;
                    var xform = Transform.Translation(vec);
                    Rhino.DocObjects.RhinoObject rhobj = goList[i] as Rhino.DocObjects.RhinoObject;
                    e.Display.DrawObject(rhobj, xform);
                }
            }
        }

        //Globals
        List<Point3d> centers = new List<Point3d>();
        Point3d scaleCenter = new Point3d();
        Point3d scaleRefPoint = new Point3d();
        double factor = 1;
        List<Object> goList = new List<Object>();
        string[] dimensions = new string[] { "3D", "2D", "1D" };
        int dimensionIndex = 0;
        Plane plane2D = new Plane();

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {


            //pick objects to expand
            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
            go.SetCommandPrompt("Select objects to expand");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.GetMultiple(1, 0);

            //Compute center
            Point3d centersAdd = new Point3d();
            for (int i = 0; i < go.ObjectCount; i++)
            {
                BoundingBox bbObj = go.Object(i).Geometry().GetBoundingBox(true);
                Point3d bbObjCenter = bbObj.Center;
                centers.Add(bbObjCenter);
                centersAdd += bbObjCenter;

                Rhino.DocObjects.ObjRef objref = go.Object(i);
                // get selected surface object
                Rhino.DocObjects.RhinoObject obj = objref.Object();
                goList.Add(obj);
            }
            Point3d allCenter = centersAdd / centers.Count;

            //pick center 
            GetPoint gp = new GetPoint();
            gp.SetCommandPrompt("Basepoint. Press Enter for automatic.");
            gp.AcceptNothing(true);
            var resgp = gp.Get();
            if (resgp == GetResult.Nothing)
                scaleCenter = allCenter;
            else
                scaleCenter = gp.Point();

            //Expansion factor
            GetPoint gp2 = new GetPoint();
            gp2.SetCommandPrompt("Expansion factor or first reference point <" + factor + ">");
            gp2.DrawLineFromPoint(scaleCenter, true);
            gp2.AcceptNumber(true, true);
            GetResult gr = gp2.Get();

            Rhino.Display.RhinoView view = gp2.View();
            plane2D = view.ActiveViewport.ConstructionPlane();

            if (gr == GetResult.Number)
                factor = gp2.Number();
            if (gr == GetResult.Point)
            {
                scaleRefPoint = gp2.Point();
                Line line1 = new Line(scaleCenter, scaleRefPoint);
                Guid tempLine = doc.Objects.AddLine(line1);
                Guid tempPoint = doc.Objects.AddPoint(scaleRefPoint);
                if (scaleCenter == gp2.Point())
                    return Rhino.Commands.Result.Cancel;
                GetPoint gp3 = new GetPoint();
                gp3.SetCommandPrompt("Secondt reference point");
                gp3.AddOptionList("ScaleDimension", dimensions, 0);
                gp3.DrawLineFromPoint(scaleCenter, true);
                gp3.DynamicDraw += RefObjDraw;

                while (true)
                {
                    GetResult res = gp3.Get();
                    if (res == GetResult.Option)
                    {
                        dimensionIndex = gp3.Option().CurrentListOptionIndex;
                        continue;
                    }
                    else if (res == GetResult.Point)
                    {
                        Point3d scaleRefPoint2 = gp3.Point();
                        factor = (scaleCenter.DistanceTo(scaleRefPoint2)) / (scaleCenter.DistanceTo(scaleRefPoint));
                        doc.Objects.Delete(tempLine, true);
                        doc.Objects.Delete(tempPoint, true);
                    }
                    break;
                }
            }
            RhinoApp.WriteLine("Expantion factor: " + factor);

            //Compute translation

            //Translate 3d
            if (dimensionIndex == 0)
            {
                for (int i = 0; i < go.ObjectCount; i++)
                {
                    Vector3d vec = (centers[i] - scaleCenter) * (factor - 1);
                    var xform = Transform.Translation(vec);
                    doc.Objects.Transform(go.Object(i), xform, true);
                }
            }
            //Translate 2d
            else if (dimensionIndex == 1)
            {
                for (int i = 0; i < go.ObjectCount; i++)
                {
                    Rhino.Geometry.Vector3d vec = (centers[i] - scaleCenter) * (factor - 1);
                    Vector3d planeNormal = plane2D.Normal;
                    if (planeNormal[0] != 0)
                        vec.X = 0;
                    else if (planeNormal[1] != 0)
                        vec.Y = 0;
                    else if (planeNormal[2] != 0)
                        vec.Z = 0;
                    var xform = Rhino.Geometry.Transform.Translation(vec);
                    doc.Objects.Transform(go.Object(i), xform, true);
                }
            }
            //Translate 1d
            else if (dimensionIndex == 2)
            {
                for (int i = 0; i < go.ObjectCount; i++)
                {
                    Vector3d vecEach = (centers[i] - scaleCenter);
                    double vecEachLength = vecEach.Length * (factor - 1);
                    Vector3d vec = (scaleRefPoint - scaleCenter);
                    double angleVec = Vector3d.VectorAngle(vecEach, vec);
                    int pol = 1;
                    if (angleVec > 1.57) pol = -1;
                    vec.Unitize();
                    vec = vec * vecEachLength * pol;

                    var xform = Transform.Translation(vec);
                    doc.Objects.Transform(go.Object(i), xform, true);
                }
            }

            goList = new List<Object>();
            centers = new List<Point3d>();
            dimensionIndex = 0;
            plane2D = new Plane();

            doc.Views.Redraw();
            return Result.Success;
        }
    }
}
