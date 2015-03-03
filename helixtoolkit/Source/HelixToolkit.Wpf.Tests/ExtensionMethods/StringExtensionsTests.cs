// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void SplitOnWhitespace()
        {
            var s1 = "1 2  3".SplitOnWhitespace();
            Assert.AreEqual(3, s1.Length);
            Assert.AreEqual("1", s1[0]);
            Assert.AreEqual("2", s1[1]);
            Assert.AreEqual("3", s1[2]);
        }

        [Test]
        public void SplitOnWhitespace_IncludingStartAndEndWhitespace()
        {
            var s1 = " 1 2  3 ".SplitOnWhitespace();
            Assert.AreEqual(3, s1.Length);
            Assert.AreEqual("1", s1[0]);
            Assert.AreEqual("2", s1[1]);
            Assert.AreEqual("3", s1[2]);
        }

        [Test]
        public void SplitOnWhitespace_IncludingTabsAndNewline()
        {
            var s1 = " 1 \t 2 \n  3 ".SplitOnWhitespace();
            Assert.AreEqual(3, s1.Length);
            Assert.AreEqual("1", s1[0]);
            Assert.AreEqual("2", s1[1]);
            Assert.AreEqual("3", s1[2]);
        }
    }
}