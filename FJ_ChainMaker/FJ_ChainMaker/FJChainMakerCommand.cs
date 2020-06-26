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
        double chainDis = 1.0;
        int chainAxisOffset = 45;
        double axisOffsetRadiant = 45 * (Math.PI/180);

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
            go.SetCommandPrompt("Select chain element.");
            go.SubObjectSelect = true;
            go.DeselectAllBeforePostSelect = false;
            //go.GroupSelect = true;
            //go.GetMultiple(1, -1);
            go.Get();
            if (go.CommandResult() != Result.Success)
                return go.CommandResult();
            Rhino.DocObjects.ObjRef objref1 = go.Object(0);
            Rhino.DocObjects.RhinoObject obj1 = objref1.Object();
            GeometryBase obj1Base = obj1.Geometry;
            

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
            gp2.DrawLineFromPoint(pt1,false);
            gp2.Get();
            if (gp2.CommandResult() != Rhino.Commands.Result.Success)
                return gp2.CommandResult();
            Point3d pt2 = gp2.Point();

            Vector3d vt1 = pt2 - pt1;

            sourcePlane = new Plane(pt1, vt1);
            Plane originPlane = new Plane(Point3d.Origin, vt1);
            Transform bform = Rhino.Geometry.Transform.PlaneToPlane(sourcePlane, originPlane);

            obj1Base.Transform(bform);
            
            GeometryBase[] obj1List = new GeometryBase[1] {obj1Base};
            
            var orientBlock = doc.InstanceDefinitions.Add("Block" + instDefCount, "OrientBlock", Point3d.Origin, obj1List);
            

            //orient instances along curve
            
            List<Guid> chainBlocks = new List<Guid>();
            Guid chainBlock;

            while (true)
            {

                foreach (var block in chainBlocks)
                {
                    doc.Objects.Delete(block, false);
                }
                chainBlocks = new List<Guid>();
                double curveLength = curve.GetLength();
                double curveDivide = curveLength / chainDis;
                int curveDivideInt = Convert.ToInt32(curveDivide);

                for (int ic = 0; ic < curveDivideInt; ic++)
                {
                    Point3d insertPoint = curve.PointAtLength(chainDis * ic);
                    Vector3d insertVec = curve.PointAtLength(chainDis * ic + 1) - curve.PointAtLength(chainDis * ic - 1);
                    Plane targetPlane = new Plane(insertPoint, insertVec);

                    var xvec = targetPlane.XAxis;
                    if (xvec.Z != 0)
                        targetPlane.Rotate(Math.PI / 2, insertVec);

                    var yvec = targetPlane.YAxis;
                    if (yvec.Z < 0)
                        targetPlane.Rotate(Math.PI, insertVec);

                    if (ic % 2 == 0)
                        targetPlane.Rotate(Math.PI / 2, insertVec);
                    targetPlane.Rotate(axisOffsetRadiant, insertVec);
                    Rhino.Geometry.Transform xform = Rhino.Geometry.Transform.PlaneToPlane(originPlane, targetPlane);
                    chainBlock = doc.Objects.AddInstanceObject(orientBlock, xform);
                    chainBlocks.Add(chainBlock);

                }

                doc.Views.Redraw();
                GetOption gd = new GetOption();
                gd.SetCommandPrompt("Set distance between element centers in mm and rotation offset in degree. Press enter to accept.");
                var dis = new Rhino.Input.Custom.OptionDouble(chainDis);
                var axisOffset = new Rhino.Input.Custom.OptionInteger(chainAxisOffset);
                gd.AddOptionDouble("distance", ref dis);
                gd.AddOptionInteger("rotation", ref axisOffset);
                gd.AcceptNothing(true);
                var resdis = gd.Get();
                if (resdis == GetResult.Nothing) break;

                chainDis = dis.CurrentValue;
                chainAxisOffset = axisOffset.CurrentValue;
                axisOffsetRadiant = chainAxisOffset * (Math.PI/180);


            }

            int index = doc.Groups.Add(chainBlocks);

            return Result.Success;
        }
    }
}
