﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshGeometryHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   Provides helper methods for mesh geometries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides helper methods for mesh geometries.
    /// </summary>
    public static class MeshGeometryHelper
    {
        // Optimizing 3D Collections in WPF
        // http://blogs.msdn.com/timothyc/archive/2006/08/31/734308.aspx
        // - Remember to disconnect collections from the MeshGeometry when changing it

        /// <summary>
        /// Calculates the normal vectors.
        /// </summary>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        /// <returns>
        /// Collection of normal vectors.
        /// </returns>
        public static Vector3DCollection CalculateNormals(MeshGeometry3D mesh)
        {
            return CalculateNormals(mesh.Positions, mesh.TriangleIndices);
        }

        /// <summary>
        /// Calculates the normal vectors.
        /// </summary>
        /// <param name="positions">
        /// The positions.
        /// </param>
        /// <param name="triangleIndices">
        /// The triangle indices.
        /// </param>
        /// <returns>
        /// Collection of normal vectors.
        /// </returns>
        public static Vector3DCollection CalculateNormals(IList<Point3D> positions, IList<int> triangleIndices)
        {
            var normals = new Vector3DCollection(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                normals.Add(new Vector3D());
            }

            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                int index0 = triangleIndices[i];
                int index1 = triangleIndices[i + 1];
                int index2 = triangleIndices[i + 2];
                var p0 = positions[index0];
                var p1 = positions[index1];
                var p2 = positions[index2];
                Vector3D u = p1 - p0;
                Vector3D v = p2 - p0;
                Vector3D w = Vector3D.CrossProduct(u, v);
                w.Normalize();
                normals[index0] += w;
                normals[index1] += w;
                normals[index2] += w;
            }

            for (int i = 0; i < normals.Count; i++)
            {
                var w = normals[i];
                w.Normalize();
                normals[i] = w;
            }

            return normals;
        }

        /// <summary>
        /// Finds edges that are only connected to one triangle.
        /// </summary>
        /// <param name="mesh">
        /// A mesh geometry.
        /// </param>
        /// <returns>
        /// The edge indices for the edges that are only used by one triangle.
        /// </returns>
        public static Int32Collection FindBorderEdges(MeshGeometry3D mesh)
        {
            var dict = new Dictionary<ulong, int>();

            for (int i = 0; i < mesh.TriangleIndices.Count / 3; i++)
            {
                int i0 = i * 3;
                for (int j = 0; j < 3; j++)
                {
                    int index0 = mesh.TriangleIndices[i0 + j];
                    int index1 = mesh.TriangleIndices[i0 + ((j + 1) % 3)];
                    int minIndex = Math.Min(index0, index1);
                    int maxIndex = Math.Max(index1, index0);
                    ulong key = CreateKey((uint)minIndex, (uint)maxIndex);
                    if (dict.ContainsKey(key))
                    {
                        dict[key] = dict[key] + 1;
                    }
                    else
                    {
                        dict.Add(key, 1);
                    }
                }
            }

            var edges = new Int32Collection();
            foreach (var kvp in dict)
            {
                // find edges only used by 1 triangle
                if (kvp.Value == 1)
                {
                    uint i0, i1;
                    ReverseKey(kvp.Key, out i0, out i1);
                    edges.Add((int)i0);
                    edges.Add((int)i1);
                }
            }

            return edges;
        }

        /// <summary>
        /// Finds all edges in the mesh (each edge is only included once).
        /// </summary>
        /// <param name="mesh">
        /// A mesh geometry.
        /// </param>
        /// <returns>
        /// The edge indices (minimum index first).
        /// </returns>
        public static Int32Collection FindEdges(MeshGeometry3D mesh)
        {
            var edges = new Int32Collection();
            var dict = new HashSet<ulong>();

            for (int i = 0; i < mesh.TriangleIndices.Count / 3; i++)
            {
                int i0 = i * 3;
                for (int j = 0; j < 3; j++)
                {
                    int index0 = mesh.TriangleIndices[i0 + j];
                    int index1 = mesh.TriangleIndices[i0 + ((j + 1) % 3)];
                    int minIndex = Math.Min(index0, index1);
                    int maxIndex = Math.Max(index1, index0);
                    ulong key = CreateKey((uint)minIndex, (uint)maxIndex);
                    if (!dict.Contains(key))
                    {
                        edges.Add(minIndex);
                        edges.Add(maxIndex);
                        dict.Add(key);
                    }
                }
            }

            return edges;
        }

        /// <summary>
        /// Finds all edges where the angle between adjacent triangle normal vectors.
        /// is larger than minimumAngle
        /// </summary>
        /// <param name="mesh">
        /// A mesh geometry.
        /// </param>
        /// <param name="minimumAngle">
        /// The minimum angle between the normal vectors of two adjacent triangles (degrees).
        /// </param>
        /// <returns>
        /// The edge indices.
        /// </returns>
        public static Int32Collection FindSharpEdges(MeshGeometry3D mesh, double minimumAngle)
        {
            var edgeIndices = new Int32Collection();

            // the keys of the dictionary are created from the triangle indices of the edge
            var edgeNormals = new Dictionary<ulong, Vector3D>();

            for (int i = 0; i < mesh.TriangleIndices.Count / 3; i++)
            {
                int i0 = i * 3;
                var p0 = mesh.Positions[mesh.TriangleIndices[i0]];
                var p1 = mesh.Positions[mesh.TriangleIndices[i0 + 1]];
                var p2 = mesh.Positions[mesh.TriangleIndices[i0 + 2]];
                var n = Vector3D.CrossProduct(p1 - p0, p2 - p0);
                n.Normalize();
                for (int j = 0; j < 3; j++)
                {
                    int index0 = mesh.TriangleIndices[i0 + j];
                    int index1 = mesh.TriangleIndices[i0 + ((j + 1) % 3)];
                    int minIndex = Math.Min(index0, index1);
                    int maxIndex = Math.Max(index0, index1);
                    ulong key = CreateKey((uint)minIndex, (uint)maxIndex);
                    Vector3D value;
                    if (edgeNormals.TryGetValue(key, out value))
                    {
                        var n2 = value;
                        n2.Normalize();
                        double angle = 180 / Math.PI * Math.Acos(Vector3D.DotProduct(n, n2));
                        if (angle > minimumAngle)
                        {
                            edgeIndices.Add(minIndex);
                            edgeIndices.Add(maxIndex);
                        }
                    }
                    else
                    {
                        edgeNormals.Add(key, n);
                    }
                }
            }

            return edgeIndices;
        }

        /// <summary>
        /// Creates a new mesh where no vertices are shared.
        /// </summary>
        /// <param name="input">
        /// The input mesh.
        /// </param>
        /// <returns>
        /// A new mesh.
        /// </returns>
        public static MeshGeometry3D NoSharedVertices(MeshGeometry3D input)
        {
            var p = new Point3DCollection();
            var ti = new Int32Collection();
            Vector3DCollection n = null;
            if (input.Normals != null)
            {
                n = new Vector3DCollection();
            }

            PointCollection tc = null;
            if (input.TextureCoordinates != null)
            {
                tc = new PointCollection();
            }

            for (int i = 0; i < input.TriangleIndices.Count; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                int index0 = input.TriangleIndices[i0];
                int index1 = input.TriangleIndices[i1];
                int index2 = input.TriangleIndices[i2];
                var p0 = input.Positions[index0];
                var p1 = input.Positions[index1];
                var p2 = input.Positions[index2];
                p.Add(p0);
                p.Add(p1);
                p.Add(p2);
                ti.Add(i0);
                ti.Add(i1);
                ti.Add(i2);
                if (n != null)
                {
                    n.Add(input.Normals[index0]);
                    n.Add(input.Normals[index1]);
                    n.Add(input.Normals[index2]);
                }

                if (tc != null)
                {
                    tc.Add(input.TextureCoordinates[index0]);
                    tc.Add(input.TextureCoordinates[index1]);
                    tc.Add(input.TextureCoordinates[index2]);
                }
            }

            return new MeshGeometry3D { Positions = p, TriangleIndices = ti, Normals = n, TextureCoordinates = tc };
        }

        /// <summary>
        /// Simplifies the specified mesh.
        /// </summary>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        /// <param name="eps">
        /// The tolerance.
        /// </param>
        /// <returns>
        /// A simplified mesh.
        /// </returns>
        public static MeshGeometry3D Simplify(MeshGeometry3D mesh, double eps)
        {
            // Find common positions
            var dict = new Dictionary<int, int>(); // map position index to first occurence of same position
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                for (int j = i + 1; j < mesh.Positions.Count; j++)
                {
                    if (dict.ContainsKey(j))
                    {
                        continue;
                    }

                    double l2 = (mesh.Positions[i] - mesh.Positions[j]).LengthSquared;
                    if (l2 < eps)
                    {
                        dict.Add(j, i);
                    }
                }
            }

            var p = new Point3DCollection();
            var ti = new Int32Collection();

            // create new positions array
            var newIndex = new Dictionary<int, int>(); // map old index to new index
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                if (!dict.ContainsKey(i))
                {
                    newIndex.Add(i, p.Count);
                    p.Add(mesh.Positions[i]);
                }
            }

            // Update triangle indices
            foreach (int index in mesh.TriangleIndices)
            {
                int j;
                ti.Add(dict.TryGetValue(index, out j) ? newIndex[j] : newIndex[index]);
            }

            var result = new MeshGeometry3D { Positions = p, TriangleIndices = ti };
            return result;
        }

        /// <summary>
        /// Validates the specified mesh.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <returns>Validation report or null if no issues were found.</returns>
        public static string Validate(MeshGeometry3D mesh)
        {
            var sb = new StringBuilder();
            if (mesh.Normals != null && mesh.Normals.Count != 0 && mesh.Normals.Count != mesh.Positions.Count)
            {
                sb.AppendLine("Wrong number of normal vectors");
            }

            if (mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count != 0
                && mesh.TextureCoordinates.Count != mesh.Positions.Count)
            {
                sb.AppendLine("Wrong number of TextureCoordinates");
            }

            if (mesh.TriangleIndices.Count % 3 != 0)
            {
                sb.AppendLine("TriangleIndices not complete");
            }

            for (int i = 0; i < mesh.TriangleIndices.Count; i++)
            {
                int index = mesh.TriangleIndices[i];
                if (index < 0 || index >= mesh.Positions.Count)
                {
                    sb.AppendFormat("Wrong index {0} in triangle {1} vertex {2}", index, i / 3, i % 3);
                    sb.AppendLine();
                }
            }

            return sb.Length > 0 ? sb.ToString() : null;
        }

        /// <summary>
        /// Cuts the mesh with the specified plane.
        /// </summary>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        /// <param name="plane">
        /// The plane origin.
        /// </param>
        /// <param name="normal">
        /// The plane normal.
        /// </param>
        /// <returns>
        /// The <see cref="MeshGeometry3D"/>.
        /// </returns>
        public static MeshGeometry3D Cut(MeshGeometry3D mesh, Point3D plane, Vector3D normal)
        {
            var hasTextureCoordinates = mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count > 0;
            var meshBuilder = new MeshBuilder(false, hasTextureCoordinates);
            var contourHelper = new ContourHelper(plane, normal, mesh, hasTextureCoordinates);
            foreach (var position in mesh.Positions)
            {
                meshBuilder.Positions.Add(position);
            }

            if (hasTextureCoordinates)
            {
                foreach (var textureCoordinate in mesh.TextureCoordinates)
                {
                    meshBuilder.TextureCoordinates.Add(textureCoordinate);
                }
            }

            for (var i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                var index0 = mesh.TriangleIndices[i];
                var index1 = mesh.TriangleIndices[i + 1];
                var index2 = mesh.TriangleIndices[i + 2];

                Point3D[] positions;
                Point[] textureCoordinates;
                int[] triangleIndices;

                contourHelper.ContourFacet(index0, index1, index2, out positions, out textureCoordinates, out triangleIndices);

                foreach (var p in positions)
                {
                    meshBuilder.Positions.Add(p);
                }

                foreach (var tc in textureCoordinates)
                {
                    meshBuilder.TextureCoordinates.Add(tc);
                }

                foreach (var ti in triangleIndices)
                {
                    meshBuilder.TriangleIndices.Add(ti);
                }
            }

            return meshBuilder.ToMesh();
        }

        /// <summary>
        /// Gets the contour segments.
        /// </summary>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        /// <param name="plane">
        /// The plane origin.
        /// </param>
        /// <param name="normal">
        /// The plane normal.
        /// </param>
        /// <returns>
        /// The segments of the contour.
        /// </returns>
        public static IList<Point3D> GetContourSegments(MeshGeometry3D mesh, Point3D plane, Vector3D normal)
        {
            var segments = new List<Point3D>();
            var contourHelper = new ContourHelper(plane, normal, mesh);
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                Point3D[] positions;
                Point[] textureCoordinates;
                int[] triangleIndices;

                contourHelper.ContourFacet(
                    mesh.TriangleIndices[i],
                    mesh.TriangleIndices[i + 1],
                    mesh.TriangleIndices[i + 2],
                    out positions,
                    out textureCoordinates,
                    out triangleIndices);
                segments.AddRange(positions);
            }

            return segments;
        }

        /// <summary>
        /// Combines the segments.
        /// </summary>
        /// <param name="segments">
        /// The segments.
        /// </param>
        /// <param name="eps">
        /// The tolerance.
        /// </param>
        /// <returns>
        /// Enumerated connected contour curves.
        /// </returns>
        public static IEnumerable<IList<Point3D>> CombineSegments(IList<Point3D> segments, double eps)
        {
            // This is a simple, slow, naïve method - should be improved:
            // http://stackoverflow.com/questions/1436091/joining-unordered-line-segments
            var curve = new List<Point3D>();
            int curveCount = 0;

            int segmentCount = segments.Count;
            int segment1 = -1, segment2 = -1;
            while (segmentCount > 0)
            {
                if (curveCount > 0)
                {
                    // Find a segment that is connected to the head of the contour
                    segment1 = FindConnectedSegment(segments, curve[0], eps);
                    if (segment1 >= 0)
                    {
                        if (segment1 % 2 == 1)
                        {
                            curve.Insert(0, segments[segment1 - 1]);
                            segments.RemoveAt(segment1 - 1);
                            segments.RemoveAt(segment1 - 1);
                        }
                        else
                        {
                            curve.Insert(0, segments[segment1 + 1]);
                            segments.RemoveAt(segment1);
                            segments.RemoveAt(segment1);
                        }

                        curveCount++;
                        segmentCount -= 2;
                    }

                    // Find a segment that is connected to the tail of the contour
                    segment2 = FindConnectedSegment(segments, curve[curveCount - 1], eps);
                    if (segment2 >= 0)
                    {
                        if (segment2 % 2 == 1)
                        {
                            curve.Add(segments[segment2 - 1]);
                            segments.RemoveAt(segment2 - 1);
                            segments.RemoveAt(segment2 - 1);
                        }
                        else
                        {
                            curve.Add(segments[segment2 + 1]);
                            segments.RemoveAt(segment2);
                            segments.RemoveAt(segment2);
                        }

                        curveCount++;
                        segmentCount -= 2;
                    }
                }

                if ((segment1 < 0 && segment2 < 0) || segmentCount == 0)
                {
                    if (curveCount > 0)
                    {
                        yield return curve;
                        curve = new List<Point3D>();
                        curveCount = 0;
                    }

                    if (segmentCount > 0)
                    {
                        curve.Add(segments[0]);
                        curve.Add(segments[1]);
                        curveCount += 2;
                        segments.RemoveAt(0);
                        segments.RemoveAt(0);
                        segmentCount -= 2;
                    }
                }
            }
        }

        /// <summary>
        /// Create a 64-bit key from two 32-bit indices
        /// </summary>
        /// <param name="i0">
        /// The i 0.
        /// </param>
        /// <param name="i1">
        /// The i 1.
        /// </param>
        /// <returns>
        /// The create key.
        /// </returns>
        private static ulong CreateKey(uint i0, uint i1)
        {
            return ((ulong)i0 << 32) + i1;
        }

        /// <summary>
        /// Extract two 32-bit indices from the 64-bit key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="i0">
        /// The i 0.
        /// </param>
        /// <param name="i1">
        /// The i 1.
        /// </param>
        private static void ReverseKey(ulong key, out uint i0, out uint i1)
        {
            i0 = (uint)(key >> 32);
            i1 = (uint)((key << 32) >> 32);
        }

        /// <summary>
        /// Finds the nearest connected segment to the specified point.
        /// </summary>
        /// <param name="segments">
        /// The segments.
        /// </param>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <param name="eps">
        /// The tolerance.
        /// </param>
        /// <returns>
        /// The index of the nearest point.
        /// </returns>
        private static int FindConnectedSegment(IList<Point3D> segments, Point3D point, double eps)
        {
            double best = eps;
            int result = -1;
            for (int i = 0; i < segments.Count; i++)
            {
                double ls0 = (point - segments[i]).LengthSquared;
                if (ls0 < best)
                {
                    result = i;
                    best = ls0;
                }
            }

            return result;
        }
    }
}