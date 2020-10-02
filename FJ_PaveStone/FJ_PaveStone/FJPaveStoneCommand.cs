using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.Render;
using Rhino.DocObjects;

namespace FJ_PaveStone
{
    public class FJPaveStoneCommand : Command
    {
        public FJPaveStoneCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJPaveStoneCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_PaveStone"; }
        }

        RenderMaterial FindMaterial(RhinoDoc doc, string name)
        {
            foreach (var material in doc.RenderMaterials)
            {
                if (material.Name == name)
                {
                    return material;
                }
            }

            return null;
        }

        List<Guid> ids = new List<Guid>();

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            double tolerance = doc.ModelAbsoluteTolerance;

            List<Curve> icur = new List<Curve>();

            GetObject gcr = new Rhino.Input.Custom.GetObject();
            gcr.SetCommandPrompt("Select reference circles for stones. (No curves)");
            gcr.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            gcr.GroupSelect = true;
            gcr.SubObjectSelect = true;
            gcr.DeselectAllBeforePostSelect = true;
            gcr.OneByOnePostSelect = false;
            gcr.GetMultiple(1, 0);

            for (int ie = 0; ie < gcr.ObjectCount; ie++)
            {
                Rhino.DocObjects.ObjRef objref = gcr.Object(ie);
                Rhino.DocObjects.RhinoObject obj = objref.Object();
                if (obj == null)
                    return Result.Failure;
                Curve refcr = objref.Curve();
                if (refcr == null)
                    return Result.Failure;
                obj.Select(false);

                icur.Add(refcr);
            }
            var rm = FindMaterial(doc, "Diamond");
            if (null == rm)
            {
                //Didn't find the material - create one and carry on.

                //Create a basic material
                var custom = new Rhino.DocObjects.Material();
                custom.Reflectivity = 1;
                custom.Transparency = 0.2;
                custom.SetEnvironmentTexture(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Free Jewels Rhino Plug-Ins/Dia3.jpg");
                custom.Name = "Diamond";
                custom.CommitChanges();

                rm = RenderMaterial.CreateBasicMaterial(custom);

                var docMats = doc.RenderMaterials;

                //docMats.BeginChange(RenderContent.ChangeContexts.Program);
                docMats.Add(rm);
                //docMats.EndChange();
            }



            // Create Stone Mesh
            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();


            mesh.Vertices.Add(0.0, 0.0, -0.44); //0

            mesh.Vertices.Add(0.0, 0.097, -0.363); //1
            mesh.Vertices.Add(0.069, 0.069, -0.363); //2
            mesh.Vertices.Add(0.097, 0.0, -0.363); //3

            mesh.Vertices.Add(0.0, 0.5, -0.013); //4
            mesh.Vertices.Add(0.098, 0.49, -0.005); //5
            mesh.Vertices.Add(0.191, 0.462, -0.013); //6
            mesh.Vertices.Add(0.278, 0.416, -0.005); //7
            mesh.Vertices.Add(0.354, 0.354, -0.013); //8
            mesh.Vertices.Add(0.416, 0.278, -0.005); //9
            mesh.Vertices.Add(0.462, 0.191, -0.013); //10
            mesh.Vertices.Add(0.49, 0.098, -0.005); //11
            mesh.Vertices.Add(0.5, 0.0, -0.013); //12

            mesh.Vertices.Add(0.0, 0.5, 0.013); //13
            mesh.Vertices.Add(0.098, 0.49, 0.005); //14
            mesh.Vertices.Add(0.191, 0.462, 0.013); //15
            mesh.Vertices.Add(0.278, 0.416, 0.005); //16
            mesh.Vertices.Add(0.354, 0.354, 0.013); //17
            mesh.Vertices.Add(0.416, 0.278, 0.005); //18
            mesh.Vertices.Add(0.462, 0.191, 0.013); //19
            mesh.Vertices.Add(0.49, 0.098, 0.005); //20
            mesh.Vertices.Add(0.5, 0.0, 0.013); //21

            mesh.Vertices.Add(0.0, 0.372, 0.12); //22
            mesh.Vertices.Add(0.263, 0.263, 0.12); //23
            mesh.Vertices.Add(0.372, 0.0, 0.12); //24
            mesh.Vertices.Add(0.263, -0.263, 0.12); //25
            mesh.Vertices.Add(0.0, -0.372, 0.12); //26
            mesh.Vertices.Add(-0.263, -0.263, 0.12); //27
            mesh.Vertices.Add(-0.372, 0.0, 0.12); //28
            mesh.Vertices.Add(-0.263, 0.263, 0.12); //29

            mesh.Vertices.Add(0.109, 0.263, 0.16); //30
            mesh.Vertices.Add(0.263, 0.109, 0.16); //31
            mesh.Vertices.Add(0.263, -0.109, 0.16); //32
            mesh.Vertices.Add(0.109, -0.263, 0.16); //33
            mesh.Vertices.Add(-0.109, -0.263, 0.16); //34
            mesh.Vertices.Add(-0.263, -0.109, 0.16); //35
            mesh.Vertices.Add(-0.263, 0.109, 0.16); //36
            mesh.Vertices.Add(-0.109, 0.263, 0.16); //37

            mesh.Vertices.Add(0.0, 0.0, 0.16); //38

            mesh.Faces.AddFace(0, 1, 6, 2);
            mesh.Faces.AddFace(0, 2, 10, 3);

            mesh.Faces.AddFace(1, 4, 5, 6);
            mesh.Faces.AddFace(2, 6, 7, 8);
            mesh.Faces.AddFace(2, 8, 9, 10);
            mesh.Faces.AddFace(3, 10, 11, 12);

            mesh.Faces.AddFace(4, 13, 14, 5);
            mesh.Faces.AddFace(5, 14, 15, 6);
            mesh.Faces.AddFace(6, 15, 16, 7);
            mesh.Faces.AddFace(7, 16, 17, 8);
            mesh.Faces.AddFace(8, 17, 18, 9);
            mesh.Faces.AddFace(9, 18, 19, 10);
            mesh.Faces.AddFace(10, 19, 20, 11);
            mesh.Faces.AddFace(11, 20, 21, 12);

            mesh.Faces.AddFace(13, 22, 15, 14);
            mesh.Faces.AddFace(15, 23, 17, 16);
            mesh.Faces.AddFace(17, 23, 19, 18);
            mesh.Faces.AddFace(19, 24, 21, 20);

            mesh.Faces.AddFace(15, 22, 30, 23);
            mesh.Faces.AddFace(19, 23, 31, 24);

            mesh.Faces.AddFace(23, 30, 31);
            mesh.Faces.AddFace(24, 31, 32);

            mesh.Faces.AddFace(32, 31, 30, 38);

            mesh.Unweld(0.001, false);

            Mesh meshAll = new Mesh();
            for (int i = 0; i < 4; i++)
            {
                meshAll.Append(mesh);
                Point3d center = new Point3d(0.0, 0.0, 0.0);
                Vector3d rotVec = new Vector3d(0.0, 0.0, 1.0);
                mesh.Rotate(Math.PI / 2, rotVec, center);
            }
            meshAll.Compact();
            meshAll.Weld(0.001);

            //Get object Guid to apply render material
            var meshGuid = doc.Objects.AddMesh(meshAll);
            ObjRef objre = new ObjRef(meshGuid);
            RhinoObject obje = objre.Object();
            obje.RenderMaterial = rm;
            obje.CommitChanges();

            //Make InstanceDefinition
            string instDefCount = DateTime.Now.ToString("ddMMyyyyHHmmss");

            var geometry = new System.Collections.Generic.List<Rhino.Geometry.GeometryBase>() { obje.Geometry };
            var attributes = new System.Collections.Generic.List<Rhino.DocObjects.ObjectAttributes>() { obje.Attributes };
            

            var stoneIndex = doc.InstanceDefinitions.Add("Stone" + instDefCount, "StoneMesh 1mm", Point3d.Origin, geometry, attributes);
            
            List<InstanceReferenceGeometry> meshPave = new List<InstanceReferenceGeometry>();


            foreach (Curve c in icur)
            {
                Circle circle1 = new Circle();
                c.TryGetCircle(out circle1, tolerance);
                double radius = circle1.Diameter;
                Point3d center = circle1.Center;
                Vector3d moveV = new Vector3d(center);
                Vector3d zaxis = new Vector3d(0.0, 0.0, 1.0);
                Plane planeOr = new Plane(center, zaxis);
                Plane planeNew = circle1.Plane;
                var transform1 = Transform.Translation(moveV);
                var transform2 = Transform.Scale(center, radius);
                var transform3 = Transform.PlaneToPlane(planeOr, planeNew);

                var stoneA = doc.Objects.AddInstanceObject(stoneIndex, transform1);
                var stoneB = doc.Objects.Transform(stoneA, transform2, true);
                var stoneC = doc.Objects.Transform(stoneB, transform3, true);
                
                ids.Add(stoneC);
            }
            doc.Groups.Add(ids);
            doc.Objects.Delete(obje);
            doc.Views.Redraw();

            return Result.Success;
        }
    }
}
