﻿using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_GemMaker
{
    public class FJGemMakerCommand : Command
    {
        public FJGemMakerCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJGemMakerCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_GemMaker"; }
        }



        public static void BrillantGem(RhinoDoc doc, double brillDiam)
        {
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
            meshAll.Scale(brillDiam);

            doc.Objects.AddMesh(meshAll);
        }
        double brillDiam = 1;

        public static void OvalGem(RhinoDoc doc, double ovalWidth, double ovalLength)
        {
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

            Point3d origin = new Point3d(0.0, 0.0, 0.0);
            Vector3d vt1 = new Vector3d(0.0, 0.0, 1.0);
            Plane pl0 = new Plane(origin, vt1);
            Transform scale1D = new Transform();
            scale1D = Transform.Scale(pl0, ovalWidth, ovalLength, ovalWidth);
            meshAll.Transform(scale1D);
            doc.Objects.AddMesh(meshAll);
        }
        double ovalLength = 1.5;
        double ovalWidth = 1.0;

        public static void BaguetteGem(RhinoDoc doc, double bagWidth, double bagLength)
        {
            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();

            mesh.Vertices.Add(0.188, 0.0, -0.293); //0
            mesh.Vertices.Add(-0.188, 0.0, -0.293); //1

            mesh.Vertices.Add(-0.25, -0.1, -0.247); //2
            mesh.Vertices.Add(0.25, -0.1, -0.247); //3
            mesh.Vertices.Add(0.25, 0.1, -0.247); //4

            mesh.Vertices.Add(-0.438, -0.4, -0.082); //5
            mesh.Vertices.Add(0.438, -0.4, -0.082); //6
            mesh.Vertices.Add(0.438, 0.4, -0.082); //7

            mesh.Vertices.Add(-0.5, -0.5, -0.006); //8
            mesh.Vertices.Add(0.5, -0.5, -0.006); //9
            mesh.Vertices.Add(0.5, 0.5, -0.006); //10

            mesh.Vertices.Add(-0.5, -0.5, 0.006); //11
            mesh.Vertices.Add(0.5, -0.5, 0.006); //12
            mesh.Vertices.Add(0.5, 0.5, 0.006); //13

            mesh.Vertices.Add(-0.438, -0.4, 0.06); //14
            mesh.Vertices.Add(0.438, -0.4, 0.06); //15
            mesh.Vertices.Add(0.438, 0.4, 0.06); //16

            mesh.Vertices.Add(-0.375, -0.3, 0.088); //17
            mesh.Vertices.Add(0.375, -0.3, 0.088); //18
            mesh.Vertices.Add(0.375, 0.3, 0.088); //19
            mesh.Vertices.Add(-0.375, 0.3, 0.088); //20

            mesh.Faces.AddFace(1, 0, 3, 2);
            mesh.Faces.AddFace(0, 4, 3);

            mesh.Faces.AddFace(2, 3, 6, 5);
            mesh.Faces.AddFace(3, 4, 7, 6);

            mesh.Faces.AddFace(5, 6, 9, 8);
            mesh.Faces.AddFace(6, 7, 10, 9);

            mesh.Faces.AddFace(8, 9, 12, 11);
            mesh.Faces.AddFace(9, 10, 13, 12);

            mesh.Faces.AddFace(11, 12, 15, 14);
            mesh.Faces.AddFace(12, 13, 16, 15);

            mesh.Faces.AddFace(14, 15, 18, 17);
            mesh.Faces.AddFace(15, 16, 19, 18);

            mesh.Faces.AddFace(17, 18, 19, 20);

            mesh.Unweld(0.001, false);

            Mesh meshAll = new Mesh();
            for (int i = 0; i < 2; i++)
            {
                meshAll.Append(mesh);
                Point3d center = new Point3d(0.0, 0.0, 0.0);
                Vector3d rotVec = new Vector3d(0.0, 0.0, 1.0);
                mesh.Rotate(Math.PI, rotVec, center);
            }
            meshAll.Compact();
            meshAll.Weld(0.001);

            Point3d origin = new Point3d(0.0, 0.0, 0.0);
            Vector3d vt1 = new Vector3d(0.0, 0.0, 1.0);
            Plane pl0 = new Plane(origin, vt1);
            Transform scale1D = new Transform();
            scale1D = Transform.Scale(pl0, bagWidth, bagLength, bagWidth);
            meshAll.Transform(scale1D);
            doc.Objects.AddMesh(meshAll);
        }
        double bagLength = 1.0;
        double bagWidth = 0.62;

        public static void EmeraldGem(RhinoDoc doc, double emWidth, double emLength, double em2Width, double em2Length, double emHigth, double emGridle, double emTop, double emFatt, double emLine)
        {
            double emFat3 = 1 + (emFatt / 20);
            double emFat2 = 1 + (emFatt / 50);
            double emFat1 = 1 + (emFatt / 100);

            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();

            mesh.Vertices.Add(-(emLine / 2), 0.0, -(emHigth - emTop - (emGridle / 2))); //0
            mesh.Vertices.Add((emLine / 2), 0.0, -(emHigth - emTop - (emGridle / 2))); //1

            mesh.Vertices.Add(-((((emLength - emLine) / 2) * 0.29 * emFat3) + (emLine / 2)), ((em2Width / 2) * 0.29 * emFat3), -(((emHigth - emTop - (emGridle / 2)) * 0.786) + (emGridle / 2))); //2
            mesh.Vertices.Add(-((((emLength - emLine) / 2) * 0.29 * emFat3) + (emLine / 2)), -((em2Width / 2) * 0.29 * emFat3), -(((emHigth - emTop - (emGridle / 2)) * 0.786) + (emGridle / 2))); //3
            mesh.Vertices.Add(-((((em2Length - emLine) / 2) * 0.285 * emFat3) + (emLine / 2)), -((emWidth / 2) * 0.285 * emFat3), -(((emHigth - emTop - (emGridle / 2)) * 0.786) + (emGridle / 2))); //4
            mesh.Vertices.Add(((((em2Length - emLine) / 2) * 0.285 * emFat3) + (emLine / 2)), -((emWidth / 2) * 0.285 * emFat3), -(((emHigth - emTop - (emGridle / 2)) * 0.786) + (emGridle / 2))); //5
            mesh.Vertices.Add(((((emLength - emLine) / 2) * 0.29 * emFat3) + (emLine / 2)), -((em2Width / 2) * 0.29 * emFat3), -(((emHigth - emTop - (emGridle / 2)) * 0.786) + (emGridle / 2))); //6

            mesh.Vertices.Add(-((((emLength - emLine) / 2) * 0.56 * emFat2) + (emLine / 2)), ((em2Width / 2) * 0.56 * emFat2), -(((emHigth - emTop - (emGridle / 2)) * 0.55) + (emGridle / 2))); //7
            mesh.Vertices.Add(-((((emLength - emLine) / 2) * 0.56 * emFat2) + (emLine / 2)), -((em2Width / 2) * 0.56 * emFat2), -(((emHigth - emTop - (emGridle / 2)) * 0.55) + (emGridle / 2))); //8
            mesh.Vertices.Add(-((((em2Length - emLine) / 2) * 0.565 * emFat2) + (emLine / 2)), -((emWidth / 2) * 0.565 * emFat2), -(((emHigth - emTop - (emGridle / 2)) * 0.55) + (emGridle / 2))); //9
            mesh.Vertices.Add(((((em2Length - emLine) / 2) * 0.565 * emFat2) + (emLine / 2)), -((emWidth / 2) * 0.565 * emFat2), -(((emHigth - emTop - (emGridle / 2)) * 0.55) + (emGridle / 2))); //10
            mesh.Vertices.Add(((((emLength - emLine) / 2) * 0.565 * emFat2) + (emLine / 2)), -((em2Width / 2) * 0.56 * emFat2), -(((emHigth - emTop - (emGridle / 2)) * 0.55) + (emGridle / 2))); //11

            mesh.Vertices.Add(-((((emLength - emLine) / 2) * 0.8 * emFat1) + (emLine / 2)), ((em2Width / 2) * 0.82 * emFat1), -(((emHigth - emTop - (emGridle / 2)) * 0.27) + (emGridle / 2))); //12
            mesh.Vertices.Add(-((((emLength - emLine) / 2) * 0.8 * emFat1) + (emLine / 2)), -((em2Width / 2) * 0.82 * emFat1), -(((emHigth - emTop - (emGridle / 2)) * 0.27) + (emGridle / 2))); //13
            mesh.Vertices.Add(-((((em2Length - emLine) / 2) * 0.8 * emFat1) + (emLine / 2)), -((emWidth / 2) * 0.82 * emFat1), -(((emHigth - emTop - (emGridle / 2)) * 0.27) + (emGridle / 2))); //14
            mesh.Vertices.Add(((((em2Length - emLine) / 2) * 0.8 * emFat1) + (emLine / 2)), -((emWidth / 2) * 0.82 * emFat1), -(((emHigth - emTop - (emGridle / 2)) * 0.27) + (emGridle / 2))); //15
            mesh.Vertices.Add(((((emLength - emLine) / 2) * 0.8 * emFat1) + (emLine / 2)), -((em2Width / 2) * 0.82 * emFat1), -(((emHigth - emTop - (emGridle / 2)) * 0.27) + (emGridle / 2))); //16

            mesh.Vertices.Add(-(emLength / 2), (em2Width / 2), -(emGridle / 2)); //17
            mesh.Vertices.Add(-(emLength / 2), -(em2Width / 2), -(emGridle / 2)); //18
            mesh.Vertices.Add(-(em2Length / 2), -(emWidth / 2), -(emGridle / 2)); //19
            mesh.Vertices.Add((em2Length / 2), -(emWidth / 2), -(emGridle / 2)); //20
            mesh.Vertices.Add((emLength / 2), -(em2Width / 2), -(emGridle / 2)); //21

            mesh.Vertices.Add(-(emLength / 2), (em2Width / 2), (emGridle / 2)); //22
            mesh.Vertices.Add(-(emLength / 2), -(em2Width / 2), (emGridle / 2)); //23
            mesh.Vertices.Add(-(em2Length / 2), -(emWidth / 2), (emGridle / 2)); //24
            mesh.Vertices.Add((em2Length / 2), -(emWidth / 2), (emGridle / 2)); //25
            mesh.Vertices.Add((emLength / 2), -(em2Width / 2), (emGridle / 2)); //26

            mesh.Vertices.Add(-((emLength / 2) * 0.87), ((em2Width / 2) * 0.81), (emTop * 0.6) + (emGridle / 2)); //27
            mesh.Vertices.Add(-((emLength / 2) * 0.87), -((em2Width / 2) * 0.81), (emTop * 0.6) + (emGridle / 2)); //28
            mesh.Vertices.Add(-((em2Length / 2) * 0.9), -((emWidth / 2) * 0.81), (emTop * 0.6) + (emGridle / 2)); //29
            mesh.Vertices.Add(((em2Length / 2) * 0.9), -((emWidth / 2) * 0.81), (emTop * 0.6) + (emGridle / 2)); //30
            mesh.Vertices.Add(((emLength / 2) * 0.87), -((em2Width / 2) * 0.81), (emTop * 0.6) + (emGridle / 2)); //31

            mesh.Vertices.Add(-((emLength / 2) * 0.73), ((em2Width / 2) * 0.6), emTop + (emGridle / 2)); //32
            mesh.Vertices.Add(-((emLength / 2) * 0.73), -((em2Width / 2) * 0.6), emTop + (emGridle / 2)); //33
            mesh.Vertices.Add(-((em2Length / 2) * 0.79), -((emWidth / 2) * 0.6), emTop + (emGridle / 2)); //34
            mesh.Vertices.Add(((em2Length / 2) * 0.79), -((emWidth / 2) * 0.6), emTop + (emGridle / 2)); //35
            mesh.Vertices.Add(((emLength / 2) * 0.73), -((em2Width / 2) * 0.6), emTop + (emGridle / 2)); //36




            mesh.Faces.AddFace(0, 3, 2);
            mesh.Faces.AddFace(0, 4, 3);
            mesh.Faces.AddFace(0, 1, 5, 4);
            mesh.Faces.AddFace(1, 6, 5);

            mesh.Faces.AddFace(2, 3, 8, 7);
            mesh.Faces.AddFace(3, 4, 9, 8);
            mesh.Faces.AddFace(4, 5, 10, 9);
            mesh.Faces.AddFace(5, 6, 11, 10);

            mesh.Faces.AddFace(7, 8, 13, 12);
            mesh.Faces.AddFace(8, 9, 14, 13);
            mesh.Faces.AddFace(9, 10, 15, 14);
            mesh.Faces.AddFace(10, 11, 16, 15);

            mesh.Faces.AddFace(12, 13, 18, 17);
            mesh.Faces.AddFace(13, 14, 19, 18);
            mesh.Faces.AddFace(14, 15, 20, 19);
            mesh.Faces.AddFace(15, 16, 21, 20);

            mesh.Faces.AddFace(17, 18, 23, 22);
            mesh.Faces.AddFace(18, 19, 24, 23);
            mesh.Faces.AddFace(19, 20, 25, 24);
            mesh.Faces.AddFace(20, 21, 26, 25);

            mesh.Faces.AddFace(22, 23, 28, 27);
            mesh.Faces.AddFace(23, 24, 29, 28);
            mesh.Faces.AddFace(24, 25, 30, 29);
            mesh.Faces.AddFace(25, 26, 31, 30);

            mesh.Faces.AddFace(27, 28, 33, 32);
            mesh.Faces.AddFace(28, 29, 34, 33);
            mesh.Faces.AddFace(29, 30, 35, 34);
            mesh.Faces.AddFace(30, 31, 36, 35);

            mesh.Faces.AddFace(32, 33, 36);
            mesh.Faces.AddFace(33, 34, 35, 36);

            mesh.Unweld(0.001, false);



            Mesh meshAll = new Mesh();
            for (int i = 0; i < 2; i++)
            {
                meshAll.Append(mesh);
                Point3d center = new Point3d(0.0, 0.0, 0.0);
                Vector3d rotVec = new Vector3d(0.0, 0.0, 1.0);
                mesh.Rotate(Math.PI, rotVec, center);
            }
            meshAll.Compact();
            meshAll.Weld(0.001);

            doc.Objects.AddMesh(meshAll);

        }
        double emLength = 2.0;
        double emWidth = 1.4;
        double em2Length = 1.4;
        double em2Width = 0.75;
        double emHigth = 0.9;
        double emGridle = 0.03;
        double emTop = 0.21;
        double emLine = 0.65;
        double emFatt = 1.0;

        public static void PearGem(RhinoDoc doc, double pearLength, double pearWidth)
        {
            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();


            mesh.Vertices.Add(0.0, 0.0, -0.461); //0

            mesh.Vertices.Add(0.052, -0.125, -0.353); //1
            mesh.Vertices.Add(0.125, -0.052, -0.353); //2
            mesh.Vertices.Add(0.125, 0.079, -0.353); //3
            mesh.Vertices.Add(0.0, 0.275, -0.377); //4

            mesh.Vertices.Add(0.0, -0.5, -0.029); //5
            mesh.Vertices.Add(0.098, -0.49, -0.02); //6
            mesh.Vertices.Add(0.191, -0.462, -0.029); //7
            mesh.Vertices.Add(0.278, -0.416, -0.02); //8
            mesh.Vertices.Add(0.354, -0.354, -0.029); //9
            mesh.Vertices.Add(0.416, -0.278, -0.02); //10
            mesh.Vertices.Add(0.462, -0.191, -0.029); //11
            mesh.Vertices.Add(0.490, -0.098, -0.02); //12
            mesh.Vertices.Add(0.5, 0.0, -0.029); //13
            mesh.Vertices.Add(0.492, 0.155, -0.02); //14
            mesh.Vertices.Add(0.467, 0.309, -0.029); //15
            mesh.Vertices.Add(0.426, 0.459, -0.02); //16
            mesh.Vertices.Add(0.369, 0.604, -0.029); //17
            mesh.Vertices.Add(0.297, 0.742, -0.02); //18
            mesh.Vertices.Add(0.211, 0.872, -0.029); //19
            mesh.Vertices.Add(0.112, 0.992, -0.02); //20
            mesh.Vertices.Add(0.0, 1.1, -0.029); //21

            mesh.Vertices.Add(0.0, -0.5, 0.009); //22
            mesh.Vertices.Add(0.098, -0.49, 0.0); //23
            mesh.Vertices.Add(0.191, -0.462, 0.009); //24
            mesh.Vertices.Add(0.278, -0.416, 0.0); //25
            mesh.Vertices.Add(0.354, -0.354, 0.009); //26
            mesh.Vertices.Add(0.416, -0.278, 0.0); //27
            mesh.Vertices.Add(0.462, -0.191, 0.009); //28
            mesh.Vertices.Add(0.490, -0.098, 0.0); //29
            mesh.Vertices.Add(0.5, 0.0, 0.009); //30
            mesh.Vertices.Add(0.492, 0.155, 0.0); //31
            mesh.Vertices.Add(0.467, 0.309, 0.009); //32
            mesh.Vertices.Add(0.426, 0.459, 0.0); //33
            mesh.Vertices.Add(0.369, 0.604, 0.009); //34
            mesh.Vertices.Add(0.297, 0.742, 0.0); //35
            mesh.Vertices.Add(0.211, 0.872, 0.009); //36
            mesh.Vertices.Add(0.112, 0.992, 0.0); //37
            mesh.Vertices.Add(0.0, 1.1, 0.009); //38

            mesh.Vertices.Add(0.15, -0.362, 0.105); //39
            mesh.Vertices.Add(0.362, -0.15, 0.105); //40
            mesh.Vertices.Add(0.362, 0.229, 0.105); //41
            mesh.Vertices.Add(0.104, 0.797, 0.105); //42

            mesh.Vertices.Add(0.0, -0.3, 0.148); //43
            mesh.Vertices.Add(0.212, -0.212, 0.148); //44
            mesh.Vertices.Add(0.3, 0.0, 0.148); //45
            mesh.Vertices.Add(0.212, 0.362, 0.148); //46
            mesh.Vertices.Add(0.0, 0.66, 0.148); //47
            mesh.Vertices.Add(0.0, 0.0, 0.148); //48

            mesh.Vertices.Add(-0.104, 0.797, 0.105); //49

            mesh.Faces.AddFace(0, 1, 5);
            mesh.Faces.AddFace(1, 7, 6, 5);
            mesh.Faces.AddFace(1, 9, 8, 7);
            mesh.Faces.AddFace(0, 2, 9, 1);
            mesh.Faces.AddFace(2, 11, 10, 9);
            mesh.Faces.AddFace(2, 13, 12, 11);
            mesh.Faces.AddFace(0, 3, 13, 2);
            mesh.Faces.AddFace(3, 15, 14, 13);
            mesh.Faces.AddFace(3, 17, 16, 15);
            mesh.Faces.AddFace(0, 4, 17, 3);
            mesh.Faces.AddFace(4, 19, 18, 17);
            mesh.Faces.AddFace(4, 21, 20, 19);

            mesh.Faces.AddFace(5, 6, 23, 22);
            mesh.Faces.AddFace(6, 7, 24, 23);
            mesh.Faces.AddFace(7, 8, 25, 24);
            mesh.Faces.AddFace(8, 9, 26, 25);
            mesh.Faces.AddFace(9, 10, 27, 26);
            mesh.Faces.AddFace(10, 11, 28, 27);
            mesh.Faces.AddFace(11, 12, 29, 28);
            mesh.Faces.AddFace(12, 13, 30, 29);
            mesh.Faces.AddFace(13, 14, 31, 30);
            mesh.Faces.AddFace(14, 15, 32, 31);
            mesh.Faces.AddFace(15, 16, 33, 32);
            mesh.Faces.AddFace(16, 17, 34, 33);
            mesh.Faces.AddFace(17, 18, 35, 34);
            mesh.Faces.AddFace(18, 19, 36, 35);
            mesh.Faces.AddFace(19, 20, 37, 36);
            mesh.Faces.AddFace(20, 21, 38, 37);

            mesh.Faces.AddFace(22, 39, 43);
            mesh.Faces.AddFace(22, 23, 24, 39);
            mesh.Faces.AddFace(24, 25, 26, 39);
            mesh.Faces.AddFace(26, 40, 44, 39);
            mesh.Faces.AddFace(26, 27, 28, 40);
            mesh.Faces.AddFace(28, 29, 30, 40);
            mesh.Faces.AddFace(30, 41, 45, 40);
            mesh.Faces.AddFace(30, 31, 32, 41);
            mesh.Faces.AddFace(32, 33, 34, 41);
            mesh.Faces.AddFace(34, 42, 46, 41);
            mesh.Faces.AddFace(34, 35, 36, 42);
            mesh.Faces.AddFace(36, 37, 38, 42);
            mesh.Faces.AddFace(39, 44, 43);
            mesh.Faces.AddFace(40, 45, 44);
            mesh.Faces.AddFace(41, 46, 45);
            mesh.Faces.AddFace(46, 42, 47);
            mesh.Faces.AddFace(42, 38, 47);

            mesh.Faces.AddFace(43, 44, 45, 48);
            mesh.Faces.AddFace(45, 46, 47, 48);

            mesh.Unweld(0.001, false);

            Mesh meshAll = new Mesh();
            meshAll.Append(mesh);

            Point3d origin = new Point3d(0.0, 0.0, 0.0);
            Vector3d vtm = new Vector3d(1.0, 0.0, 0.0);
            Plane plm = new Plane(origin, vtm);
            Transform mirror1 = new Transform();
            mirror1 = Transform.Mirror(plm);
            mesh.Transform(mirror1);
            meshAll.Append(mesh);

            meshAll.UnifyNormals();
            meshAll.Compact();
            meshAll.Weld(0.001);

            Vector3d vt1 = new Vector3d(0.0, 0.0, 1.0);
            Plane pl0 = new Plane(origin, vt1);
            Transform scale3D = new Transform();
            scale3D = Transform.Scale(pl0, pearWidth, (pearLength / 1.6), pearWidth);
            meshAll.Transform(scale3D);

            doc.Objects.AddMesh(meshAll);

        }
        double pearLength = 1.6;
        double pearWidth = 1.0;

        public static void NavGem(RhinoDoc doc, double navLength, double navWidth)
        {
            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();


            mesh.Vertices.Add(0.0, 0.0, -0.461); //0

            mesh.Vertices.Add(0.052, -0.125, -0.353); //1
            mesh.Vertices.Add(0.125, -0.052, -0.353); //2
            mesh.Vertices.Add(0.125, 0.079, -0.353); //3
            mesh.Vertices.Add(0.0, 0.275, -0.377); //4

            mesh.Vertices.Add(0.0, -0.5, -0.029); //5
            mesh.Vertices.Add(0.098, -0.49, -0.02); //6
            mesh.Vertices.Add(0.191, -0.462, -0.029); //7
            mesh.Vertices.Add(0.278, -0.416, -0.02); //8
            mesh.Vertices.Add(0.354, -0.354, -0.029); //9
            mesh.Vertices.Add(0.416, -0.278, -0.02); //10
            mesh.Vertices.Add(0.462, -0.191, -0.029); //11
            mesh.Vertices.Add(0.490, -0.098, -0.02); //12
            mesh.Vertices.Add(0.5, 0.0, -0.029); //13
            mesh.Vertices.Add(0.492, 0.155, -0.02); //14
            mesh.Vertices.Add(0.467, 0.309, -0.029); //15
            mesh.Vertices.Add(0.426, 0.459, -0.02); //16
            mesh.Vertices.Add(0.369, 0.604, -0.029); //17
            mesh.Vertices.Add(0.297, 0.742, -0.02); //18
            mesh.Vertices.Add(0.211, 0.872, -0.029); //19
            mesh.Vertices.Add(0.112, 0.992, -0.02); //20
            mesh.Vertices.Add(0.0, 1.1, -0.029); //21

            mesh.Vertices.Add(0.0, -0.5, 0.009); //22
            mesh.Vertices.Add(0.098, -0.49, 0.0); //23
            mesh.Vertices.Add(0.191, -0.462, 0.009); //24
            mesh.Vertices.Add(0.278, -0.416, 0.0); //25
            mesh.Vertices.Add(0.354, -0.354, 0.009); //26
            mesh.Vertices.Add(0.416, -0.278, 0.0); //27
            mesh.Vertices.Add(0.462, -0.191, 0.009); //28
            mesh.Vertices.Add(0.490, -0.098, 0.0); //29
            mesh.Vertices.Add(0.5, 0.0, 0.009); //30
            mesh.Vertices.Add(0.492, 0.155, 0.0); //31
            mesh.Vertices.Add(0.467, 0.309, 0.009); //32
            mesh.Vertices.Add(0.426, 0.459, 0.0); //33
            mesh.Vertices.Add(0.369, 0.604, 0.009); //34
            mesh.Vertices.Add(0.297, 0.742, 0.0); //35
            mesh.Vertices.Add(0.211, 0.872, 0.009); //36
            mesh.Vertices.Add(0.112, 0.992, 0.0); //37
            mesh.Vertices.Add(0.0, 1.1, 0.009); //38

            mesh.Vertices.Add(0.15, -0.362, 0.105); //39
            mesh.Vertices.Add(0.362, -0.15, 0.105); //40
            mesh.Vertices.Add(0.362, 0.229, 0.105); //41
            mesh.Vertices.Add(0.104, 0.797, 0.105); //42

            mesh.Vertices.Add(0.0, -0.3, 0.148); //43
            mesh.Vertices.Add(0.212, -0.212, 0.148); //44
            mesh.Vertices.Add(0.3, 0.0, 0.148); //45
            mesh.Vertices.Add(0.212, 0.362, 0.148); //46
            mesh.Vertices.Add(0.0, 0.66, 0.148); //47
            mesh.Vertices.Add(0.0, 0.0, 0.148); //48


            mesh.Faces.AddFace(0, 3, 13);
            mesh.Faces.AddFace(3, 15, 14, 13);
            mesh.Faces.AddFace(3, 17, 16, 15);
            mesh.Faces.AddFace(0, 4, 17, 3);
            mesh.Faces.AddFace(4, 19, 18, 17);
            mesh.Faces.AddFace(4, 21, 20, 19);


            mesh.Faces.AddFace(13, 14, 31, 30);
            mesh.Faces.AddFace(14, 15, 32, 31);
            mesh.Faces.AddFace(15, 16, 33, 32);
            mesh.Faces.AddFace(16, 17, 34, 33);
            mesh.Faces.AddFace(17, 18, 35, 34);
            mesh.Faces.AddFace(18, 19, 36, 35);
            mesh.Faces.AddFace(19, 20, 37, 36);
            mesh.Faces.AddFace(20, 21, 38, 37);


            mesh.Faces.AddFace(30, 41, 45);
            mesh.Faces.AddFace(30, 31, 32, 41);
            mesh.Faces.AddFace(32, 33, 34, 41);
            mesh.Faces.AddFace(34, 42, 46, 41);
            mesh.Faces.AddFace(34, 35, 36, 42);
            mesh.Faces.AddFace(36, 37, 38, 42);

            mesh.Faces.AddFace(41, 46, 45);
            mesh.Faces.AddFace(46, 42, 47);
            mesh.Faces.AddFace(42, 38, 47);

            mesh.Faces.AddFace(45, 46, 47, 48);

            mesh.Unweld(0.001, false);

            Mesh meshAll = new Mesh();
            Mesh meshAll2 = new Mesh();
            meshAll.Append(mesh);

            Point3d origin = new Point3d(0.0, 0.0, 0.0);
            Vector3d vtmY = new Vector3d(1.0, 0.0, 0.0);
            Plane plmY = new Plane(origin, vtmY);
            Transform mirrorY = new Transform();
            mirrorY = Transform.Mirror(plmY);
            mesh.Transform(mirrorY);
            meshAll.Append(mesh);
            meshAll2.Append(meshAll);

            Vector3d vtmX = new Vector3d(0.0, 1.0, 0.0);
            Plane plmX = new Plane(origin, vtmX);
            Transform mirrorX = new Transform();
            mirrorX = Transform.Mirror(plmX);
            meshAll.Transform(mirrorX);
            meshAll2.Append(meshAll);

            meshAll2.UnifyNormals();
            meshAll2.Compact();
            meshAll2.Weld(0.001);


            Vector3d vt1 = new Vector3d(0.0, 0.0, 1.0);
            Plane pl0 = new Plane(origin, vt1);
            Transform scale3D = new Transform();
            scale3D = Transform.Scale(pl0, navWidth, (navLength / 2.2), navWidth);
            meshAll2.Transform(scale3D);

            doc.Objects.AddMesh(meshAll2);
        }
        double navLength = 2.2;
        double navWidth = 1.0;

        public static void PrincessGem(RhinoDoc doc, double princeLength)
        {
            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();


            mesh.Vertices.Add(0.0, 0.0, -0.584); //0

            mesh.Vertices.Add(-0.05, 0.0, -0.556); //1
            mesh.Vertices.Add(-0.0, -0.05, -0.556); //2

            mesh.Vertices.Add(0.0, -0.15, -0.491); //3
            mesh.Vertices.Add(0.0, -0.25, -0.421); //4

            mesh.Vertices.Add(-0.5, -0.5, -0.013); //5
            mesh.Vertices.Add(0.5, -0.5, -0.013); //6

            mesh.Vertices.Add(-0.5, -0.5, 0.013); //7
            mesh.Vertices.Add(0.5, -0.5, 0.013); //8

            mesh.Vertices.Add(-0.412, -0.36, 0.086); //9
            mesh.Vertices.Add(-0.36, -0.413, 0.086); //10
            mesh.Vertices.Add(0.36, -0.413, 0.086); //11

            mesh.Vertices.Add(-0.294, -0.294, 0.146); //12
            mesh.Vertices.Add(0.0, -0.325, 0.146); //13
            mesh.Vertices.Add(0.294, -0.294, 0.146); //14
            mesh.Vertices.Add(-0.325, 0.0, 0.146); //15
            mesh.Vertices.Add(0.0, 0.0, 0.146); //16

            mesh.Vertices.Add(-0.15, 0.0, -0.491); //17
            mesh.Vertices.Add(-0.25, 0.0, -0.421); //18


            mesh.Faces.AddFace(0, 2, 5, 1);
            mesh.Faces.AddFace(2, 3, 5);
            mesh.Faces.AddFace(3, 4, 5);
            mesh.Faces.AddFace(4, 6, 5);
            mesh.Faces.AddFace(1, 5, 17);
            mesh.Faces.AddFace(17, 5, 18);

            mesh.Faces.AddFace(5, 6, 8, 7);
            mesh.Faces.AddFace(7, 8, 11, 10);

            mesh.Faces.AddFace(7, 10, 12);
            mesh.Faces.AddFace(8, 14, 11);
            mesh.Faces.AddFace(10, 13, 12);
            mesh.Faces.AddFace(10, 11, 13);
            mesh.Faces.AddFace(11, 14, 13);

            mesh.Faces.AddFace(12, 13, 16, 15);

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
            meshAll.Scale(princeLength);
            doc.Objects.AddMesh(meshAll);
        }
        double princeLength = 1.0;

        public static void TrapGem(RhinoDoc doc, double tL, double tW1, double tW2)
        {
            double br1 = tW2 / tW1;
            double br2 = br1 + ((1 - br1) / 2);

            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();

            mesh.Vertices.Add(-0.188, 0.0, -0.293 * br2); //0
            mesh.Vertices.Add(0.188, 0.0, -0.293); //1

            mesh.Vertices.Add(-0.25, 0.1 * br1, -0.247 * br2); //2
            mesh.Vertices.Add(-0.25, -0.1 * br1, -0.247 * br2); //3
            mesh.Vertices.Add(0.25, -0.1, -0.247); //4
            mesh.Vertices.Add(0.25, 0.1, -0.247); //5

            mesh.Vertices.Add(-0.438, 0.4 * br1, -0.082 * br2); //6
            mesh.Vertices.Add(-0.438, -0.4 * br1, -0.082 * br2); //7
            mesh.Vertices.Add(0.438, -0.4, -0.082); //8
            mesh.Vertices.Add(0.438, 0.4, -0.082); //9

            mesh.Vertices.Add(-0.5, 0.5 * br1, -0.006); //10
            mesh.Vertices.Add(-0.5, -0.5 * br1, -0.006); //11
            mesh.Vertices.Add(0.5, -0.5, -0.006); //12
            mesh.Vertices.Add(0.5, 0.5, -0.006); //13

            mesh.Vertices.Add(-0.5, 0.5 * br1, 0.006); //14
            mesh.Vertices.Add(-0.5, -0.5 * br1, 0.006); //15
            mesh.Vertices.Add(0.5, -0.5, 0.006); //16
            mesh.Vertices.Add(0.5, 0.5, 0.006); //17

            mesh.Vertices.Add(-0.438, 0.4 * br1, 0.06 * br2); //18
            mesh.Vertices.Add(-0.438, -0.4 * br1, 0.06 * br2); //19
            mesh.Vertices.Add(0.438, -0.4, 0.06); //20
            mesh.Vertices.Add(0.438, 0.4, 0.06); //21

            mesh.Vertices.Add(-0.375, 0.3 * br1, 0.088 * br2); //22
            mesh.Vertices.Add(-0.375, -0.3 * br1, 0.088 * br2); //23
            mesh.Vertices.Add(0.375, -0.3, 0.088); //24
            mesh.Vertices.Add(0.375, 0.3, 0.088); //25



            mesh.Faces.AddFace(0, 3, 2);
            mesh.Faces.AddFace(0, 1, 4, 3);
            mesh.Faces.AddFace(1, 5, 4);
            mesh.Faces.AddFace(0, 2, 5, 1);

            mesh.Faces.AddFace(2, 3, 7, 6);
            mesh.Faces.AddFace(3, 4, 8, 7);
            mesh.Faces.AddFace(4, 5, 9, 8);
            mesh.Faces.AddFace(5, 2, 6, 9);

            mesh.Faces.AddFace(6, 7, 11, 10);
            mesh.Faces.AddFace(7, 8, 12, 11);
            mesh.Faces.AddFace(8, 9, 13, 12);
            mesh.Faces.AddFace(9, 6, 10, 13);

            mesh.Faces.AddFace(10, 11, 15, 14);
            mesh.Faces.AddFace(11, 12, 16, 15);
            mesh.Faces.AddFace(12, 13, 17, 16);
            mesh.Faces.AddFace(13, 10, 14, 17);

            mesh.Faces.AddFace(14, 15, 19, 18);
            mesh.Faces.AddFace(15, 16, 20, 19);
            mesh.Faces.AddFace(16, 17, 21, 20);
            mesh.Faces.AddFace(17, 14, 18, 21);

            mesh.Faces.AddFace(18, 19, 23, 22);
            mesh.Faces.AddFace(19, 20, 24, 23);
            mesh.Faces.AddFace(20, 21, 25, 24);
            mesh.Faces.AddFace(21, 18, 22, 25);

            mesh.Faces.AddFace(22, 23, 24, 25);

            mesh.Unweld(0.001, false);

            Point3d origin = new Point3d(0.0, 0.0, 0.0);
            Vector3d vt1 = new Vector3d(0.0, 0.0, 1.0);
            Plane pl0 = new Plane(origin, vt1);
            Transform scale1D = new Transform();
            scale1D = Transform.Scale(pl0, tL, tW1, tL);
            mesh.Transform(scale1D);
            mesh.Rotate(Math.PI / 2, vt1, origin);
            mesh.RebuildNormals();
            mesh.Compact();
            mesh.Weld(0.001);


            doc.Objects.AddMesh(mesh);
        }
        double tL = 1.0;
        double tW1 = 0.62;
        double tW2 = 0.4;

        public static void TrilGem(RhinoDoc doc, double trilLength)
        {
            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();


            mesh.Vertices.Add(0.0, 0.0, -0.4); //0

            mesh.Vertices.Add(0.0, -0.423, -0.01); //1
            mesh.Vertices.Add(0.292, -0.38, -0.01); //2
            mesh.Vertices.Add(0.50, -0.289, -0.01); //3
            mesh.Vertices.Add(0.476, -0.07, -0.01); //4
            mesh.Vertices.Add(0.366, 0.211, -0.01); //5
            mesh.Vertices.Add(0.179, 0.444, -0.01); //6
            mesh.Vertices.Add(0.0, 0.577, -0.01); //7

            mesh.Vertices.Add(0.0, -0.423, 0.01); //8
            mesh.Vertices.Add(0.292, -0.38, 0.01); //9
            mesh.Vertices.Add(0.50, -0.289, 0.01); //10
            mesh.Vertices.Add(0.476, -0.07, 0.01); //11
            mesh.Vertices.Add(0.366, 0.211, 0.01); //12
            mesh.Vertices.Add(0.179, 0.444, 0.01); //13
            mesh.Vertices.Add(0.0, 0.577, 0.01); //14

            mesh.Vertices.Add(0.095, -0.259, 0.13); //15
            mesh.Vertices.Add(0.294, -0.170, 0.13); //16
            mesh.Vertices.Add(0.27, 0.045, 0.13); //17
            mesh.Vertices.Add(0.175, 0.209, 0.13); //18
            mesh.Vertices.Add(0.0, 0.34, 0.13); //19
            mesh.Vertices.Add(0.0, 0.0, 0.13); //20
            mesh.Vertices.Add(-0.0, -0.259, 0.13); //21


            mesh.Faces.AddFace(0, 2, 1);
            mesh.Faces.AddFace(0, 3, 2);
            mesh.Faces.AddFace(0, 4, 3);
            mesh.Faces.AddFace(0, 5, 4);
            mesh.Faces.AddFace(0, 6, 5);
            mesh.Faces.AddFace(0, 7, 6);
            mesh.Faces.AddFace(1, 2, 9, 8);
            mesh.Faces.AddFace(2, 3, 10, 9);
            mesh.Faces.AddFace(3, 4, 11, 10);
            mesh.Faces.AddFace(4, 5, 12, 11);
            mesh.Faces.AddFace(5, 6, 13, 12);
            mesh.Faces.AddFace(6, 7, 14, 13);

            mesh.Faces.AddFace(8, 9, 15);
            mesh.Faces.AddFace(9, 10, 16, 15);
            mesh.Faces.AddFace(10, 11, 17, 16);
            mesh.Faces.AddFace(11, 12, 17);
            mesh.Faces.AddFace(12, 18, 17);
            mesh.Faces.AddFace(12, 13, 18);
            mesh.Faces.AddFace(13, 14, 19, 18);
            mesh.Faces.AddFace(15, 16, 17, 20);
            mesh.Faces.AddFace(17, 18, 19, 20);

            mesh.Faces.AddFace(8, 15, 21);
            mesh.Faces.AddFace(15, 20, 21);

            mesh.Unweld(0.001, false);

            Mesh meshAll = new Mesh();
            meshAll.Append(mesh);

            Point3d origin = new Point3d(0.0, 0.0, 0.0);
            Vector3d vtm = new Vector3d(1.0, 0.0, 0.0);
            Plane plm = new Plane(origin, vtm);
            Transform mirror1 = new Transform();
            mirror1 = Transform.Mirror(plm);
            mesh.Transform(mirror1);
            meshAll.Append(mesh);

            meshAll.UnifyNormals();
            meshAll.Compact();
            meshAll.Weld(0.001);

            meshAll.Scale(trilLength);

            doc.Objects.AddMesh(meshAll);

        }
        double trilLength = 1.0;

        public static void CushGem(RhinoDoc doc, double cushLength, double cushWidth)
        {
            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();


            mesh.Vertices.Add(0.0, 0.0, -0.444); //0

            mesh.Vertices.Add(-0.142, 0.0, -0.366); //1
            mesh.Vertices.Add(0.0, -0.142, -0.366); //2

            mesh.Vertices.Add(-0.5, 0.0, -0.018); //3
            mesh.Vertices.Add(-0.495, -0.14, -0.006); //4
            mesh.Vertices.Add(-0.47, -0.24, -0.018); //5
            mesh.Vertices.Add(-0.441, -0.317, -0.007); //6
            mesh.Vertices.Add(-0.387, -0.387, -0.018); //7
            mesh.Vertices.Add(-0.317, -0.441, -0.007); //8
            mesh.Vertices.Add(-0.24, -0.47, -0.018); //9
            mesh.Vertices.Add(-0.14, -0.495, -0.006); //10
            mesh.Vertices.Add(0.0, -0.5, -0.018); //11

            mesh.Vertices.Add(-0.5, 0.0, 0.014); //12
            mesh.Vertices.Add(-0.495, -0.14, 0.006); //13
            mesh.Vertices.Add(-0.47, -0.24, 0.014); //14
            mesh.Vertices.Add(-0.441, -0.317, 0.006); //15
            mesh.Vertices.Add(-0.387, -0.387, 0.014); //16
            mesh.Vertices.Add(-0.317, -0.441, 0.006); //17
            mesh.Vertices.Add(-0.24, -0.47, 0.014); //18
            mesh.Vertices.Add(-0.14, -0.495, 0.006); //19
            mesh.Vertices.Add(0.0, -0.5, 0.014); //20

            mesh.Vertices.Add(-0.321, -0.188, 0.12); //21
            mesh.Vertices.Add(-0.188, -0.321, 0.12); //22
            mesh.Vertices.Add(0.188, -0.321, 0.12); //23
            mesh.Vertices.Add(-0.254, 0.0, 0.16); //24
            mesh.Vertices.Add(-0.205, -0.205, 0.16); //25
            mesh.Vertices.Add(0.0, -0.254, 0.16); //26
            mesh.Vertices.Add(0.0, 0.0, 0.16); //27


            mesh.Faces.AddFace(0, 2, 7, 1);
            mesh.Faces.AddFace(1, 5, 4, 3);
            mesh.Faces.AddFace(1, 7, 6, 5);
            mesh.Faces.AddFace(2, 9, 8, 7);
            mesh.Faces.AddFace(2, 11, 10, 9);

            mesh.Faces.AddFace(3, 4, 13, 12);
            mesh.Faces.AddFace(4, 5, 14, 13);
            mesh.Faces.AddFace(5, 6, 15, 14);
            mesh.Faces.AddFace(6, 7, 16, 15);
            mesh.Faces.AddFace(7, 8, 17, 16);
            mesh.Faces.AddFace(8, 9, 18, 17);
            mesh.Faces.AddFace(9, 10, 19, 18);
            mesh.Faces.AddFace(10, 11, 20, 19);

            mesh.Faces.AddFace(12, 13, 14, 21);
            mesh.Faces.AddFace(14, 15, 16, 21);
            mesh.Faces.AddFace(16, 22, 25, 21);
            mesh.Faces.AddFace(16, 17, 18, 22);
            mesh.Faces.AddFace(18, 19, 20, 22);
            mesh.Faces.AddFace(22, 20, 23, 26);
            mesh.Faces.AddFace(21, 25, 24);
            mesh.Faces.AddFace(22, 26, 25);
            mesh.Faces.AddFace(24, 25, 26, 27);



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

            Point3d origin = new Point3d(0.0, 0.0, 0.0);
            Vector3d vt1 = new Vector3d(0.0, 0.0, 1.0);
            Plane pl0 = new Plane(origin, vt1);
            Transform scale1D = new Transform();
            scale1D = Transform.Scale(pl0, cushLength, cushWidth, cushWidth);
            meshAll.Transform(scale1D);
            doc.Objects.AddMesh(meshAll);
        }
        double cushLength = 1.5;
        double cushWidth = 1.0;

        public static void AntGem(RhinoDoc doc, double antLength, double antWidth)
        {
            Rhino.Geometry.Mesh mesh = new Rhino.Geometry.Mesh();


            mesh.Vertices.Add(0.0, 0.0, -0.44); //0

            mesh.Vertices.Add(-0.115, 0.0, -0.382); //1
            mesh.Vertices.Add(0.0, -0.115, -0.382); //2

            mesh.Vertices.Add(-0.5, 0.0, -0.014); //3
            mesh.Vertices.Add(-0.491, -0.14, -0.008); //4
            mesh.Vertices.Add(-0.473, -0.245, -0.014); //5
            mesh.Vertices.Add(-0.451, -0.328, -0.011); //6
            mesh.Vertices.Add(-0.419, -0.419, -0.014); //7
            mesh.Vertices.Add(-0.328, -0.451, -0.011); //8
            mesh.Vertices.Add(-0.245, -0.473, -0.014); //9
            mesh.Vertices.Add(-0.14, -0.491, -0.008); //10
            mesh.Vertices.Add(0.0, -0.5, -0.014); //11

            mesh.Vertices.Add(-0.5, 0.0, 0.014); //12
            mesh.Vertices.Add(-0.491, -0.14, 0.009); //13
            mesh.Vertices.Add(-0.473, -0.245, 0.014); //14
            mesh.Vertices.Add(-0.451, -0.328, 0.011); //15
            mesh.Vertices.Add(-0.419, -0.419, 0.014); //16
            mesh.Vertices.Add(-0.328, -0.451, 0.011); //17
            mesh.Vertices.Add(-0.245, -0.473, 0.014); //18
            mesh.Vertices.Add(-0.14, -0.491, 0.009); //19
            mesh.Vertices.Add(0.0, -0.5, 0.014); //20

            mesh.Vertices.Add(-0.357, -0.241, 0.109); //21
            mesh.Vertices.Add(-0.241, -0.357, 0.109); //22
            mesh.Vertices.Add(0.241, -0.357, 0.109); //23
            mesh.Vertices.Add(-0.28, 0.0, 0.16); //24
            mesh.Vertices.Add(-0.234, -0.234, 0.16); //25
            mesh.Vertices.Add(0.0, -0.28, 0.16); //26
            mesh.Vertices.Add(0.0, 0.0, 0.16); //27


            mesh.Faces.AddFace(0, 2, 7, 1);
            mesh.Faces.AddFace(1, 5, 4, 3);
            mesh.Faces.AddFace(1, 7, 6, 5);
            mesh.Faces.AddFace(2, 9, 8, 7);
            mesh.Faces.AddFace(2, 11, 10, 9);

            mesh.Faces.AddFace(3, 4, 13, 12);
            mesh.Faces.AddFace(4, 5, 14, 13);
            mesh.Faces.AddFace(5, 6, 15, 14);
            mesh.Faces.AddFace(6, 7, 16, 15);
            mesh.Faces.AddFace(7, 8, 17, 16);
            mesh.Faces.AddFace(8, 9, 18, 17);
            mesh.Faces.AddFace(9, 10, 19, 18);
            mesh.Faces.AddFace(10, 11, 20, 19);

            mesh.Faces.AddFace(12, 13, 14, 21);
            mesh.Faces.AddFace(14, 15, 16, 21);
            mesh.Faces.AddFace(16, 22, 25, 21);
            mesh.Faces.AddFace(16, 17, 18, 22);
            mesh.Faces.AddFace(18, 19, 20, 22);
            mesh.Faces.AddFace(22, 20, 23, 26);
            mesh.Faces.AddFace(21, 25, 24);
            mesh.Faces.AddFace(22, 26, 25);
            mesh.Faces.AddFace(24, 25, 26, 27);



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

            Point3d origin = new Point3d(0.0, 0.0, 0.0);
            Vector3d vt1 = new Vector3d(0.0, 0.0, 1.0);
            Plane pl0 = new Plane(origin, vt1);
            Transform scale1D = new Transform();
            scale1D = Transform.Scale(pl0, antLength, antWidth, antWidth);
            meshAll.Transform(scale1D);
            doc.Objects.AddMesh(meshAll);
        }
        double antLength = 1.5;
        double antWidth = 1.0;

        Boolean brillantCut = false;
        Boolean ovalCut = false;
        Boolean baguetteCut = false;
        Boolean princessCut = false;
        Boolean emeraldCut = false;
        Boolean pearCut = false;
        Boolean navCut = false;
        Boolean trapCut = false;
        Boolean trilCut = false;
        Boolean cushCut = false;
        Boolean antCut = false;

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            GetOption selectCut = new GetOption();
            selectCut.SetCommandPrompt("Please choose stone cut.");
            var cutBrill = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Brilliant", ref cutBrill);
            var cutOval = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Oval", ref cutOval);
            var cutBag = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Baguette", ref cutBag);
            var cutPrince = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Princess", ref cutPrince);
            var cutEmerald = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Emerald", ref cutEmerald);
            var cutPear = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Pear", ref cutPear);
            var cutNav = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Navette", ref cutNav);
            var cutTrap = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Trapeze", ref cutTrap);
            var cutTril = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Trillion", ref cutTril);
            var cutCush = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Cushion", ref cutCush);
            var cutAnt = new OptionToggle(false, "False", "True");
            selectCut.AddOptionToggle("Antique", ref cutAnt);

            selectCut.Get();


            brillantCut = cutBrill.CurrentValue;
            ovalCut = cutOval.CurrentValue;
            baguetteCut = cutBag.CurrentValue;
            princessCut = cutPrince.CurrentValue;
            emeraldCut = cutEmerald.CurrentValue;
            pearCut = cutPear.CurrentValue;
            navCut = cutNav.CurrentValue;
            trapCut = cutTrap.CurrentValue;
            trilCut = cutTril.CurrentValue;
            cushCut = cutCush.CurrentValue;
            antCut = cutAnt.CurrentValue;



            if (brillantCut)
            {
                GetOption stoneDiam = new GetOption();
                stoneDiam.SetCommandPrompt("Please choose brilliant diameter.");
                var diamBrill = new Rhino.Input.Custom.OptionDouble(brillDiam);
                stoneDiam.AddOptionDouble("Diameter", ref diamBrill);
                stoneDiam.Get();
                brillDiam = diamBrill.CurrentValue;
                BrillantGem(doc, brillDiam);
            }

            if (ovalCut)
            {
                while (true)
                {
                    GetOption stoneOval = new GetOption();
                    stoneOval.SetCommandPrompt("Please choose oval cut lenght and width.");
                    stoneOval.AcceptNothing(true);
                    var lengthOval = new Rhino.Input.Custom.OptionDouble(ovalLength);
                    stoneOval.AddOptionDouble("Length", ref lengthOval);
                    var widthOval = new Rhino.Input.Custom.OptionDouble(ovalWidth);
                    stoneOval.AddOptionDouble("Width", ref widthOval);
                    GetResult res = stoneOval.Get();
                    ovalLength = lengthOval.CurrentValue;
                    ovalWidth = widthOval.CurrentValue;
                    if (res == GetResult.Nothing) break;
                }

                OvalGem(doc, ovalLength, ovalWidth);
            }

            if (baguetteCut)
            {
                while (true)
                {
                    GetOption stoneBag = new GetOption();
                    stoneBag.SetCommandPrompt("Please choose baguette cut lenght and width.");
                    stoneBag.AcceptNothing(true);
                    var lengthBag = new Rhino.Input.Custom.OptionDouble(bagLength);
                    stoneBag.AddOptionDouble("Length", ref lengthBag);
                    var widthBag = new Rhino.Input.Custom.OptionDouble(bagWidth);
                    stoneBag.AddOptionDouble("Width", ref widthBag);
                    GetResult res = stoneBag.Get();
                    bagLength = lengthBag.CurrentValue;
                    bagWidth = widthBag.CurrentValue;
                    if (res == GetResult.Nothing) break;
                }

                BaguetteGem(doc, bagLength, bagWidth);
            }

            if (princessCut)
            {
                GetOption stonePrince = new GetOption();
                stonePrince.SetCommandPrompt("Please choose princess cut length.");
                var lengthPrince = new Rhino.Input.Custom.OptionDouble(princeLength);
                stonePrince.AddOptionDouble("Length", ref lengthPrince);
                stonePrince.Get();
                princeLength = lengthPrince.CurrentValue;
                PrincessGem(doc, princeLength);
            }

            if (emeraldCut)
            {

                while (true)
                {
                    GetOption stoneEm = new GetOption();
                    stoneEm.SetCommandPrompt("Please choose emerald cut length and width.");
                    stoneEm.AcceptNothing(true);
                    var lengthEm = new Rhino.Input.Custom.OptionDouble(emLength);
                    stoneEm.AddOptionDouble("Length", ref lengthEm);
                    var widthEm = new Rhino.Input.Custom.OptionDouble(emWidth);
                    stoneEm.AddOptionDouble("Width", ref widthEm);
                    var length2Em = new Rhino.Input.Custom.OptionDouble(em2Length);
                    stoneEm.AddOptionDouble("Length2", ref length2Em);
                    var width2Em = new Rhino.Input.Custom.OptionDouble(em2Width);
                    stoneEm.AddOptionDouble("Width2", ref width2Em);
                    var higthEm = new Rhino.Input.Custom.OptionDouble(emHigth);
                    stoneEm.AddOptionDouble("Higth", ref higthEm);
                    var gridleEm = new Rhino.Input.Custom.OptionDouble(emGridle);
                    stoneEm.AddOptionDouble("Gridle", ref gridleEm);
                    var topEm = new Rhino.Input.Custom.OptionDouble(emTop);
                    stoneEm.AddOptionDouble("Top", ref topEm);
                    var lineEm = new Rhino.Input.Custom.OptionDouble(emLine);
                    stoneEm.AddOptionDouble("Line", ref lineEm);
                    var fattEm = new Rhino.Input.Custom.OptionDouble(emFatt);
                    stoneEm.AddOptionDouble("Fat", ref fattEm);
                    GetResult res = stoneEm.Get();

                    emLength = lengthEm.CurrentValue;
                    emWidth = widthEm.CurrentValue;
                    em2Width = width2Em.CurrentValue;
                    em2Length = length2Em.CurrentValue;
                    emHigth = higthEm.CurrentValue;
                    emGridle = gridleEm.CurrentValue;
                    emTop = topEm.CurrentValue;
                    emFatt = fattEm.CurrentValue;
                    emLine = lineEm.CurrentValue;

                    if (res == GetResult.Nothing) break;
                }
                EmeraldGem(doc, emWidth, emLength, em2Width, em2Length, emHigth, emGridle, emTop, emFatt, emLine);
            }

            if (pearCut)
            {
                while (true)
                {
                    GetOption stonePear = new GetOption();
                    stonePear.SetCommandPrompt("Please choose pear cut lenght and width.");
                    stonePear.AcceptNothing(true);
                    var lengthPear = new Rhino.Input.Custom.OptionDouble(pearLength);
                    stonePear.AddOptionDouble("Length", ref lengthPear);
                    var widthPear = new Rhino.Input.Custom.OptionDouble(pearWidth);
                    stonePear.AddOptionDouble("Width", ref widthPear);
                    GetResult res = stonePear.Get();
                    pearLength = lengthPear.CurrentValue;
                    pearWidth = widthPear.CurrentValue;
                    if (res == GetResult.Nothing) break;
                }

                PearGem(doc, pearLength, pearWidth);
            }

            if (navCut)
            {
                while (true)
                {
                    GetOption stoneNav = new GetOption();
                    stoneNav.SetCommandPrompt("Please choose navette cut lenght and width.");
                    stoneNav.AcceptNothing(true);
                    var lengthNav = new Rhino.Input.Custom.OptionDouble(navLength);
                    stoneNav.AddOptionDouble("Length", ref lengthNav);
                    var widthNav = new Rhino.Input.Custom.OptionDouble(navWidth);
                    stoneNav.AddOptionDouble("Width", ref widthNav);
                    GetResult res = stoneNav.Get();
                    navLength = lengthNav.CurrentValue;
                    navWidth = widthNav.CurrentValue;
                    if (res == GetResult.Nothing) break;
                }

                NavGem(doc, navLength, navWidth);
            }

            if (trapCut)
            {
                while (true)
                {
                    GetOption stoneNav = new GetOption();
                    stoneNav.SetCommandPrompt("Please choose trapeze cut lenght, width 1 and width 2.");
                    stoneNav.AcceptNothing(true);
                    var trapL = new Rhino.Input.Custom.OptionDouble(tL);
                    stoneNav.AddOptionDouble("Length", ref trapL);
                    var trapW1 = new Rhino.Input.Custom.OptionDouble(tW1);
                    stoneNav.AddOptionDouble("Width-1", ref trapW1);
                    var trapW2 = new Rhino.Input.Custom.OptionDouble(tW2);
                    stoneNav.AddOptionDouble("Width-2", ref trapW2);
                    GetResult res = stoneNav.Get();
                    tL = trapL.CurrentValue;
                    tW1 = trapW1.CurrentValue;
                    tW2 = trapW2.CurrentValue;
                    if (res == GetResult.Nothing) break;
                }

                TrapGem(doc, tL, tW1, tW2);
            }

            if (trilCut)
            {
                GetOption stoneTril = new GetOption();
                stoneTril.SetCommandPrompt("Please choose trillon length.");
                var lengthTril = new Rhino.Input.Custom.OptionDouble(trilLength);
                stoneTril.AddOptionDouble("Length", ref lengthTril);
                stoneTril.Get();
                trilLength = lengthTril.CurrentValue;
                TrilGem(doc, trilLength);
            }

            if (cushCut)
            {
                while (true)
                {
                    GetOption stoneCush = new GetOption();
                    stoneCush.SetCommandPrompt("Please choose oval cut lenght and width.");
                    stoneCush.AcceptNothing(true);
                    var lengthCush = new Rhino.Input.Custom.OptionDouble(cushLength);
                    stoneCush.AddOptionDouble("Length", ref lengthCush);
                    var widthCush = new Rhino.Input.Custom.OptionDouble(cushWidth);
                    stoneCush.AddOptionDouble("Width", ref widthCush);
                    GetResult res = stoneCush.Get();
                    cushLength = lengthCush.CurrentValue;
                    cushWidth = widthCush.CurrentValue;
                    if (res == GetResult.Nothing) break;
                }

                CushGem(doc, cushLength, cushWidth);
            }

            if (antCut)
            {
                while (true)
                {
                    GetOption stoneAnt = new GetOption();
                    stoneAnt.SetCommandPrompt("Please choose antique cut lenght and width.");
                    stoneAnt.AcceptNothing(true);
                    var lengthAnt = new Rhino.Input.Custom.OptionDouble(antLength);
                    stoneAnt.AddOptionDouble("Length", ref lengthAnt);
                    var widthAnt = new Rhino.Input.Custom.OptionDouble(antWidth);
                    stoneAnt.AddOptionDouble("Width", ref widthAnt);
                    GetResult res = stoneAnt.Get();
                    antLength = lengthAnt.CurrentValue;
                    antWidth = widthAnt.CurrentValue;
                    if (res == GetResult.Nothing) break;
                }

                AntGem(doc, antLength, antWidth);
            }

            doc.Views.Redraw();

            return Result.Success;
        }
    }
}
