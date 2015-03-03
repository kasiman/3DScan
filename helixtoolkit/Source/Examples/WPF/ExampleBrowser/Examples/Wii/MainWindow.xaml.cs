﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WiiDemo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Change the transformation of a model by the Wii remote.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainViewModel(Dispatcher, view1);
            DataContext = vm;
            Loaded += MainWindow_Loaded;

            vm.UpAction += Explode;
            CompositionTarget.Rendering += this.OnCompositionTargetRendering;

        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            var meshesToDelete = new List<ExplodingMesh>();
            foreach (var mesh in explodingMeshes)
            {
                mesh.Integrate();
                if (!mesh.IsMoving())
                    meshesToDelete.Add(mesh);
            }

            foreach (var m in meshesToDelete)
                explodingMeshes.Remove(m);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            vm.OnLoaded();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            vm.OnClosing();
        }

        private readonly MainViewModel vm;

        List<ExplodingMesh> explodingMeshes = new List<ExplodingMesh>();

        private void Explode()
        {
            var pos = Mouse.GetPosition(view1);
            var hits = Viewport3DHelper.FindHits(view1.Viewport, pos);
            if (hits.Count > 0)
            {
                var mesh = hits[0].Mesh;
                var model = hits[0].Model as GeometryModel3D;
                var hitpos = hits[0].Position;

                var explodingMesh = new ExplodingMesh(mesh, hitpos);
                model.Geometry = explodingMesh.Mesh;
                explodingMeshes.Add(explodingMesh);
            }
        }

        private void view1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount==2)
                Explode();
        }
    }

    public class ExplodingMesh
    {
        private VerletIntegrator integrator;

        public MeshGeometry3D Mesh { get; set; }

        Stopwatch watch = new Stopwatch();

        public ExplodingMesh(MeshGeometry3D inputMesh, Point3D hitpos)
        {
            var mesh = MeshGeometryHelper.NoSharedVertices(inputMesh);

            double cx, cy, cz;
            cx = cy = cz = 0;
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                cx += mesh.Positions[i].X;
                cy += mesh.Positions[i].Y;
                cz += mesh.Positions[i].Z;
            }
            int n = mesh.Positions.Count;
            var center = new Point3D(cx / n, cy / n, cz / n);

            integrator = new VerletIntegrator();
            integrator.Resize(mesh.Positions.Count);
            var r = new Random();
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                var delta = mesh.Positions[i] - center;
                delta.Normalize();
                integrator.Positions[i] = mesh.Positions[i] + delta * (1 + r.NextDouble() * 2);
                integrator.Positions0[i] = mesh.Positions[i];
                integrator.Accelerations[i] = new Vector3D(0, 0, -1000);
                integrator.InverseMass[i] = 0.01;
            }

            integrator.CreateConstraintsByMesh(mesh, 0.7);
            integrator.AddFloor(0.3);
            this.Mesh = mesh;
            watch.Start();
        }

        private double totalTime = 0;

        public void Integrate()
        {
            var dt = watch.ElapsedMilliseconds*0.001;
            if (dt == 0)
                dt = 0.01;
            totalTime += dt;
            watch.Restart();

            integrator.TimeStep(dt);
            integrator.TransferPositions(Mesh);

        }

        public bool IsMoving() {
            return totalTime < 3;
        }
    }
}