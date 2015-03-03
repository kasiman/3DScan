// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace VoxelDemo
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Edit a voxel scene by clicking the sides of the voxels.")]
    public partial class MainWindow : Window
    {
        private readonly MainViewModel vm = new MainViewModel();

        public MainWindow()
        {
            this.InitializeComponent();
            this.vm.TryLoad("MyModel.xml");
            this.DataContext = vm;
            this.Loaded += this.MainWindowLoaded;
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomExtents(500);
            view1.Focus();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.vm.Save("MyModel.xml");
            base.OnClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Space:
                    vm.PaletteIndex++;
                    vm.CurrentColor = vm.GetPaletteColor();
                    break;
                case Key.A:
                    view1.ZoomExtents(500);
                    break;
                case Key.C:
                    vm.Clear();
                    break;
            }
        }

        Model3D FindSource(Point p, out Vector3D normal)
        {
            var hits = Viewport3DHelper.FindHits(view1.Viewport, p);

            foreach (var h in hits)
            {
                if (h.Model == vm.PreviewModel)
                    continue;
                normal = h.Normal;
                return h.Model;
            }

            normal = new Vector3D();
            return null;
        }

        private void view1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
            var p = e.GetPosition(view1);

            Vector3D n;
            var source = FindSource(p, out n);
            if (source != null)
            {
                if (shift)
                    vm.Remove(source);
                else
                    vm.Add(source, n);
            }
            else
            {
                var ray = Viewport3DHelper.Point2DtoRay3D(view1.Viewport, p);
                if (ray != null)
                {
                    var pi = ray.PlaneIntersection(new Point3D(0, 0, 0.5), new Vector3D(0, 0, 1));
                    if (pi.HasValue)
                    {
                        var pRound = new Point3D(Math.Round(pi.Value.X), Math.Round(pi.Value.Y),0);
                    //    var pRound = new Point3D(Math.Floor(pi.Value.X), Math.Floor(pi.Value.Y), Math.Floor(pi.Value.Z));
                        //var pRound = new Point3D((int)pi.Value.X, (int)pi.Value.Y, (int)pi.Value.Z);
                        vm.AddVoxel(pRound);
                    }
                }
            }
            UpdatePreview();
            //CaptureMouse();
        }

        private void view1_MouseMove(object sender, MouseEventArgs e)
        {
            UpdatePreview();
        }

        void UpdatePreview()
        {
            var p = Mouse.GetPosition(view1);
            bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
            Vector3D n;
            var source = FindSource(p, out n);
            if (shift)
            {
                vm.PreviewVoxel(null);
                vm.HighlightVoxel(source);
            }
            else
            {
                vm.PreviewVoxel(source, n);
                vm.HighlightVoxel(null);
            }

        }

        private void view1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //  ReleaseMouseCapture();
        }

        private void view1_KeyUp(object sender, KeyEventArgs e)
        {
            // Should update preview voxel when shift is released
            UpdatePreview();
        }

        private void view1_KeyDown(object sender, KeyEventArgs e)
        {
            // Should update preview voxel when shift is pressed
            UpdatePreview();
        }
    }
}