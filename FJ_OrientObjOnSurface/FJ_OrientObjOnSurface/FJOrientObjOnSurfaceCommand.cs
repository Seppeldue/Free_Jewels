using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_OrientObjOnSurface
{
    public class FJOrientObjOnSurfaceCommand : Command
    {
        public FJOrientObjOnSurfaceCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJOrientObjOnSurfaceCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_OrientObjOnSurface"; }
        }
        // Dynamic Draw
        void RefObjDraw(object sender, Rhino.Input.Custom.GetPointDrawEventArgs e)
        {
            Rhino.DocObjects.RhinoObject rhobj = e.Source.Tag as Rhino.DocObjects.RhinoObject;
            if (rhobj == null)
                return;

            double u, v;
            var currentPoint = e.CurrentPoint;
            Vector3d direction = new Vector3d();
            Plane currentPlane = new Plane();
            if (surface != null)
            {
                surface.ClosestPoint(currentPoint, out u, out v);
                direction = surface.NormalAt(u, v);
                currentPlane = new Plane(currentPoint, direction);
                Transform xform = Rhino.Geometry.Transform.PlaneToPlane(sourcePlane, currentPlane);
                e.Display.DrawObject(rhobj, xform);
            }
            else if (surface == null)
            {
                surface2.ClosestPoint(currentPoint, out u, out v);
                direction = surface2.NormalAt(u, v);
                currentPlane = new Plane(currentPoint, direction);
                Transform xform = Rhino.Geometry.Transform.PlaneToPlane(instancePlane, currentPlane);
                e.Display.DrawObject(rhobj, xform);
            }


        }

        Surface surface;
        Surface surface2;
        Plane sourcePlane;

        Point3d instancePoint;
        Plane instancePlane;

        bool copyBol = false;

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            //pick surface to orient to or block instance to relocate
            GetObject gs = new Rhino.Input.Custom.GetObject();
            gs.SetCommandPrompt("Surface to orient new object on or BlockInstance to move");
            gs.GeometryFilter = Rhino.DocObjects.ObjectType.Surface | Rhino.DocObjects.ObjectType.InstanceReference;
            gs.SubObjectSelect = true;
            gs.DeselectAllBeforePostSelect = false;
            gs.OneByOnePostSelect = true;
            gs.Get();
            if (gs.CommandResult() != Result.Success)
                return gs.CommandResult();

            Rhino.DocObjects.ObjRef objref = gs.Object(0);
            Rhino.DocObjects.RhinoObject obj = objref.Object();
            if (obj == null)
                return Result.Failure;
            surface = objref.Surface();
            

            //relocate block instance
            if (surface == null)
            {
                Rhino.DocObjects.InstanceObject instance1 = objref.Object() as Rhino.DocObjects.InstanceObject;

                instancePoint = instance1.InsertionPoint;
                double g, h;
                surface2.ClosestPoint(instancePoint, out g, out h);
                var instanceDirection = surface2.NormalAt(g, h);
                instancePlane = new Plane(instancePoint, instanceDirection);

                Rhino.Input.Custom.GetPoint gpss = new Rhino.Input.Custom.GetPoint();
                gpss.SetCommandPrompt("Point on surface to orient to");
                gpss.Constrain(surface2, false);

                gpss.DynamicDraw += RefObjDraw;
                gpss.Tag = instance1;

                gpss.Get();
                if (gpss.CommandResult() != Rhino.Commands.Result.Success)
                    return gpss.CommandResult();
                Point3d ptss = gpss.Point();
                surface2.ClosestPoint(ptss, out g, out h);
                var direction1 = surface2.NormalAt(g, h);
                Plane pl11 = new Plane(ptss, direction1);
                Transform iform = Rhino.Geometry.Transform.PlaneToPlane(instancePlane, pl11);
                doc.Objects.Transform(instance1, iform, true);

                return Result.Success;
            }

            obj.Select(false);

            //pick objekt to orient
            var copy = new Rhino.Input.Custom.OptionToggle(false, "No", "Yes");

            GetObject go = new GetObject();
            go.SetCommandPrompt("Select object to orient.");
            go.AddOptionToggle("Copy", ref copy);
            go.SubObjectSelect = true;
            go.DeselectAllBeforePostSelect = false;
            go.GroupSelect = true;

            for (; ; )
            {
                var res = go.GetMultiple(1, -1);
                if (gs.CommandResult() != Result.Success)
                    return gs.CommandResult();
                if (res == GetResult.Option)
                {
                    copyBol = copy.CurrentValue;
                    continue;
                }
                if (gs.CommandResult() != Result.Success)
                    return gs.CommandResult();

                break;
            }

            

            int obCount = go.ObjectCount;
            string instDefCount = DateTime.Now.ToString("ddMMyyyyHHmmss");

            //create block instance and plane for instance
            Rhino.Input.Custom.GetPoint gp = new Rhino.Input.Custom.GetPoint();
            gp.SetCommandPrompt("Point to orient from");
            gp.Get();
            if (gp.CommandResult() != Rhino.Commands.Result.Success)
                return gp.CommandResult();

            Vector3d vt1 = new Vector3d(0, 0, 1);
            Point3d pt1 = gp.Point();

            sourcePlane = new Plane(pt1, vt1);
            Plane originPlane = new Plane(Point3d.Origin, vt1);
            Transform bform = Rhino.Geometry.Transform.PlaneToPlane(sourcePlane, originPlane);
            //block instance
            GeometryBase[] obj1List = new GeometryBase[obCount];
            List<Brep> opList = new List<Brep>();

            for (int igo = 0; igo < obCount; igo++)
            {
                Rhino.DocObjects.ObjRef objref1 = go.Object(igo);
                Rhino.DocObjects.RhinoObject obj1 = objref1.Object();
                if (obj == null)
                    return Result.Failure;
                obj.Select(false);


                Rhino.Geometry.Brep opItem = objref1.Brep();
                opList.Add(opItem);

                GeometryBase obj1Base = obj1.Geometry;
                obj1Base.Transform(bform);

                obj1List[igo] = obj1Base;
            }

            var orientBlock = doc.InstanceDefinitions.Add("Block" + instDefCount, "OrientBlock", Point3d.Origin, obj1List);

            //get all go.Objects to .Tag 
            Brep[] op = new Brep[obCount];
            op = Brep.CreateBooleanUnion(opList, 0.01);
            Brep od = new Brep();
            od = op[0];
            var odGuid = doc.Objects.AddBrep(od);
            Rhino.DocObjects.ObjRef objref2 = new Rhino.DocObjects.ObjRef(odGuid);
            Rhino.DocObjects.RhinoObject objDrw = objref2.Object();

            //orient plane to surface
            if (copyBol)
            {
                while (true)
                {
                    Rhino.Input.Custom.GetPoint gps = new Rhino.Input.Custom.GetPoint();
                    gps.SetCommandPrompt("Point on surface to orient to. Press enter when done.");
                    gps.Constrain(surface, false);
                    gps.AcceptNothing(true);
                    gps.DynamicDraw += RefObjDraw;
                    gps.Tag = objDrw;

                    var res = gps.Get();

                    if (res == GetResult.Nothing) break;
                    //else if (gps.CommandResult() != Rhino.Commands.Result.Success)
                    //    return gps.CommandResult();
                    

                    Point3d pts = gps.Point();
                    double u, v;
                    surface.ClosestPoint(pts, out u, out v);
                    Vector3d direction = surface.NormalAt(u, v);
                    Plane pl1 = new Plane(pts, direction);

                    Rhino.Geometry.Transform xform = Rhino.Geometry.Transform.PlaneToPlane(originPlane, pl1);

                    doc.Objects.AddInstanceObject(orientBlock, xform);

                    doc.Objects.Delete(objDrw);

                }
                copyBol = false;
            }
            else
            {
                Rhino.Input.Custom.GetPoint gps = new Rhino.Input.Custom.GetPoint();
                gps.SetCommandPrompt("Point on surface to orient to");
                gps.Constrain(surface, false);

                gps.DynamicDraw += RefObjDraw;
                gps.Tag = objDrw;

                gps.Get();
                if (gps.CommandResult() != Rhino.Commands.Result.Success)
                    return gps.CommandResult();
                Point3d pts = gps.Point();
                double u, v;
                surface.ClosestPoint(pts, out u, out v);
                Vector3d direction = surface.NormalAt(u, v);
                Plane pl1 = new Plane(pts, direction);

                Rhino.Geometry.Transform xform = Rhino.Geometry.Transform.PlaneToPlane(originPlane, pl1);

                doc.Objects.AddInstanceObject(orientBlock, xform);

                doc.Objects.Delete(objDrw);

            }

            surface2 = surface;

            return Result.Success;
        }
    }
}
