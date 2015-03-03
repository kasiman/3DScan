﻿namespace ShadowMapDemo
{
    using System;
    using System.Windows.Media.Animation;

    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;

    using SharpDX;

    using Matrix = SharpDX.Matrix;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }        
        public MeshGeometry3D Plane { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }
        public Matrix[] Instances { get; private set; }

        public PhongMaterial RedMaterial { get; private set; }
        public PhongMaterial GreenMaterial { get; private set; }
        public PhongMaterial BlueMaterial { get; private set; }
        public PhongMaterial GrayMaterial { get; private set; }
        public SharpDX.Color GridColor { get; private set; }

        public Media3D.Transform3D Model1Transform { get; private set; }
        public Media3D.Transform3D Model2Transform { get; private set; }
        public Media3D.Transform3D Model3Transform { get; private set; }
        public Media3D.Transform3D GridTransform { get; private set; }
        public Media3D.Transform3D PlaneTransform { get; private set; }
        
        public Media3D.Transform3D LightDirectionTransform { get; set; }
        public Vector3 DirectionalLightDirection { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }
        public Vector2 ShadowMapResolution { get; private set; }

        public double XValue { get { return this.xvalue; } set { this.SetXValue(value); } }
        public bool IsAnimated { get { return this.isAnimated; } set { this.OnAnimatedChanged(value); } }

        public MainViewModel()
        {
            Title = "Shadow Map Demo";
            SubTitle = "WPF & SharpDX";

            // setup lighting            
            this.AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            this.DirectionalLightColor = Color.White;
            this.DirectionalLightDirection = new Vector3(-0, -1, -1);
            this.LightDirectionTransform = CreateAnimatedTransform(-DirectionalLightDirection.ToVector3D(), new Vector3D(0, 1, -1), 24);
            this.ShadowMapResolution = new Vector2(2048, 2048);

            // camera setup
            this.Camera = new PerspectiveCamera { Position = (Point3D)(-DirectionalLightDirection.ToVector3D()), LookDirection = DirectionalLightDirection.ToVector3D(), UpDirection = new Vector3D(0, 1, 0) };

            // floor plane grid
            //Grid = LineBuilder.GenerateGrid();
            //GridColor = SharpDX.Color.Black;
            //GridTransform = new Media3D.TranslateTransform3D(-5, -1, -5);

            // scene model3d
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.25, 2, BoxFaces.All);
            Model = b1.ToMeshGeometry3D();
            //Instances = new[] { Matrix.Translation(0, 0, -1.5f), Matrix.Translation(0, 0, 1.5f) };

            var b2 = new MeshBuilder();
            b2.AddBox(new Vector3(0, 0, 0), 10, 0, 10, BoxFaces.PositiveY);
            Plane = b2.ToMeshGeometry3D();
            PlaneTransform = new Media3D.TranslateTransform3D(-0, -2, -0);
            GrayMaterial = PhongMaterials.LightGray;
            //GrayMaterial.TextureMap = new BitmapImage(new System.Uri(@"TextureCheckerboard2.jpg", System.UriKind.RelativeOrAbsolute)); 

            // lines model3d            
            Lines = LineBuilder.GenerateBoundingBox(Model);

            // model trafos
            Model1Transform = new Media3D.TranslateTransform3D(0, 0, 0);
            Model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
            Model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);            

            // model materials
            RedMaterial = PhongMaterials.Glass;
            GreenMaterial = PhongMaterials.Green;
            BlueMaterial = PhongMaterials.Blue;
        }

        private void SetXValue(double x)
        {
            Console.WriteLine("x: {0}", x);
            this.xvalue = x;
            //this.DirectionalLightDirection = new Vector3D(x, -10, -10);
            this.LightDirectionTransform = new Media3D.TranslateTransform3D(x, -10, 10);
        }
        private double xvalue;

        private Media3D.Transform3D CreateAnimatedTransform(Vector3D translate, Vector3D axis, double speed = 4)
        {
            var lightTrafo = new Media3D.Transform3DGroup();
            lightTrafo.Children.Add(new Media3D.TranslateTransform3D(translate));

            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                //By = new Media3D.AxisAngleRotation3D(axis, 180),
                From = new Media3D.AxisAngleRotation3D(axis, 135),
                To = new Media3D.AxisAngleRotation3D(axis, 225),
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(speed / 4),
                //IsCumulative = true,                  
            };

            var rotateTransform = new Media3D.RotateTransform3D();
            rotateTransform.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);            
            return lightTrafo;
        }

        private void OnAnimatedChanged(bool value)
        {
            this.isAnimated = value;
            if (value)
            {
                this.LightDirectionTransform = CreateAnimatedTransform(-DirectionalLightDirection.ToVector3D(), new Vector3D(0, 1, -1), 24);
            }
            else
            {
                this.LightDirectionTransform = Media3D.Transform3D.Identity;
            }
        }
        private bool isAnimated = true;
    }
}
