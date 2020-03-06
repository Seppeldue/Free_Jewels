using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_WeightReport
{
    public class FJWeightReportCommand : Command
    {
        public FJWeightReportCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJWeightReportCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_WeightReport"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            bool allBrepSolid = true;
            const Rhino.DocObjects.ObjectType geometryFilter = Rhino.DocObjects.ObjectType.Brep | Rhino.DocObjects.ObjectType.Mesh;

            GetObject wro = new Rhino.Input.Custom.GetObject();
            wro.SetCommandPrompt("Select objects to weight.");
            wro.GeometryFilter = geometryFilter;

            wro.SubObjectSelect = true;
            wro.DeselectAllBeforePostSelect = false;
            wro.OneByOnePostSelect = true;
            wro.GetMultiple(1, 0);

            double volObj3 = 0;


            for (int i = 0; i < wro.ObjectCount; i++)
            {
                Rhino.DocObjects.ObjRef objref = wro.Object(i);
                // get selected surface object
                Rhino.DocObjects.RhinoObject obj = objref.Object();
                if (obj == null)
                    return Result.Failure;
                // get selected surface (face)

                Brep refcr = objref.Brep();
                if (refcr != null)
                {
                    var volObj = VolumeMassProperties.Compute(refcr);
                    double volObj2 = refcr.GetVolume();
                    volObj3 = volObj3 + volObj2;
                    bool brepSolid = refcr.IsSolid;
                    if (brepSolid == false)
                        allBrepSolid = false;
                }
                if (refcr == null)
                {
                    Mesh refcm = objref.Mesh();
                    Brep refmb = Brep.CreateFromMesh(refcm, false);
                    var volOb = VolumeMassProperties.Compute(refmb);
                    double volObj4 = refmb.GetVolume();
                    volObj3 = volObj3 + volObj4;
                    bool brepSolid = refmb.IsSolid;
                    if (brepSolid == false)
                        allBrepSolid = false;
                }



            }

            double gold18k = Math.Round(volObj3 * 0.0158, 2, MidpointRounding.AwayFromZero);
            double gold14k = Math.Round(volObj3 * 0.0146, 2, MidpointRounding.AwayFromZero);
            double platinum = Math.Round(volObj3 * 0.0214, 2, MidpointRounding.AwayFromZero);
            double palladium = Math.Round(volObj3 * 0.012, 2, MidpointRounding.AwayFromZero);
            double silver925 = Math.Round(volObj3 * 0.0102, 2, MidpointRounding.AwayFromZero);
            double diaCt = Math.Round(volObj3 * 0.01755, 2, MidpointRounding.AwayFromZero);

            if (allBrepSolid)
                RhinoApp.WriteLine("The weight is: Gold 18k " + gold18k + "g, Gold 14k " + gold14k + "g, Platinum " + platinum + "g, Palladium " + palladium + "g, Silver 925 " + silver925 + "g, Diamond" + diaCt + "Ct");
            else
                RhinoApp.WriteLine("AT LEAST ONE OBJECT IS OPEN! Result might be false!" + "The weight is: Gold 18k " + gold18k + "g, Gold 14k " + gold14k + "g, Platinum " + platinum + "g, Palladium " + palladium + "g, Silver 925 " + silver925 + "g, Diamond" + diaCt + "Ct");





            return Result.Success;
        }
    }
}
