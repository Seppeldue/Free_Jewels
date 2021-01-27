using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.DocObjects;
using Rhino.UI;

namespace FJ_MultiPipe
{
    public class FJMultiPipeCommand : Command
    {
        public FJMultiPipeCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJMultiPipeCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_MultiPipe"; }
        }

        //Globals
        double radius1 = 1.0;
        double radius2 = 1.0;
        string[] caps = new string[] { "None", "Flat", "Round" };
        int capIndex = 0;


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            GetObject gc = new GetObject();
            gc.SetCommandPrompt("Select curve(s) for multipipe");
            gc.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            gc.GroupSelect = true;
            gc.GetMultiple(1, 0);

            if (gc.CommandResult() != Rhino.Commands.Result.Success)
                return gc.CommandResult();

            List<Curve> ids = new List<Curve>();
            for (int i = 0; i < gc.ObjectCount; i++)
            {
                Rhino.DocObjects.ObjRef objref = gc.Object(i);
                Rhino.Geometry.Curve curve = objref.Curve();
                ids.Add(curve);
            }


            Rhino.Input.Custom.GetNumber gn = new Rhino.Input.Custom.GetNumber();
            gn.SetCommandPrompt("Start and end radius");
            int opList = gn.AddOptionList("Cap", caps, capIndex);
            gn.SetDefaultNumber(radius1);

            while (true)
            {
                Rhino.Input.GetResult get_rc = gn.Get();
                if (gn.CommandResult() != Rhino.Commands.Result.Success)
                    return gn.CommandResult();

                if (get_rc == Rhino.Input.GetResult.Number)
                {
                    radius1 = gn.Number();
                    if (radius1 < 0 || radius1 < 0.0)
                    {
                        Dialogs.ShowMessage("Value must be >= 0.0", "Warning!");
                        continue;
                    }
                }
                    
                
                else if (get_rc == Rhino.Input.GetResult.Option)
                {
                    if (gn.OptionIndex() == opList)
                    capIndex = gn.Option().CurrentListOptionIndex;
                    continue;
                }

                break;
            }



            while (true)
            {
                RhinoGet.GetNumber("Middle radius", false, ref radius2);
                if (radius2 <= 0 || radius2 <=0.0)
                {
                    Dialogs.ShowMessage("Value must be > 0.0", "Warning!");
                    continue;
                }

                break;
            }

            
       

            Brep[] pipe;

            double[] radiParam = { 0.0, 0.5, 1.0 };
            double[] radi = { radius1, radius2, radius1 };
            PipeCapMode[] pCapM = { PipeCapMode.None, PipeCapMode.Flat, PipeCapMode.Round };

            foreach (Curve x in ids)
            {
                pipe = Brep.CreatePipe(x, radiParam, radi, true, pCapM[capIndex], true, doc.ModelAbsoluteTolerance, doc.ModelAngleToleranceRadians);
                doc.Objects.AddBrep(pipe[0]);
                doc.Views.Redraw();
            }


            doc.Views.Redraw();
            return Result.Success;
        }
    }
}
