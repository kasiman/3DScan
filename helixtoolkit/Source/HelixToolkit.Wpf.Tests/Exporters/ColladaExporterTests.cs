﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColladaExporterTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml.Schema;
    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class ColladaExporterTests : ExporterTests
    {
        [Test]
        public void Export_SimpleModel_ValidOutput()
        {
            string path = "temp.dae";
            var e = new ColladaExporter();
            using (var stream = File.Create(path))
            {
                this.ExportSimpleModel(e, stream);
            }

            var result = this.Validate(path);
            Assert.IsNull(result, result);
        }

        private string Validate(string path)
        {
            var sc = new XmlSchemaSet();
            string dir = @"..\..\..\..\Schemas\Collada\";
            sc.Add("http://www.collada.org/2008/03/COLLADASchema", dir + "collada_schema_1_5.xsd");
            return this.Validate(path, sc);
        }
    }
}