namespace DataTemplateDemo
{
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents an element.
    /// </summary>
    public class Element
    {
        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Point3D Position { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public double Radius { get; set; }
    }
}