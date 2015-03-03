﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraSetting.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    /// <summary>
    /// Represents camera settings.
    /// </summary>
    public class CameraSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraSetting"/> class.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public CameraSetting(ProjectionCamera camera)
        {
            this.Position = camera.Position;
            this.LookDirection = camera.LookDirection;
            this.UpDirection = camera.UpDirection;
            this.NearPlaneDistance = camera.NearPlaneDistance;
            this.FarPlaneDistance = camera.FarPlaneDistance;
            var pcamera = camera as PerspectiveCamera;
            if (pcamera != null)
            {
                this.FieldOfView = pcamera.FieldOfView;
            }

            var ocamera = camera as OrthographicCamera;
            if (ocamera != null)
            {
                this.Width = ocamera.Width;
            }
        }

        /// <summary>
        /// Gets or sets FarPlaneDistance.
        /// </summary>
        public double FarPlaneDistance { get; set; }

        /// <summary>
        /// Gets or sets FieldOfView.
        /// </summary>
        public double FieldOfView { get; set; }

        /// <summary>
        /// Gets or sets LookDirection.
        /// </summary>
        public Vector3D LookDirection { get; set; }

        /// <summary>
        /// Gets or sets NearPlaneDistance.
        /// </summary>
        public double NearPlaneDistance { get; set; }

        /// <summary>
        /// Gets or sets Position.
        /// </summary>
        public Point3D Position { get; set; }

        /// <summary>
        /// Gets or sets UpDirection.
        /// </summary>
        public Vector3D UpDirection { get; set; }

        /// <summary>
        /// Gets or sets Width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Sets the properties of the specified camera to the settings stored in this object.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public void UpdateCamera(ProjectionCamera camera)
        {
            camera.Position = this.Position;
            camera.LookDirection = this.LookDirection;
            camera.UpDirection = this.UpDirection;
            camera.NearPlaneDistance = this.NearPlaneDistance;
            camera.FarPlaneDistance = this.FarPlaneDistance;
            var perspectiveCamera = camera as PerspectiveCamera;
            if (perspectiveCamera != null)
            {
                perspectiveCamera.FieldOfView = this.FieldOfView;
            }

            var orthographicCamera = camera as OrthographicCamera;
            if (orthographicCamera != null)
            {
                orthographicCamera.Width = this.Width;
            }
        }
    }
}