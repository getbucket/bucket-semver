/*
 * This file is part of the Bucket package.
 *
 * (c) Yu Meng Han <menghanyu1994@gmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://github.com/getbucket/semver/wiki
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bucket.Semver.Tests
{
    [TestClass]
    public class TestsComparator
    {
        [TestMethod]
        public void TestCompare()
        {
            Assert.AreEqual(true, Comparator.GreaterThan("2.0", "1.0"));
            Assert.AreEqual(true, Comparator.GreaterThanOrEqual("2.0", "2.0"));
            Assert.AreEqual(true, Comparator.LessThan("1.0", "2.0"));
            Assert.AreEqual(true, Comparator.LessThanOrEqual("1.0", "1.0"));
            Assert.AreEqual(true, Comparator.Equal("1.0", "1.0"));
            Assert.AreEqual(true, Comparator.NotEqual("2.0", "1.0"));
            Assert.AreEqual(true, Comparator.Compare("2.0", "!=", "1.0"));
        }
    }
}
