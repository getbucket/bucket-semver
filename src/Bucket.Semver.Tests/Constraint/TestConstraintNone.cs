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

using Bucket.Semver.Constraint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bucket.Semver.Tests.Constraint
{
    [TestClass]
    public class TestConstraintNone
    {
        [TestMethod]
        public void TestMatches()
        {
            var constraint = new ConstraintNone();

            Assert.IsTrue(constraint.Matches(new ConstraintNone()));
        }

        [TestMethod]
        public void TestToString()
        {
            var constraint = new ConstraintNone();

            Assert.AreEqual("[]", constraint.ToString());
        }

        [TestMethod]
        public void TestSetPrettyString()
        {
            var constraint = new ConstraintNone();
            Assert.AreEqual("[]", constraint.GetPrettyString());

            constraint.SetPrettyString("pretty string.");
            Assert.AreEqual("pretty string.", constraint.GetPrettyString());
        }
    }
}
