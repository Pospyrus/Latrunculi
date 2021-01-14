using LatrunculiCore.Desk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LatrunculiCoreTests
{
    [TestClass]
    public class DeskSizeTest
    {
        [TestMethod]
        public void PositivePositionRange()
        {
            new DeskSize(5, 10);
            new DeskSize(0, 0);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DeskSize(-5, 10));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DeskSize(0, -10));
        }

        [TestMethod]
        public void PropertiesTest()
        {
            int expectedWidth = 10;
            int expectedHeight = 10;
            int expectedCount = 100;
            var size = new DeskSize(expectedWidth, expectedHeight);
            Assert.AreEqual(expectedWidth, size.Width, "DeskSize width");
            Assert.AreEqual(expectedHeight, size.Height, "DeskSize height");
            Assert.AreEqual(expectedCount, size.Count, "DeskSize count");
        }
    }
}
