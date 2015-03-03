namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::SharpDX;

    using HelixToolkit.Wpf.SharpDX.Core;

    [Serializable]
    public class MeshGeometry3D : Geometry3D
    {
        public Vector3Collection Normals { get; set; }
        public Vector2Collection TextureCoordinates { get; set; }

        public Vector3Collection Tangents { get; set; }
        public Vector3Collection BiTangents { get; set; }

        public IEnumerable<Geometry3D.Triangle> Triangles
        {
            get
            {
                for (int i = 0; i < Indices.Count; i += 3)
                {
                    yield return new Triangle() { P0 = Positions[Indices[i]], P1 = Positions[Indices[i + 1]], P2 = Positions[Indices[i + 2]], };
                }
            }
        }

        public static MeshGeometry3D Merge(params MeshGeometry3D[] meshes)
        {
            var positions = new Vector3Collection();
            var indices = new IntCollection();

            var normals = meshes.All(x => x.Normals != null) ? new Vector3Collection() : null;
            var colors = meshes.All(x => x.Colors != null) ? new Color4Collection() : null;
            var textureCoods = meshes.All(x => x.TextureCoordinates != null) ? new Vector2Collection() : null;
            var tangents = meshes.All(x => x.Tangents != null) ? new Vector3Collection() : null;
            var bitangents = meshes.All(x => x.BiTangents != null) ? new Vector3Collection() : null;

            int index = 0;
            foreach (var part in meshes)
            {
                for (int i = 0; i < part.Positions.Count; i++)
                {
                    positions.Add(part.Positions[i]);
                }

                for (int i = 0; i < part.Indices.Count; i++)
                {
                    indices.Add(index + part.Indices[i]);
                }

                index += part.Indices.Count;
            }

            if (normals != null)
            {
                normals = new Vector3Collection(meshes.SelectMany(x => x.Normals));
            }

            if (colors != null)
            {
                colors = new Color4Collection(meshes.SelectMany(x => x.Colors));
            }

            if (textureCoods != null)
            {
                textureCoods = new Vector2Collection(meshes.SelectMany(x => x.TextureCoordinates));
            }

            if (tangents != null)
            {
                tangents = new Vector3Collection(meshes.SelectMany(x => x.Tangents));
            }

            if (bitangents != null)
            {
                bitangents = new Vector3Collection(meshes.SelectMany(x => x.BiTangents));
            }

            var mesh = new MeshGeometry3D()
            {
                Positions = positions,
                Indices = indices,
            };

            mesh.Normals = normals;
            mesh.Colors = colors;
            mesh.TextureCoordinates = textureCoods;
            mesh.Tangents = tangents;
            mesh.BiTangents = bitangents;

            return mesh;
        }
    }
}
