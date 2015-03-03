﻿namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows;

    using global::SharpDX;

    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;

    /// <summary>
    ///   A translate manipulator.
    /// </summary>
    public class UITranslateManipulator3D : UIManipulator3D
    {
        /// <summary>
        /// The diameter property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(UITranslateManipulator3D), new UIPropertyMetadata(0.2, ModelChanged));

        /// <summary>
        /// The direction property.
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Vector3), typeof(UITranslateManipulator3D), new UIPropertyMetadata(new Vector3(0, 0, 1), ModelChanged));

        /// <summary>
        /// The length property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(double), typeof(UITranslateManipulator3D), new UIPropertyMetadata(1.0, ModelChanged));

        /// <summary>
        /// Gets or sets the diameter of the manipulator arrow.
        /// </summary>
        /// <value> The diameter. </value>
        public double Diameter
        {
            get { return (double)this.GetValue(DiameterProperty); }
            set { this.SetValue(DiameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the direction of the translation.
        /// </summary>
        /// <value> The direction. </value>
        public Vector3 Direction
        {
            get { return (Vector3)this.GetValue(DirectionProperty); }
            set { this.SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the length of the manipulator arrow.
        /// </summary>
        /// <value> The length. </value>
        public double Length
        {
            get { return (double)this.GetValue(LengthProperty); }
            set { this.SetValue(LengthProperty, value); }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="UIManipulator3D" /> class.
        /// </summary>
        public UITranslateManipulator3D()
        {
            OnModelChanged();
            this.Material = PhongMaterials.Red;
        }

        /// <summary>
        /// Called when geometry has been changed.
        /// </summary>
        protected override void OnModelChanged()
        {            
            var mb = new MeshBuilder();
            var p0 = this.Offset;// new Vector3(0, 0, 0);
            var d = this.Direction;
            d.Normalize();
            var p1 = p0 + (d * (float)this.Length);
            mb.AddArrow(p0, p1, this.Diameter, 2, 64);
            this.Geometry = mb.ToMeshGeometry3D();            
        }

        /// <summary>
        /// 
        /// </summary>        
        protected override void UpdateManipulator(RoutedEventArgs e)
        {
            var args = e as Mouse3DEventArgs;

            // camera normal
            var normalWS = this.cameraNormal;
            // move directon
            var directionWS = ToWorldVec(this.Direction);
            // up direction
            var upWS = Vector3.Cross(normalWS, directionWS);
            // the direction plane
            normalWS = Vector3.Cross(upWS, directionWS); normalWS.Normalize();
            // find new hit on the camera-direction plane
            var newHit = this.viewport.UnProjectOnPlane(args.Position.ToVector2(), lastHitPosWS, normalWS);

            if (newHit.HasValue)
            {
                // project point on ray
                // a: vec to project on
                //b(a) = (a.b)/(a.a)*a;
                var b = newHit.Value - lastHitPosWS;
                var ab = Vector3.Dot(directionWS, b);
                var aa = Vector3.Dot(directionWS, directionWS);
                var ba = (ab / aa) * directionWS;
                newHit = lastHitPosWS + ba;

                var delta = newHit.Value - lastHitPosWS;
                this.Value += Vector3.Dot(delta, directionWS);
                var deltaTranslateTrafo = new TranslateTransform3D(delta.ToVector3D());

                if (this.TargetTransform != null)
                {                    
                    this.TargetTransform = this.TargetTransform.AppendTransform(deltaTranslateTrafo);
                }
                else
                {                                        
                    this.Transform = this.Transform.AppendTransform(deltaTranslateTrafo);
                }

                this.lastHitPosWS = newHit.Value;
            }
        }
    }
}