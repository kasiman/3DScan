﻿namespace LineShadingDemo
{
    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;

    using SharpDX;

    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }
        public double LineThickness { get; set; }
        public double LineSmoothness { get; set; }
        public bool LinesEnabled { get; set; }
        public bool GridEnabled { get; set; }
                
        public PhongMaterial Material1 { get; private set; }
        public PhongMaterial Material2 { get; private set; }
        public PhongMaterial Material3 { get; private set; }        
        public SharpDX.Color GridColor { get; private set; }

        public Transform3D Model1Transform { get; private set; }
        public Transform3D Model2Transform { get; private set; }
        public Transform3D Model3Transform { get; private set; }
        public Transform3D GridTransform { get; private set; }

        public Vector3 DirectionalLightDirection { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }

      
        public MainViewModel()
        {
            this.Title = "Line Shading Demo (HelixToolkitDX)";
            this.SubTitle = null;

            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(0, 5, 5), LookDirection = new Vector3D(-0, -5, -5), UpDirection = new Vector3D(0, 1, 0) };

            // setup lighting            
            this.AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            this.DirectionalLightColor = Color.White;
            this.DirectionalLightDirection = new Vector3(-2, -5, -2);

            // floor plane grid
            this.Grid = LineBuilder.GenerateGrid();
            this.GridColor = SharpDX.Color.Black;
            this.GridTransform = new TranslateTransform3D(-5, -1, -5);

            // scene model3d
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2, BoxFaces.All);
            this.Model = b1.ToMeshGeometry3D();

            // lines model3d
            var e1 = new LineBuilder();
            e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);
            //e1.AddLine(new Vector3(-1, 0, 0), new Vector3(1, 0, 0));
            this.Lines = e1.ToLineGeometry3D();

            // lines params
            this.LineThickness = 2;
            this.LineSmoothness = 2.0;
            this.LinesEnabled = true;
            this.GridEnabled = true;

            // model trafos
            this.Model1Transform = new TranslateTransform3D(0, 0, 0);
            this.Model2Transform = new TranslateTransform3D(-2, 0, 0);
            this.Model3Transform = new TranslateTransform3D(+2, 0, 0);

            // model materials
            this.Material1 = PhongMaterials.PolishedGold;
            this.Material2 = PhongMaterials.Copper;
            this.Material3 = PhongMaterials.Glass;                        
        }
    }
}
