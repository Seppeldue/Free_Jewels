using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using Rhino.DocObjects;

namespace FJ_IterativeBoolDif
{
    public class FJIterativeBoolDifCommand : Command
    {
        public FJIterativeBoolDifCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJIterativeBoolDifCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_IterativeBoolDif"; }
        }


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //Select first Surface
            const Rhino.DocObjects.ObjectType geometryFilter = Rhino.DocObjects.ObjectType.Surface | Rhino.DocObjects.ObjectType.PolysrfFilter ;
            Rhino.DocObjects.ObjRef objref;
            Result rc = Rhino.Input.RhinoGet.GetOneObject("Select polysurface or surface to subtract from",
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
            doc.Objects.UnselectAll();

            //Select set of surfaces to substract
            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
            go.SetCommandPrompt("Select set of polysurfaces or surfaces to subtract");
            go.GeometryFilter = geometryFilter | Rhino.DocObjects.ObjectType.InstanceReference;
            go.GroupSelect = true;
            go.GetMultiple(1, 0);
 
            bool isSolid = true;

            //Add set to breps list
            List<Rhino.Geometry.Brep> breps = new List<Rhino.Geometry.Brep>();
            for (int i = 0; i < go.ObjectCount; i++)
            {

                //Explode if instance object and add to breps list
                if (go.Object(i).Object() is InstanceObject)
                {
                    InstanceObject instObj = go.Object(i).Object() as InstanceObject;
                    RhinoObject[] explodedObjects;
                    ObjectAttributes[] attributesOfExplodedObjects;
                    Transform[] transformOfExplodedObjects;

                    instObj.Explode(false, out explodedObjects, out attributesOfExplodedObjects, out transformOfExplodedObjects);
                    Guid addedObjectID = doc.Objects.Add(explodedObjects[0].Geometry, explodedObjects[0].Attributes);

                    ObjRef objrefs = new Rhino.DocObjects.ObjRef(addedObjectID);
                    Rhino.Geometry.Brep brepd = objrefs.Brep();

                    brepd.Transform(transformOfExplodedObjects[0]);

                    resIssolid = brepd.IsSolid;
                    if (!resIssolid)
                        isSolid = false;
                    if (brepd != null && firstBrep != addedObjectID)
                        breps.Add(brepd);
                    doc.Objects.Delete(addedObjectID, true);
                }
                else
                {
                    Rhino.DocObjects.ObjRef objrefs = go.Object(i);
                    Rhino.Geometry.Brep brepd = objrefs.Brep();
                    resIssolid = brepd.IsSolid;
                    if (!resIssolid)
                        isSolid = false;
                    if (brepd != null && firstBrep != objrefs.ObjectId)
                        breps.Add(brepd);
                }

                
            }
            if (!isSolid)
                Dialogs.ShowMessage("At least on polysurface or surface to subtract is not solid! Result might not be valid!", "Warning!");

            doc.Objects.UnselectAll();

            //Create layers for failed and successfull booleans if not already existing
            var fail_layer_index = doc.Layers.FindName("FJ Boolean Fails");
            if (fail_layer_index == null)
            {
                string name_fail_layer = "FJ Boolean Fails";
                Rhino.DocObjects.Layer boolFail = new Rhino.DocObjects.Layer();
                boolFail.Name = name_fail_layer;
                boolFail.Color = System.Drawing.Color.Red;
                doc.Layers.Add(boolFail);
                fail_layer_index = doc.Layers.FindName(name_fail_layer);
            }

            var done_layer_index = doc.Layers.FindName("FJ Boolean Done");
            if (done_layer_index == null)
            {
                string name_done_layer = "FJ Boolean Done";
                Rhino.DocObjects.Layer boolDone = new Rhino.DocObjects.Layer();
                boolDone.Name = name_done_layer;
                boolDone.Color = System.Drawing.Color.BlueViolet;
                doc.Layers.Add(boolDone);
                done_layer_index = doc.Layers.FindName(name_done_layer);
            }


            //Compute boolean differences
            double tolerance = doc.ModelAbsoluteTolerance;
            int a = 0;
            for (int i = 0; i < breps.Count; i++)
            {
                RhinoApp.WriteLine("computing number: " + i + " of: " + breps.Count + " operations...");
                try
                {
                    Rhino.Geometry.Brep[] brepDif = Rhino.Geometry.Brep.CreateBooleanDifference(brep, breps[i], tolerance, true);
                    brep = brepDif[0];

                }
                catch
                {
                    a++;
                    doc.Objects.Delete(go.Object(i).Object());

                    var boolresultfail = doc.Objects.AddBrep(breps[i]);
                    ObjRef objFailref = new ObjRef(boolresultfail);
                    Rhino.DocObjects.RhinoObject objFail = objFailref.Object();
                    objFail.Attributes.LayerIndex = fail_layer_index.Index;
                    objFail.CommitChanges();
                    doc.Views.Redraw();
                    continue;
                }
                doc.Objects.Delete(go.Object(i).Object());

                var boolresult = doc.Objects.AddBrep(breps[i]);
                ObjRef obj20ref = new ObjRef(boolresult);
                Rhino.DocObjects.RhinoObject obj2Org = obj20ref.Object();
                obj2Org.Attributes.LayerIndex = done_layer_index.Index;
                obj2Org.CommitChanges();
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
