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

        void CalMat(RhinoDoc doc, double spezWeight, Rhino.DocObjects.ObjectType objType, Rhino.DocObjects.ObjRef objref, List<string> matList, String[] matName,int pos, string g_ct)
        {
            if ((objType == Rhino.DocObjects.ObjectType.Surface | objType == Rhino.DocObjects.ObjectType.PolysrfFilter | objType == Rhino.DocObjects.ObjectType.Brep | objType == Rhino.DocObjects.ObjectType.Extrusion))
            {
                Brep objrefBrep = objref.Brep();
                double volObj = objrefBrep.GetVolume();
                double weight = Math.Round(volObj * spezWeight, 3, MidpointRounding.AwayFromZero);
                matList.Add(matName[pos] + " " + weight + g_ct);
            }
            else if (objType == Rhino.DocObjects.ObjectType.Mesh)
            {
                Mesh objrefMesh = objref.Mesh();
                double volObj = objrefMesh.Volume();
                double weight = Math.Round(volObj * spezWeight, 3, MidpointRounding.AwayFromZero);
                matList.Add(matName[pos] + " " + weight + g_ct);
            }
        }
        
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            String[] matName = new String[] { "WhiteGold_18ct", "YellowGold_18ct", "RedGold_18ct", "RodiumBlack_18ct", "Silver_925", "Platinum_960", "Ruby", "Emerald", "Saphir", "Paraiba", "Granat", "Amethyst", "Morganite", "Diamond" };

            Rhino.DocObjects.ObjectEnumeratorSettings settings = new Rhino.DocObjects.ObjectEnumeratorSettings();
            settings.VisibleFilter = true;
            System.Collections.Generic.List<Guid> ids = new System.Collections.Generic.List<Guid>();
            List<string> allItems = new List<string>();
            List<string> metalItems = new List<string>();
            List<string> stoneItems = new List<string>();
            List<double> circleStones = new List<double>();
            List<int> quantCirclesStones = new List<int>();

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

                    int match = circleStones.IndexOf(diamStone);

                    if (match == -1)
                    {
                        circleStones.Add(diamStone);
                        quantCirclesStones.Add(1);
                    }
                    else
                    {
                        int currVal = quantCirclesStones[match];
                        quantCirclesStones[match] = currVal + 1;
                    }
                }

                Rhino.DocObjects.Material mat = rhObj.GetMaterial(true);
                var objMatName = mat.Name;
                int pos = Array.IndexOf(matName, objMatName);
                switch (pos)
                {
                    //18ctGold
                    case int n when (n < 4):
                        double gold18ct = 0.016;
                        CalMat(doc, gold18ct, objType, objref, metalItems, matName, pos, " g");
                        break;
                    //Silver
                    case 4:
                        double silver = 0.0105;
                        CalMat(doc, silver ,objType, objref, metalItems, matName,pos, " g");
                        break;
                    //Platinum
                    case 5:
                        double platinum = 0.0203;
                        CalMat(doc, platinum, objType, objref, metalItems, matName, pos, " g");
                        break;
                    //ColorStones ruby=4g Emerald=2.65g saphr=4g Paraiba=3g Granat=3.9g Amethyst=2.65 Morganite=2.65
                    case 6:
                        double ruby = 0.02;
                        CalMat(doc, ruby, objType, objref, stoneItems, matName, pos, " ct");
                        break;
                    case 7:
                        double emerald = 0.0133;
                        CalMat(doc, emerald, objType, objref, stoneItems, matName, pos, " ct");
                        break;
                    case 8:
                        double saphir = 0.02;
                        CalMat(doc, saphir, objType, objref, stoneItems, matName, pos, " ct");
                        break;
                    case 9:
                        double paraiba = 0.015;
                        CalMat(doc, paraiba, objType, objref, stoneItems, matName, pos, " ct");
                        break;
                    case 10:
                        double granat = 0.0195;
                        CalMat(doc, granat, objType, objref, stoneItems, matName, pos, " ct");
                        break;
                    case 11:
                        double amethyst = 0.0133;
                        CalMat(doc, amethyst, objType, objref, stoneItems, matName, pos, " ct");
                        break;
                    case 12:
                        double morganite = 0.0133;
                        CalMat(doc, morganite, objType, objref, stoneItems, matName, pos, " ct");
                        break;
                    //Diamond
                    case 13:
                        double diamond = 0.02;
                        CalMat(doc, diamond, objType, objref, stoneItems, matName, pos, " ct");
                        break;
                }

            }

            metalItems.Sort();
            stoneItems.Sort();

            allItems.Add("--------------Metals--------------");
            for (int i = 0; i < metalItems.Count; i++)
            {
                allItems.Add(metalItems[i]);
            }

            allItems.Add("--------------Stone---------------");
            for (int i = 0; i < stoneItems.Count; i++)
            {
                allItems.Add(stoneItems[i]);
            }

            allItems.Add("----------From circles------------");
            double totalCt = 0.0;
            for (int i = 0; i < circleStones.Count; i++)
            {
                double ct;
                ct = Math.Round((circleStones[i] * circleStones[i] * circleStones[i] * 0.6 * 0.0061), 4, MidpointRounding.AwayFromZero);
                allItems.Add("Brill: " + quantCirclesStones[i] + " x " + circleStones[i] + " mm " + ct +" ct Total: " + ct*quantCirclesStones[i] + " ct");
                totalCt = totalCt + ct * quantCirclesStones[i];
            }

            allItems.Add("Total Brillant weight: " + totalCt + " ct");

            String clipboardString = "";
            foreach (String o in allItems)
            {
                clipboardString = clipboardString + o + "\n";
            }

            Clipboard.SetText(clipboardString);


            Dialogs.ShowListBox("Report", "This data is copied to your clipboard \n Use Ctrl+V to paste", allItems);

            return Result.Success;
        }
    }
}
