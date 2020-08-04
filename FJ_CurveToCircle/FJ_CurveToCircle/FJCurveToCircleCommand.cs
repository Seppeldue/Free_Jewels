using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_CurveToCircle
{
    public class FJCurveToCircleCommand : Command
    {
        public FJCurveToCircleCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJCurveToCircleCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_CurveToCircle"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var layerCheck = doc.Layers.FindName("CurveToCircle");
            if (layerCheck == null)
            {
                doc.Layers.Add("CurveToCircle", System.Drawing.Color.Turquoise);
                layerCheck = doc.Layers.FindName("CurveToCircle");
            }
                

            

            GetObject gc = new GetObject();
            gc.SetCommandPrompt("Select curves to make into circles");
            gc.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            gc.GroupSelect = true;
            gc.SubObjectSelect = false;
            gc.EnableClearObjectsOnEntry(false);
            gc.EnableUnselectObjectsOnExit(false);
            gc.DeselectAllBeforePostSelect = false;
            gc.GetMultiple(1, 0);

            for (int i = 0; i < gc.ObjectCount; i++)
            {
                Rhino.DocObjects.ObjRef objref = gc.Object(i);
                var curve = objref.Curve();
                if (curve == null)
                    return Result.Nothing;

                Circle circle;
                
                if (!curve.TryGetCircle(out circle))
                {

                    if (!curve.IsClosed)
                    {
                        if (curve.IsClosable(doc.ModelAbsoluteTolerance))
                        {
                            curve.MakeClosed(doc.ModelAbsoluteTolerance);
                        }
                        else
                        {
                            Curve blendCurve;
                            blendCurve = Curve.CreateBlendCurve(curve, curve, BlendContinuity.Curvature);
                            Curve[] curveSet = { curve, blendCurve };
                            Curve[] joinedCurve = Curve.JoinCurves(curveSet);
                            curve = joinedCurve[0];

                        }

                    }

                    //curve.Rebuild(3,2,true);
                    double[] curveDivide = curve.DivideByCount(8, true);
                    Point3d curvePoint;
                    Point3d[] curvePoints = new Point3d[8];
                    for (int d = 0; d < 8; d++)
                    {
                        curvePoint = curve.PointAt(curveDivide[d]);
                        curvePoints[d] = curvePoint;
                    }
                    
                    if (!Circle.TryFitCircleToPoints(curvePoints, out circle))
                        circle = new Circle(5);



                }



                var circGuid = doc.Objects.AddCircle(circle);
                Rhino.DocObjects.RhinoObject circObj = doc.Objects.Find(circGuid);
                circObj.Attributes.LayerIndex = layerCheck.Index;
                circObj.CommitChanges();

            }


            doc.Views.Redraw();

            return Result.Success;
        }
    }
}
