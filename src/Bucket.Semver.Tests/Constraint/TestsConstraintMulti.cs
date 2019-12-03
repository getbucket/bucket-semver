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
using BConstraint = Bucket.Semver.Constraint.Constraint;

namespace Bucket.Semver.Tests.Constraint
{
    [TestClass]
    public class TestsConstraintMulti
    {
        [TestMethod]
        public void TestMultiVersionMatchSucceeds()
        {
            var versionRequireStart = new BConstraint(">", "1.0");
            var versionRequireEnd = new BConstraint("<", "1.2");
            var versionProvide = new BConstraint("==", "1.1");

            var multiRequire = new ConstraintMulti(versionRequireStart, versionRequireEnd);
            Assert.IsTrue(multiRequire.Matches(versionProvide));
        }

        [TestMethod]
        public void TestMultiVersionMatchDisjunctive()
        {
            var versionRequireStart = new BConstraint(">", "1.0");
            var versionRequireEnd = new BConstraint("<", "1.2");
            var versionProvide = new BConstraint("==", "0.1");

            var multiRequire = new ConstraintMulti(versionRequireStart, versionRequireEnd);
            Assert.IsFalse(multiRequire.Matches(versionProvide));

            multiRequire = new ConstraintMulti(versionRequireStart, versionRequireEnd)
            {
                IsConjunctive = false,
            };
            Assert.IsTrue(multiRequire.Matches(versionProvide));
        }

        [TestMethod]
        public void TestToString()
        {
            var versionRequireStart = new BConstraint(">", "1.0");
            var versionRequireEnd = new BConstraint("<", "1.2");
            var multiRequire = new ConstraintMulti(versionRequireStart, versionRequireEnd);

            Assert.AreEqual("[> 1.0 < 1.2]", multiRequire.ToString());
        }

        [TestMethod]
        public void TestSetPrettyString()
        {
            var versionRequireStart = new BConstraint(">", "1.0");
            var versionRequireEnd = new BConstraint("<", "1.2");
            var multiRequire = new ConstraintMulti(versionRequireStart, versionRequireEnd);

            Assert.AreEqual("[> 1.0 < 1.2]", multiRequire.GetPrettyString());
            multiRequire.SetPrettyString("pretty string.");
            Assert.AreEqual("[> 1.0 < 1.2]", multiRequire.ToString());
            Assert.AreEqual("pretty string.", multiRequire.GetPrettyString());
        }

        [TestMethod]
        public void TestGetConstraints()
        {
            var versionRequireStart = new BConstraint(">", "1.0");
            var versionRequireEnd = new BConstraint("<", "1.2");
            var multiRequire = new ConstraintMulti(versionRequireStart, versionRequireEnd);

            CollectionAssert.AreEqual(
                new[] { versionRequireStart, versionRequireEnd },
                multiRequire.GetConstraints());
        }

        [TestMethod]
        public void TestMultiVersionProvidedMatchSucceeds()
        {
            var versionRequireStart = new BConstraint(">", "1.0");
            var versionRequireEnd = new BConstraint("<", "1.2");
            var versionProvideStart = new BConstraint(">=", "1.1");
            var versionProvideEnd = new BConstraint("<", "2.0");

            var multiRequire = new ConstraintMulti(versionRequireStart, versionRequireEnd);
            var multiProvide = new ConstraintMulti(versionProvideStart, versionProvideEnd);

            Assert.IsTrue(multiRequire.Matches(multiProvide));
        }

        [TestMethod]
        public void TestMultiVersionMatchFails()
        {
            var versionRequireStart = new BConstraint(">", "1.0");
            var versionRequireEnd = new BConstraint("<", "1.2");
            var versionProvide = new BConstraint("==", "1.2");
            var multiRequire = new ConstraintMulti(versionRequireStart, versionRequireEnd);

            Assert.IsFalse(multiRequire.Matches(versionProvide));
        }

        [TestMethod]
        public void TestMultUnstableVersion()
        {
            var versionRequire = new BConstraint("==", "3.0.0.0-alpha");
            var versionProvideStart = new BConstraint(">=", "3.0.0.0-dev");
            var versionProvideEnd = new BConstraint("<", "4.0.0.0-dev");

            var multiProvide = new ConstraintMulti(versionProvideStart, versionProvideEnd);

            Assert.IsTrue(multiProvide.Matches(versionRequire));
        }
    }
}
