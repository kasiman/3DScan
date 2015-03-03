using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using HelixToolkit;

namespace _3DScan
{
    class Model
    {
        Model3DGroup model;
        PointsVisual3D points = null;
        ModelVisual3D visualModel;
        HelixViewport3D view;
        double xMin;                        //   TEST: Abscissa of front plane
        double xMax;
        double yMin;
        double yMax;
        double zMin;
        double zMax;

        public Model(HelixViewport3D view, ModelVisual3D visualModel)
        {
            this.visualModel = visualModel;
            this.view = view;
        }

        public bool Load(string filePath)
        {
            try
            {
                ModelImporter importer = new ModelImporter();
                this.model = importer.Load(filePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Loading error!");
                return false;
            }
            return true;
        }

        public void ModelVisualisation(bool asPoly, bool asPoints, bool showPlane)
        {
            ClearView();

            if (asPoly && !asPoints)
            {
                toPoly();
            }
            else if (!asPoly && asPoints)
            {
                toPoint();
            }
            else if (asPoly && asPoints)
            {
                toPolyPoint();
            }


            if (showPlane)                                     //   TEST: Front plane visualisation
            {
                showPlanes(asPoly, asPoints);
            }
        }

        public Model3D LoadPoly()
        {
            return this.model;
        }

        public PointsVisual3D LoadPoints()
        {
            GeometryModel3D geom;
            MeshGeometry3D mesh;
            this.points = new PointsVisual3D();

            try
            {
                geom = (GeometryModel3D)this.model.Children[0];
                mesh = (MeshGeometry3D)geom.Geometry;

                xMin = mesh.Positions[0].X;                      //   TEST: Search a abscissa's front plane
                xMax = mesh.Positions[0].X;
                yMin = mesh.Positions[0].Y;
                xMax = mesh.Positions[0].Y;
                zMin = mesh.Positions[0].Z;
                xMax = mesh.Positions[0].Z;

                foreach (Point3D p in mesh.Positions)
                {
                    this.points.Points.Add(p);

                    if (p.X > xMax)                              //   TEST: Search a abscissa's front plane
                        xMax = p.X;                              //   TEST: Search a abscissa's front plane
                    if (p.X < xMin)                              //   TEST: Search a abscissa's front plane
                        xMin = p.X;                              //   TEST: Search a abscissa's front plane
                    if (p.Y > yMax)                              //   TEST: Search a abscissa's front plane
                        yMax = p.Y;                              //   TEST: Search a abscissa's front plane
                    if (p.Y < yMin)                              //   TEST: Search a abscissa's front plane
                        yMin = p.Y;                              //   TEST: Search a abscissa's front plane
                    if (p.Z > zMax)                              //   TEST: Search a abscissa's front plane
                        zMax = p.Z;                              //   TEST: Search a abscissa's front plane
                    if (p.Z < zMin)                              //   TEST: Search a abscissa's front plane
                        zMin = p.Z;                              //   TEST: Search a abscissa's front plane

                }
                this.points.Size = 2;
                this.points.Color = Colors.Green;
                return this.points;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Loading error!");
                return null;
            }
        }



        public PointsVisual3D frontPlane()                      //   TEST: Search all points of front plane
        {
            PointsVisual3D frontPoints = new PointsVisual3D();
            foreach (Point3D p in this.points.Points)
            {
                if (p.X == xMin)
                    frontPoints.Points.Add(p);
            }
            frontPoints.Color = Colors.Red;
            frontPoints.Size = 5;
            return frontPoints;
        }

        public PointsVisual3D backPlane()                      //   TEST: Search all points of front plane
        {
            PointsVisual3D backPoints = new PointsVisual3D();
            foreach (Point3D p in this.points.Points)
            {
                if (p.X == xMax)
                    backPoints.Points.Add(p);
            }
            backPoints.Color = Colors.Red;
            backPoints.Size = 5;
            return backPoints;
        }

        public PointsVisual3D leftPlane()                      //   TEST: Search all points of front plane
        {
            PointsVisual3D leftPoints = new PointsVisual3D();
            foreach (Point3D p in this.points.Points)
            {
                if (p.Y == yMax)
                    leftPoints.Points.Add(p);
            }
            leftPoints.Color = Colors.Green;
            leftPoints.Size = 5;
            return leftPoints;
        }

        public PointsVisual3D rightPlane()                      //   TEST: Search all points of front plane
        {
            PointsVisual3D rightPoints = new PointsVisual3D();
            foreach (Point3D p in this.points.Points)
            {
                if (p.Y == yMin)
                    rightPoints.Points.Add(p);
            }
            rightPoints.Color = Colors.Green;
            rightPoints.Size = 5;
            return rightPoints;
        }

        public PointsVisual3D upPlane()                      //   TEST: Search all points of front plane
        {
            PointsVisual3D upPoints = new PointsVisual3D();
            foreach (Point3D p in this.points.Points)
            {
                if (p.Z == zMax)
                    upPoints.Points.Add(p);
            }
            upPoints.Color = Colors.Blue;
            upPoints.Size = 5;
            return upPoints;
        }

        public PointsVisual3D downPlane()                      //   TEST: Search all points of front plane
        {
            PointsVisual3D downPoints = new PointsVisual3D();
            foreach (Point3D p in this.points.Points)
            {
                if (p.Z == zMin)
                    downPoints.Points.Add(p);
            }
            downPoints.Color = Colors.Blue;
            downPoints.Size = 5;
            return downPoints;
        }


        public void showPlanes(bool asPoly, bool asPoint)                            //   TEST: Add plane to visualModel
        {
            ClearView();
            toPoint();
            if (asPoly)
                this.visualModel.Content = LoadPoly();
            if (!asPoint)
                this.visualModel.Children.Clear();
            
            this.visualModel.Children.Add(frontPlane());
            this.visualModel.Children.Add(backPlane());
            this.visualModel.Children.Add(leftPlane());
            this.visualModel.Children.Add(rightPlane());
            this.visualModel.Children.Add(upPlane());
            this.visualModel.Children.Add(downPlane());

            cubeValid();
            //   var x = downPlane().Points.ToArray();
        }

        void cubeValid()
        {
            if ((System.Math.Abs(xMax - xMin) == System.Math.Abs(yMax - yMin)) && (System.Math.Abs(yMax - yMin) == System.Math.Abs(zMax - zMin)))
                 MessageBox.Show("It's Cube");
            else
                MessageBox.Show("It's not a Cube");
        }

        public void ClearView()
        {
            this.visualModel.Content = null;
            this.visualModel.Children.Clear();
            if (points != null)
            {
                this.points.Points.Clear();
            }
        }

        public void toPoly()
        {
            ClearView();
            this.visualModel.Content = LoadPoly();
        }

        public void toPoint()
        {
            ClearView();
            this.visualModel.Children.Add(LoadPoints());
        }

        public void toPolyPoint()
        {
            ClearView();
            this.visualModel.Content = LoadPoly();
            this.visualModel.Children.Add(LoadPoints());
        }

        public bool Save(string filePath)
        {
            Brush currentBrush = this.view.Background;
            try
            {
                this.view.Background = Brushes.Transparent;
                this.view.Export(filePath);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Loading error!");
                return false;
            }
            finally
            {
                this.view.Background = currentBrush;
            }
        }
    }
}
