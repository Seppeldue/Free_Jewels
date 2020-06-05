using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_ChainMaker
{
    public class FJChainMakerCommand : Command
    {
        public FJChainMakerCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJChainMakerCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_ChainMaker"; }
        }

        Plane sourcePlane;
        Curve curve;

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            //Pick curve for chain
            GetObject getCurve = new GetObject();
            getCurve.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            getCurve.SetCommandPrompt("Select curve for chain");
            var res = getCurve.Get();
            Rhino.DocObjects.ObjRef objref = getCurve.Object(0);
            Rhino.DocObjects.RhinoObject obj = objref.Object();
            if (obj == null)
                return Result.Failure;
            curve = objref.Curve();
            if (curve == null)
                return Result.Failure;
            obj.Select(false);

            //Pick object for chain (instance)
            //pick objekt to orient
            GetObject go = new GetObject();
            go.SetCommandPrompt("Select object to orient.");
            go.SubObjectSelect = true;
            go.DeselectAllBeforePostSelect = false;
            go.GroupSelect = true;
            go.GetMultiple(1, -1);
            if (go.CommandResult() != Result.Success)
                return go.CommandResult();

            int obCount = go.ObjectCount;
            string instDefCount = DateTime.Now.ToString("ddMMyyyyHHmmss");

            //create block instance and plane for instance
            Rhino.Input.Custom.GetPoint gp1 = new Rhino.Input.Custom.GetPoint();
            gp1.SetCommandPrompt("Center point to orient from");
            gp1.Get();
            if (gp1.CommandResult() != Rhino.Commands.Result.Success)
                return gp1.CommandResult();
            Point3d pt1 = gp1.Point();
            Rhino.Input.Custom.GetPoint gp2 = new Rhino.Input.Custom.GetPoint();
            gp2.SetCommandPrompt("Point for orientation");
            gp2.Get();
            if (gp2.CommandResult() != Rhino.Commands.Result.Success)
                return gp2.CommandResult();
            Point3d pt2 = gp2.Point();

            Vector3d vt1 = pt2 - pt1;

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

            //orient instances along curve

            while (true)
            {
                double chainDis = 1.0;
                double curveLength = curve.GetLength();
                double curveDivide = curveLength / chainDis;
                int curveDivideInt = Convert.ToInt32(curveDivide);




                for (int ic = 0; ic < curveDivideInt; ic++)
                {
                    Point3d insertPoint = curve.PointAtLength(chainDis * ic);
                    Vector3d insertVec = curve.PointAtLength(chainDis * ic+1) - insertPoint;
                    if (ic % 2 == 0)
                    {
                        Plane targetPlane = new Plane(insertPoint, insertVec);
                        Rhino.Geometry.Transform xform = Rhino.Geometry.Transform.PlaneToPlane(originPlane, targetPlane);
                        doc.Objects.AddInstanceObject(orientBlock, xform);

                    }
                    else
                    {
                        Plane targetPlane = new Plane(insertPoint, insertVec);
                        targetPlane.Rotate(Math.PI / 2, insertVec);
                        Rhino.Geometry.Transform xform = Rhino.Geometry.Transform.PlaneToPlane(originPlane, targetPlane);
                        doc.Objects.AddInstanceObject(orientBlock, xform);

                    }
                    
                }





                GetOption gd = new GetOption();
                //gd.SetCommandPrompt("Distance");
                var dis = new Rhino.Input.Custom.OptionDouble(chainDis);
                gd.AddOptionDouble("distance", ref dis , "Distance in mm");
                gd.AcceptNothing(true);

                
                var resdis = gd.Get();
                if (resdis == GetResult.Nothing) break;

                chainDis = dis.CurrentValue;
            }




            doc.Views.Redraw();
            

            // ---

            return Result.Success;
        }
    }
}
