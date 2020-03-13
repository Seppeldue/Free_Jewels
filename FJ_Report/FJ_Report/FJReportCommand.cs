using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;


namespace FJ_Report
{
    public class FJReportCommand : Command
    {
        public FJReportCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJReportCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_Report"; }
        }

        

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            string yg18ctName = "18ct yellow gold";
            string wg18ctName = "18ct white gold";

            Rhino.DocObjects.ObjectEnumeratorSettings settings = new Rhino.DocObjects.ObjectEnumeratorSettings();
            settings.VisibleFilter = true;
            System.Collections.Generic.List<Guid> ids = new System.Collections.Generic.List<Guid>();
            List<string> items = new List<string>();
            List<double> stones = new List<double>();
            List<int> qStones = new List<int>();

            foreach (Rhino.DocObjects.RhinoObject rhObj in doc.Objects.GetObjectList(settings))
            {
                
                var objType = rhObj.ObjectType;
                Guid objGuid = rhObj.Id;
                Rhino.DocObjects.ObjRef objref = new Rhino.DocObjects.ObjRef(objGuid);

                if (objType == Rhino.DocObjects.ObjectType.Curve)
                {
                    Curve curveRadius = objref.Curve();
                    Circle circleRadius = new Circle();
                    curveRadius.TryGetCircle(out circleRadius, doc.ModelAbsoluteTolerance);
                    double diamStone = Math.Round(circleRadius.Diameter, 2, MidpointRounding.AwayFromZero);

                    int match = stones.IndexOf(diamStone);

                    if (match == -1)
                    {
                        stones.Add(diamStone);
                        qStones.Add(1);
                    }
                    else
                    {
                        int currVal = qStones[match];
                        qStones[match] = currVal +1;
                    }
                }
                else 
                {
                    Rhino.DocObjects.Material mat = rhObj.GetMaterial(true);
                    var matName = mat.Name;
                    RhinoApp.WriteLine(matName);
                    if (matName == wg18ctName)
                    {
                        Brep objrefBrep = objref.Brep();
                        double volObj = objrefBrep.GetVolume();
                        double gold18k = Math.Round(volObj * 0.0158, 2, MidpointRounding.AwayFromZero);
                        items.Add("18ct white gold: " + gold18k+"g");
                    }
                    else if (matName == yg18ctName)
                    {
                        Brep objrefBrep = objref.Brep();
                        double volObj = objrefBrep.GetVolume();
                        double gold18k = Math.Round(volObj * 0.0158, 2, MidpointRounding.AwayFromZero);
                        items.Add("18ct yellow gold: " + gold18k + "g");
                    }
                }
            }

            items.Add("------------------------------------");
            double totalCt = 0.0;
            for (int i = 0; i < stones.Count; i++)
            {
                double ct;
                ct = Math.Round((stones[i] * stones[i] * stones[i] * 0.6 * 0.0061), 4, MidpointRounding.AwayFromZero);
                items.Add("Brill: " + qStones[i] + " x " + stones[i] + "mm " + ct +"ct Total: " + ct*qStones[i] + "ct");
                totalCt = totalCt + ct * qStones[i];
            }

            items.Add("Total Brillant weight: " + totalCt + "ct");

            Dialogs.ShowListBox("Report", "Elements", items);

            return Result.Success;
        }
    }
}
