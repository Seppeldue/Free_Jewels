using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using System.Windows.Forms;


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

            String[] matName = new String[] { "WhiteGold_18ct", "YellowGold_18ct", "RedGold_18ct", "RodiumBlack_18ct", "Silver_925", "Platinum_960", "Ruby", "Emerald", "Saphir", "Paraiba", "Granat", "Amethyst", "Morganite", "Diamond" };

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
                        qStones[match] = currVal + 1;
                    }
                }
                else if (objType == Rhino.DocObjects.ObjectType.Surface | objType == Rhino.DocObjects.ObjectType.PolysrfFilter | objType == Rhino.DocObjects.ObjectType.Brep | objType == Rhino.DocObjects.ObjectType.Extrusion)
                {
                    Rhino.DocObjects.Material mat = rhObj.GetMaterial(true);
                    var objMatName = mat.Name;
                    int pos = Array.IndexOf(matName, objMatName);
                    //RhinoApp.WriteLine(matName);
                    if (pos < 4)
                    {
                        Brep objrefBrep = objref.Brep();
                        double volObj = objrefBrep.GetVolume();
                        double gold18k = Math.Round(volObj * 0.0158, 2, MidpointRounding.AwayFromZero);
                        items.Add(matName[pos] + " " + gold18k + " g");
                    }
                    else if (pos == 4)
                    {
                        Brep objrefBrep = objref.Brep();
                        double volObj = objrefBrep.GetVolume();
                        double silver925 = Math.Round(volObj * 0.0158, 2, MidpointRounding.AwayFromZero);
                        items.Add(matName[pos] + " " + silver925 + " g");
                    }
                    else if (pos == 5)
                    {
                        Brep objrefBrep = objref.Brep();
                        double volObj = objrefBrep.GetVolume();
                        double plat960 = Math.Round(volObj * 0.0158, 2, MidpointRounding.AwayFromZero);
                        items.Add(matName[pos] + " " + plat960 + " g");
                    }
                }
                else if (objType == Rhino.DocObjects.ObjectType.Mesh)
                {
                    Rhino.DocObjects.Material mat = rhObj.GetMaterial(true);
                    var objMatName = mat.Name;
                    int pos = Array.IndexOf(matName, objMatName);

                    if (pos == 13)
                    {
                        Mesh objrefMesh = objref.Mesh();
                        double volObj = objrefMesh.Volume();
                        double diaCt = Math.Round(volObj * 0.02, 4, MidpointRounding.AwayFromZero);
                        items.Add(matName[pos] + " " + diaCt + " ct");
                    }
                    else if (pos < 13)
                    {
                        Mesh objrefMesh = objref.Mesh();
                        double volObj = objrefMesh.Volume();
                        double colStone = Math.Round(volObj * 0.0158, 2, MidpointRounding.AwayFromZero);
                        items.Add(matName[pos] + " " + colStone + " ct");
                    }
                }

            }

            items.Add("----------From circles-------------");
            double totalCt = 0.0;
            for (int i = 0; i < stones.Count; i++)
            {
                double ct;
                ct = Math.Round((stones[i] * stones[i] * stones[i] * 0.6 * 0.0061), 4, MidpointRounding.AwayFromZero);
                items.Add("Brill: " + qStones[i] + " x " + stones[i] + " mm " + ct +" ct Total: " + ct*qStones[i] + " ct");
                totalCt = totalCt + ct * qStones[i];
            }

            items.Add("Total Brillant weight: " + totalCt + " ct");

            String clipboardString = "";
            foreach (String o in items)
            {
                clipboardString = clipboardString + o + "\n";
            }

            Clipboard.SetText(clipboardString);


            Dialogs.ShowListBox("Report", "This data is copied to your clipboard \n Use Ctrl+V to paste", items);

            return Result.Success;
        }
    }
}
