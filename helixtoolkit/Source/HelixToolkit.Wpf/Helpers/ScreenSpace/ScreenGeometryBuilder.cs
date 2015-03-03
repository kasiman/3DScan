﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenGeometryBuilder.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Controls;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Base class for mesh geometry builders that work on screen coordinates.
    /// </summary>
    public abstract class ScreenGeometryBuilder
    {
        /// <summary>
        /// The parent visual.
        /// </summary>
        protected readonly Visual3D visual;

        /// <summary>
        /// The screen to visual transformation matrix.
        /// </summary>
        protected Matrix3D screenToVisual;

        /// <summary>
        /// The visual to screen transformation matrix.
        /// </summary>
        protected Matrix3D visualToScreen;

        /// <summary>
        /// The visual to projection transformation matrix.
        /// </summary>
        protected Matrix3D visualToProjection;

        /// <summary>
        /// The projection to screen transformation matrix.
        /// </summary>
        protected Matrix3D projectionToScreen;

        /// <summary>
        /// The viewport
        /// </summary>
        private Viewport3D viewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenGeometryBuilder"/> class.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        protected ScreenGeometryBuilder(Visual3D visual)
        {
            this.visual = visual;
        }

        /// <summary>
        /// Updates the transforms.
        /// </summary>
        /// <returns>
        /// True if the transform was changed.
        /// </returns>
        public bool UpdateTransforms()
        {
            var newTransform = this.visual.GetViewportTransform();

            if (double.IsNaN(newTransform.M11))
            {
                return false;
            }

            if (!newTransform.HasInverse)
            {
                return false;
            }

            if (newTransform == this.visualToScreen)
            {
                return false;
            }

            this.visualToScreen = newTransform;
            this.screenToVisual = newTransform.Inverse();

            if (this.viewport == null)
            {
                this.viewport = this.visual.GetViewport3D();
            }

            this.projectionToScreen = this.viewport.GetProjectionMatrix() * this.viewport.GetViewportTransform();
            this.visualToProjection = this.visualToScreen * this.projectionToScreen.Inverse();

            return true;
        }
    }
}