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
        private const int HISTORY_VERSION = 84726599;
        List<Guid> ids = new List<Guid>();
        


        double radius1 = 1.0;
        double radius2 = 1.0;
        string[] caps = new string[] { "None", "Flat", "Round" };
        int capIndex = 0;


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Rhino.ApplicationSettings.HistorySettings.RecordingEnabled = true;
            double tolerance = doc.ModelAbsoluteTolerance;

            GetObject gc = new GetObject();
            gc.SetCommandPrompt("Select curve(s) for multipipe");
            gc.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            gc.GroupSelect = true;
            gc.GetMultiple(1, 0);

            if (gc.CommandResult() != Rhino.Commands.Result.Success)
                return gc.CommandResult();

            List<Curve> curves = new List<Curve>();
            for (int i = 0; i < gc.ObjectCount; i++)
            {
                Rhino.DocObjects.ObjRef objref = gc.Object(i);
                Rhino.Geometry.Curve curve = objref.Curve();
                curves.Add(curve);
                ids.Add(objref.ObjectId);
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

            // Create a history record
            Rhino.DocObjects.HistoryRecord history = new Rhino.DocObjects.HistoryRecord(this, HISTORY_VERSION);
            WriteHistory(history, ids, radius1, radius2, capIndex);

            foreach (Curve x in curves)
            {
                pipe = Brep.CreatePipe(x, radiParam, radi, true, pCapM[capIndex], true, doc.ModelAbsoluteTolerance, doc.ModelAngleToleranceRadians);
                doc.Objects.AddBrep(pipe[0], null, history, false);
                doc.Views.Redraw();
            }


            doc.Views.Redraw();
            return Result.Success;
        }
        /// <summary>
        /// Rhino calls the virtual ReplayHistory functions to to remake an objects when inputs have changed.  
        /// </summary>
        protected override bool ReplayHistory(Rhino.DocObjects.ReplayHistoryData replay)
        {
            Rhino.DocObjects.ObjRef objref2 = null;
            List<Curve> curves2 = new List<Curve>();
            Brep[] pipe2;
            PipeCapMode[] pCapM = { PipeCapMode.None, PipeCapMode.Flat, PipeCapMode.Round };

            double radius1 = 0.0;
            double radius2 = 0.0;
            double[] radiParam = { 0.0, 0.5, 1.0 };
            double[] radi = { radius1, radius2, radius1 };

            if (!ReadHistory(replay, ref ids, ref radius1, ref radius2, ref capIndex))
                return false;

            foreach (Guid i in ids)
            {
                objref2 = new ObjRef(i);
                Rhino.Geometry.Curve curve = objref2.Curve();
                curves2.Add(curve);
                
            }

            foreach (Curve x in curves2)
            {
                pipe2 = Brep.CreatePipe(x, radiParam, radi, true, pCapM[capIndex], true, 0.01, 0.01);
                replay.Results[0].UpdateToBrep(pipe2[0], null);
            }

          

            return true;
        }

        /// <summary>
        /// Read a history record and extract the references for the antecedent objects.
        /// </summary>
        private bool ReadHistory(Rhino.DocObjects.ReplayHistoryData replay, ref List<Guid> ids, ref double radius1, ref double radius2, ref int capIndex)
        {
            if (HISTORY_VERSION != replay.HistoryVersion)
                return false;
            foreach (Guid g in ids)
            {
                Guid id;
                replay.TryGetGuid(0, out id);
            }
            
            //if (null == objrefs)
            //    return false;

            if (!replay.TryGetDouble(1, out radius1))
                return false;

            if (!replay.TryGetDouble(2, out radius2))
                return false;

            if (!replay.TryGetInt(3, out capIndex))
                return false;

            return true;
        }

        /// <summary>
        /// Write a history record referencing the antecedent objects
        /// The history should contain all the information required to reconstruct the input.
        /// This may include parameters, options, or settings.
        /// </summary>
        private bool WriteHistory(Rhino.DocObjects.HistoryRecord history, List<Guid> ids, double radius1, double radius2, int capIndex)
        {
            if (!history.SetGuids(0, ids))
                return false;

            if (!history.SetDouble(1, radius1))
                return false;

            if (!history.SetDouble(2, radius2))
                return false;

            if (!history.SetInt(3, capIndex))
                return false;
            return true;
        }
    }
}
