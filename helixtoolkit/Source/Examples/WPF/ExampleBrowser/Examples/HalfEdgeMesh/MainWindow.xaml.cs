﻿namespace HalfEdgeMeshDemo
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Example on a half-edge mesh geometry.")]
    public partial class MainWindow : Window
    {
        public HalfEdgeMesh Mesh { get; set; }
        
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Mesh = this.CreateUnitCubeMesh();
        }

        private HalfEdgeMesh CreateUnitCubeMesh()
        {
            var vertices = new[]
                {
                    new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 1, 0), new Point3D(0, 1, 0),
                    new Point3D(0, 0, 1), new Point3D(1, 0, 1), new Point3D(1, 1, 1), new Point3D(0, 1, 1)
                };
            var mesh = new HalfEdgeMesh(vertices);
            mesh.AddFace(3, 2, 1, 0);
            mesh.AddFace(4, 5, 6, 7);
            mesh.AddFace(0, 1, 5, 4);
            mesh.AddFace(1, 2, 6, 5);
            mesh.AddFace(2, 3, 7, 6);
            mesh.AddFace(3, 0, 4, 7);
            return mesh;
        }
    }
}
