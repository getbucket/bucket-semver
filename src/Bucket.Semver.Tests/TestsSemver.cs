/*
 * This file is part of the Bucket package.
 *
 * (c) Yu Meng Han <menghanyu1994@gmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://github.com/getbucket/bucket-semver/wiki
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bucket.Semver.Tests
{
    [TestClass]
    public class TestsSemver
    {
        [TestMethod]
        public void TestSatisfies()
        {
            Assert.AreEqual(true, Semver.Satisfies("1.8", "^1.5"));
            Assert.AreEqual(false, Semver.Satisfies("1.0", "^2.5"));
        }

        [TestMethod]
        public void TestSatisfiesBy()
        {
            var actual = Semver.SatisfiesBy(new[] { "1.8", "2.0", "1.6", "1.2" }, "^1.5");
            CollectionAssert.AreEqual(new[] { "1.8", "1.6" }, actual);
        }

        [TestMethod]
        public void TestSortAsc()
        {
            var actual = Semver.Sort(new[] { "1.1", "1.8", "1.3", "0.6", "2.1" });
            CollectionAssert.AreEqual(new[] { "0.6", "1.1", "1.3", "1.8", "2.1" }, actual);
        }

        [TestMethod]
        public void TestSortDesc()
        {
            var actual = Semver.Sort(new[] { "1.1", "1.8", "1.3", "0.6", "2.1" }, true);
            CollectionAssert.AreEqual(new[] { "2.1", "1.8", "1.3", "1.1", "0.6" }, actual);
        }
    }
}
