﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix 3D Toolkit examples">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MazeDemo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The camera position.
        /// </summary>
        private Point3D cameraPosition;

        /// <summary>
        /// The ground geometry.
        /// </summary>
        private MeshGeometry3D groundGeometry;

        /// <summary>
        /// The maze.
        /// </summary>
        private bool[,] maze;

        /// <summary>
        /// The offset x.
        /// </summary>
        private double offsetX;

        /// <summary>
        /// The offset y.
        /// </summary>
        private double offsetY;

        /// <summary>
        /// The previous point.
        /// </summary>
        private Point3D previousPoint;

        /// <summary>
        /// The solution geometry.
        /// </summary>
        private MeshGeometry3D solutionGeometry;

        /// <summary>
        /// The walls geometry.
        /// </summary>
        private MeshGeometry3D wallsGeometry;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            this.CreateMaze();
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets CameraPosition.
        /// </summary>
        public Point3D CameraPosition
        {
            get
            {
                return this.cameraPosition;
            }

            set
            {
                this.cameraPosition = value;
                this.RaisePropertyChanged("CameraPosition");
            }
        }

        /// <summary>
        /// Gets or sets the ground geometry.
        /// </summary>
        /// <value>
        /// The ground.
        /// </value>
        public MeshGeometry3D GroundGeometry
        {
            get
            {
                return this.groundGeometry;
            }

            set
            {
                this.groundGeometry = value;
                this.RaisePropertyChanged("GroundGeometry");
            }
        }

        /// <summary>
        /// Gets or sets the solution geometry.
        /// </summary>
        /// <value>
        /// The solution.
        /// </value>
        public MeshGeometry3D SolutionGeometry
        {
            get
            {
                return this.solutionGeometry;
            }

            set
            {
                this.solutionGeometry = value;
                this.RaisePropertyChanged("SolutionGeometry");
            }
        }

        /// <summary>
        /// Gets or sets the walls geometry.
        /// </summary>
        /// <value>
        /// The walls.
        /// </value>
        public MeshGeometry3D WallsGeometry
        {
            get
            {
                return this.wallsGeometry;
            }

            set
            {
                this.wallsGeometry = value;
                this.RaisePropertyChanged("WallsGeometry");
            }
        }

        /// <summary>
        /// Coerces the specified position.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <returns>
        /// Null or the coerced position.
        /// </returns>
        public Point3D? CoercePosition(Point3D position)
        {
            this.CameraPosition = position;

            // poor man's collision detection
            int m = this.maze.GetLength(0);
            int n = this.maze.GetLength(1);
            var i = (int)(position.X + m * 0.5 + 0.5);
            var j = (int)(position.Y + n * 0.5 + 0.5);
            bool insideWall = false;
            if (i >= 0 && i < m && j >= 0 && j < n && position.Z >= 0 && position.Z < 2)
            {
                insideWall = this.maze[i, j];
            }

            if (insideWall)
            {
                var delta = position - this.previousPoint;
                return this.previousPoint - delta * 2;
            }

            this.previousPoint = position;
            return null;
        }

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        protected void RaisePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Creates the ground geometry.
        /// </summary>
        /// <param name="themaze">
        /// The maze.
        /// </param>
        /// <param name="padding">
        /// The padding.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        /// <returns>
        /// The geometry.
        /// </returns>
        private MeshGeometry3D CreateGroundGeometry(bool[,] themaze, int padding = 0, double z = 0)
        {
            int m = themaze.GetUpperBound(0) + 1;
            int n = themaze.GetUpperBound(1) + 1;
            var builder = new MeshBuilder();
            builder.AddQuad(
                this.GetPosition(-1 - padding, -1 - padding, z),
                this.GetPosition(m + padding, -1 - padding, z),
                this.GetPosition(m + padding, n + padding, z),
                this.GetPosition(-1 - padding, n + padding, z));
            return builder.ToMesh();
        }

        /// <summary>
        /// Creates the maze.
        /// </summary>
        private void CreateMaze()
        {
            // Generate the maze
            var mazeGenerator = new MazeGenerator2 { Width = 21, Height = 31 };
            this.maze = mazeGenerator.Generate();

            // Solve the maze
            var solver = new MazeSolver1();
            var start = new Cell(1, 1);
            var end = new Cell(this.maze.GetLength(0) - 2, this.maze.GetLength(1) - 2);
            var solution = solver.Solve(this.maze, start, end);

            int m = this.maze.GetUpperBound(0) + 1;
            int n = this.maze.GetUpperBound(1) + 1;
            this.offsetX = -m * 0.5;
            this.offsetY = -n * 0.5;

            this.WallsGeometry = this.CreateMazeGeometry(this.maze);
            this.GroundGeometry = this.CreateGroundGeometry(this.maze, 1, -0.005);
            this.SolutionGeometry = this.CreateSolutionGeometry(solution);
        }

        /// <summary>
        /// Creates the maze geometry.
        /// </summary>
        /// <param name="themaze">
        /// The maze.
        /// </param>
        /// <param name="height">
        /// The height of the blocks.
        /// </param>
        /// <param name="size">
        /// The size of the blocks.
        /// </param>
        /// <returns>
        /// The geometry.
        /// </returns>
        private MeshGeometry3D CreateMazeGeometry(bool[,] themaze, double height = 2, double size = 0.995)
        {
            var builder = new MeshBuilder();
            int m = themaze.GetUpperBound(0) + 1;
            int n = themaze.GetUpperBound(1) + 1;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (themaze[i, j])
                    {
                        builder.AddBox(this.GetPosition(i, j, height * 0.5), size, size, height);
                    }
                }
            }

            return builder.ToMesh();
        }

        /// <summary>
        /// The create solution geometry.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="height">The height.</param>
        /// <param name="diameter">The diameter.</param>
        /// <returns>The solution tube geometry.</returns>
        private MeshGeometry3D CreateSolutionGeometry(IEnumerable<Cell> solution, double height = 0.25, double diameter = 0.25)
        {
            var builder = new MeshBuilder();
            var path = solution.Select(cell => this.GetPosition(cell, height)).ToList();

            var spline = CanonicalSplineHelper.CreateSpline(path, 0.7, null, false, 0.05);
            builder.AddTube(spline, diameter, 13, false);
            return builder.ToMesh();
        }

        /// <summary>
        /// Get the position for the specified cell.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="j">
        /// The j.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        /// <returns>
        /// The position.
        /// </returns>
        private Point3D GetPosition(int i, int j, double z)
        {
            return new Point3D(i + this.offsetX, j + this.offsetY, z);
        }

        /// <summary>
        /// Get the position for the specified cell.
        /// </summary>
        /// <param name="cell">
        /// The cell.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        /// <returns>
        /// The position.
        /// </returns>
        private Point3D GetPosition(Cell cell, double z)
        {
            return this.GetPosition(cell.I, cell.J, z);
        }

    }
}