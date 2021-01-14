using LatrunculiCore.Desk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LatrunculiCoreTests
{
    [TestClass]
    public class AllPositionsTest
    {
        [TestMethod]
        public void PositionsCount()
        {
            Assert.AreEqual(56, new AllPositions(new DeskSize(7, 8)).Count(), "Size 56 test");
            Assert.AreEqual(64, new AllPositions(new DeskSize(8, 8)).Count(), "Size 64 test");
            Assert.AreEqual(0, new AllPositions(new DeskSize(0, 8)).Count(), "Size 0 test");
        }

        [TestMethod]
        public void AllPositionsNotNull()
        {
            var positions1 = new AllPositions(new DeskSize(7, 8));
            var positions2 = new AllPositions(new DeskSize(7, 0));
            CollectionAssert.AllItemsAreInstancesOfType(positions1.ToList(), typeof(ChessBoxPosition), "Some null position test");
            CollectionAssert.AllItemsAreInstancesOfType(positions2.ToList(), typeof(ChessBoxPosition), "Some null position test2");
            CollectionAssert.AllItemsAreInstancesOfType(positions1.Positions, typeof(ChessBoxPosition), "Some null position test3");
            CollectionAssert.AllItemsAreInstancesOfType(positions2.Positions, typeof(ChessBoxPosition), "Some null position test4");
        }
    }
}
