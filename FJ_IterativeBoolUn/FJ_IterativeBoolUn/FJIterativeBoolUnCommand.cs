using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;

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
            const Rhino.DocObjects.ObjectType geometryFilter = Rhino.DocObjects.ObjectType.Surface | Rhino.DocObjects.ObjectType.PolysrfFilter;
            Rhino.DocObjects.ObjRef objref;
            Result rc = Rhino.Input.RhinoGet.GetOneObject("Select first/main polysurface or surface to bool",
                                                                false, geometryFilter, out objref);
            if (rc != Rhino.Commands.Result.Success)
                return rc;
            if (objref == null)
                return Rhino.Commands.Result.Failure;

            Rhino.Geometry.Brep brep = objref.Brep();
            bool resIssolid = brep.IsSolid;
            if (!resIssolid)
            {
                Dialogs.ShowMessage("Your polysurface or surface is not solid! Result might not be valid!", "Warning!");
            }
            Guid firstBrep = objref.ObjectId;

            doc.Objects.UnselectAll(true);
            Rhino.DocObjects.ObjRef[] objrefs;
            Result rcd = Rhino.Input.RhinoGet.GetMultipleObjects("Select rest of polysurfaces or surfaces to bool",
              false, geometryFilter, out objrefs);
            if (rcd != Rhino.Commands.Result.Success)
                return rcd;
            if (objrefs == null || objrefs.Length < 1)
                return Rhino.Commands.Result.Failure;
            bool isSolid = true;
            List<Rhino.Geometry.Brep> breps = new List<Rhino.Geometry.Brep>();
            for (int i = 0; i < objrefs.Length; i++)
            {
                Rhino.Geometry.Brep brepd = objrefs[i].Brep();
                resIssolid = brepd.IsSolid;
                if (!resIssolid)
                    isSolid = false;
                Guid secondBrep = objrefs[i].ObjectId;
                if (brepd != null && firstBrep != secondBrep)
                    breps.Add(brepd);
            }
            if (!isSolid)
                Dialogs.ShowMessage("At least on polysurface or surface to subtract is not solid! Result might not be valid!", "Warning!");

            doc.Objects.UnselectAll(true);
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
