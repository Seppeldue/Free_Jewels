using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_IterativeBoolUn
{
    public class FJIterativeBoolUnCommand : Command
    {
        public FJIterativeBoolUnCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJIterativeBoolUnCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_IterativeBoolUn"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Rhino.DocObjects.ObjRef objref;
            Result rc = Rhino.Input.RhinoGet.GetOneObject("Select first/main polysurface to bool",
                                                                false, Rhino.DocObjects.ObjectType.PolysrfFilter, out objref);
            if (rc != Rhino.Commands.Result.Success || objref == null)
                return rc;
            if (objref == null)
                return Rhino.Commands.Result.Failure;

            Rhino.Geometry.Brep brep = objref.Brep();
            Guid firstBrep = objref.ObjectId;

            doc.Objects.UnselectAll();
            Rhino.DocObjects.ObjRef[] objrefs;
            Result rcd = Rhino.Input.RhinoGet.GetMultipleObjects("Select rest of polysurfaces to bool",
              false, Rhino.DocObjects.ObjectType.PolysrfFilter, out objrefs);
            if (rcd != Rhino.Commands.Result.Success)
                return rcd;
            if (objrefs == null || objrefs.Length < 1)
                return Rhino.Commands.Result.Failure;

            List<Rhino.Geometry.Brep> breps = new List<Rhino.Geometry.Brep>();
            for (int i = 0; i < objrefs.Length; i++)
            {
                Rhino.Geometry.Brep brepd = objrefs[i].Brep();
                Guid secondBrep = objrefs[i].ObjectId;
                if (brepd != null && firstBrep != secondBrep)
                    breps.Add(brepd);
            }
            doc.Objects.UnselectAll();
            string name_fail_layer = "FJ Boolean Fails";
            Rhino.DocObjects.Layer boolFail = new Rhino.DocObjects.Layer();
            boolFail.Name = name_fail_layer;
            boolFail.Color = System.Drawing.Color.Red;
            doc.Layers.Add(boolFail);
            var fail_layer_index = doc.Layers.FindName(name_fail_layer);

            string name_done_layer = "FJ Boolean Done";
            Rhino.DocObjects.Layer boolDone = new Rhino.DocObjects.Layer();
            boolDone.Name = name_done_layer;
            boolDone.Color = System.Drawing.Color.BlueViolet;
            doc.Layers.Add(boolDone);
            var done_layer_index = doc.Layers.FindName(name_done_layer);

            double tolerance = doc.ModelAbsoluteTolerance;
            int a = 0;
            for (int i = 0; i < breps.Count; i++)
            {
                RhinoApp.WriteLine("computing number: " + i + " of: " + breps.Count + " operations...");

                List<Brep> brepBool = new List<Brep>();
                brepBool.Add(brep);
                brepBool.Add(breps[i]);
                Rhino.Geometry.Brep[] brepBoolNew = Rhino.Geometry.Brep.CreateBooleanUnion(brepBool, tolerance, true);

                if (brepBoolNew == null || brepBoolNew.Length > 1)
                {
                    a++;
                    Rhino.DocObjects.RhinoObject objFail = objrefs[i].Object();
                    objFail.Attributes.LayerIndex = fail_layer_index.Index;
                    objFail.CommitChanges();
                }
                else
                {
                    brep = brepBoolNew[0];
                    Rhino.DocObjects.RhinoObject obj2Org = objrefs[i].Object();
                    obj2Org.Attributes.LayerIndex = done_layer_index.Index;
                    obj2Org.CommitChanges();
                    //doc.Objects.Delete(obj2Org);
                }

                doc.Views.Redraw();
            }

            RhinoApp.WriteLine(a + " of " + breps.Count + " operations failed!");

            Rhino.DocObjects.RhinoObject obj1Org = objref.Object();
            doc.Objects.Delete(obj1Org);

            doc.Objects.AddBrep(brep);

            doc.Views.Redraw();
            return Rhino.Commands.Result.Success;


        }
    }
}
