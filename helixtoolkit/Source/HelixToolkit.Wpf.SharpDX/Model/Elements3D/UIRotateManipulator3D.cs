﻿namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows;

    using global::SharpDX;

    /// <summary>
    ///   A translate manipulator.
    /// </summary>
    public class UIRotateManipulator3D : UIManipulator3D
    {
        /// <summary>
        /// The axis property.
        /// </summary>
        public static readonly DependencyProperty AxisProperty = DependencyProperty.Register(
            "Axis", typeof(Vector3), typeof(UIRotateManipulator3D), new UIPropertyMetadata(new Vector3(0, 0, 1), ModelChanged));

        /// <summary>
        /// The diameter property.
        /// </summary>
        public static readonly DependencyProperty OuterDiameterProperty = DependencyProperty.Register(
            "OuterDiameter", typeof(double), typeof(UIRotateManipulator3D), new UIPropertyMetadata(1.5, ModelChanged));

        /// <summary>
        /// The inner diameter property.
        /// </summary>
        public static readonly DependencyProperty InnerDiameterProperty = DependencyProperty.Register(
            "InnerDiameter", typeof(double), typeof(UIRotateManipulator3D), new UIPropertyMetadata(1.0, ModelChanged));

        /// <summary>
        /// The length property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(UIRotateManipulator3D), new UIPropertyMetadata(0.1, ModelChanged));

        /// <summary>
        /// The pivot point property.
        /// </summary>
        public static readonly DependencyProperty PivotProperty = DependencyProperty.Register(
            "Pivot", typeof(Vector3), typeof(UIRotateManipulator3D), new PropertyMetadata(new Vector3(0, 0, 0)));

        /// <summary>
        /// Gets or sets the rotation axis.
        /// </summary>
        /// <value>The axis.</value>
        public Vector3 Axis
        {
            get { return (Vector3)this.GetValue(AxisProperty); }
            set { this.SetValue(AxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the diameter of the manipulator arrow.
        /// </summary>
        /// <value> The diameter. </value>
        public double OuterDiameter
        {
            get { return (double)this.GetValue(OuterDiameterProperty); }
            set { this.SetValue(OuterDiameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the inner diameter.
        /// </summary>
        /// <value>The inner diameter.</value>
        public double InnerDiameter
        {
            get { return (double)this.GetValue(InnerDiameterProperty); }
            set { this.SetValue(InnerDiameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the length of the cylinder.
        /// </summary>
        /// <value>The length.</value>
        public double Length
        {
            get { return (double)this.GetValue(LengthProperty); }
            set { this.SetValue(LengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pivot point of the manipulator.
        /// </summary>
        /// <value> The position. </value>
        public Vector3 Pivot
        {
            get { return (Vector3)this.GetValue(PivotProperty); }
            set { this.SetValue(PivotProperty, value); }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="UIManipulator3D" /> class.
        /// </summary>
        public UIRotateManipulator3D()
        {
            OnModelChanged();            
        }

        /// <summary>
        /// Called when geometry has been changed.
        /// </summary>
        protected override void OnModelChanged()
        {
            var mb = new MeshBuilder();
            var p0 = this.Offset; //new Vector3(0, 0, 0);
            if (this.InnerDiameter >= this.OuterDiameter)
                this.OuterDiameter = this.InnerDiameter + 0.3;

            var d = this.Axis;
            d.Normalize();
            var p1 = p0 - (d * (float)this.Length * 0.5f);
            var p2 = p0 + (d * (float)this.Length * 0.5f);
            mb.AddPipe(p1, p2, this.InnerDiameter, this.OuterDiameter, 64);
            this.Geometry = mb.ToMeshGeometry3D();
        }

        public override void Render(RenderContext renderContext)
        {
            var position = this.totalModelMatrix.TranslationVector;
            base.Render(renderContext);
        }

        /// <summary>
        /// 
        /// </summary>        
        protected override void UpdateManipulator(RoutedEventArgs e)
        {
            if (!this.isMouseCaptured) return;

            var args = e as Mouse3DEventArgs;            

            /// --- get the plane for translation (camera normal is a good choice)                     
            var normal = this.cameraNormal;
            var position = this.ModelMatrix.TranslationVector;
            //var position = this.totalModelMatrix.TranslationVector;

            /// --- hit position 
            var newHit = this.viewport.UnProjectOnPlane(args.Position.ToVector2(), lastHitPosWS, normal);

            if (newHit.HasValue)
            {
                var newHitPos = newHit.Value;
                var v = this.lastHitPosWS - position;
                var u = newHitPos - position;
                v.Normalize();
                u.Normalize();
                
                var currentAxis = Vector3.Cross(u, v);
                var mainAxis = ToWorldVec(this.Axis);// this.Transform.Transform(this.Axis.ToVector3D()).ToVector3();
                double sign = -Vector3.Dot(mainAxis, currentAxis);
                double theta = Math.Sign(sign) * Math.Asin(currentAxis.Length()) / Math.PI * 180;
                this.Value += theta;
                
                var rotateTransform = new System.Windows.Media.Media3D.RotateTransform3D(new System.Windows.Media.Media3D.AxisAngleRotation3D(this.Axis.ToVector3D(), theta), Pivot.ToPoint3D());

                /// rotate target
                if (this.TargetTransform != null)
                {                    
                    this.TargetTransform = rotateTransform.AppendTransform(this.TargetTransform);
                }
                else
                {
                    this.Transform = rotateTransform.AppendTransform(this.Transform);
                }
                this.lastHitPosWS = newHitPos;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnMouse3DMove(object sender, RoutedEventArgs e)
        {            
            if (IsHitTestVisible)
                base.OnMouse3DMove(sender, e);
        }
    }
}