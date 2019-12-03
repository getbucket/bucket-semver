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
using System;
using System.Collections.Generic;
using BConstraint = Bucket.Semver.Constraint.Constraint;

namespace Bucket.Semver.Tests.Constraint
{
    [TestClass]
    public class TestsConstraint
    {
        public static IEnumerable<object[]> SuccessfulVersionMatches()
        {
            return new[]
            {
                // >    require    provide
                new[] { "==", "1", "==", "1" },
                new[] { ">=", "1", ">=", "2" },
                new[] { ">=", "2", ">=", "1" },
                new[] { ">=", "2", ">", "1" },
                new[] { "<=", "2", ">=", "1" },
                new[] { ">=", "1", "<=", "2" },
                new[] { "==", "2", ">=", "2" },
                new[] { "!=", "1", "!=", "1" },
                new[] { "!=", "1", "==", "2" },
                new[] { "!=", "1", "<", "1" },
                new[] { "!=", "1", "<=", "1" },
                new[] { "!=", "1", ">", "1" },
                new[] { "!=", "1", ">=", "1" },
                new[] { "==", "dev-foo-bar", "==", "dev-foo-bar" },
                new[] { "==", "dev-events+issue-17", "==", "dev-events+issue-17" },
                new[] { "==", "dev-foo-xyz", "==", "dev-foo-xyz" },
                new[] { ">=", "dev-foo-bar", ">=", "dev-foo-xyz" },
                new[] { "<=", "dev-foo-bar", "<", "dev-foo-xyz" },
                new[] { "!=", "dev-foo-bar", "<", "dev-foo-xyz" },
                new[] { ">=", "dev-foo-bar", "!=", "dev-foo-bar" },
                new[] { "!=", "dev-foo-bar", "!=", "dev-foo-xyz" },
            };
        }

        public static IEnumerable<object[]> FailingVersionMatches()
        {
            return new[]
            {
                // >    require    provide
                new[] { "==", "1", "==", "2" },
                new[] { ">=", "2", "<=", "1" },
                new[] { ">=", "2", "<", "2" },
                new[] { "<=", "2", ">", "2" },
                new[] { ">", "2", "<=", "2" },
                new[] { "<=", "1", ">=", "2" },
                new[] { ">=", "2", "<=", "1" },
                new[] { "==", "2", "<", "2" },
                new[] { "!=", "1", "==", "1" },
                new[] { "==", "1", "!=", "1" },
                new[] { "==", "dev-foo-bar", "==", "dev-foo-abr" },
                new[] { "==", "dev-foo-bar", "==", "dev-foo-rab" },
                new[] { "<=", "dev-foo-bar", ">=", "dev-foo-baz" },
                new[] { ">=", "dev-foo-bap", "<", "dev-foo-bad" },
                new[] { "<", "0.12", "==", "dev-foo" }, // branches are not comparable
                new[] { ">", "0.12", "==", "dev-foo" }, // branches are not comparable
            };
        }

        [TestMethod]
        [DynamicData("SuccessfulVersionMatches", DynamicDataSourceType.Method)]
        public void TestVersionMatchSucceeds(string requireOperator, string requireVersion, string provideOperator, string provideVersion)
        {
            var versionRequire = new BConstraint(requireOperator, requireVersion);
            var versionProvide = new BConstraint(provideOperator, provideVersion);

            Assert.IsTrue(versionRequire.Matches(versionProvide));
        }

        [TestMethod]
        [DynamicData("FailingVersionMatches", DynamicDataSourceType.Method)]
        public void TestVersionMatchFails(string requireOperator, string requireVersion, string provideOperator, string provideVersion)
        {
            var versionRequire = new BConstraint(requireOperator, requireVersion);
            var versionProvide = new BConstraint(provideOperator, provideVersion);

            Assert.IsFalse(versionRequire.Matches(versionProvide));
        }

        [TestMethod]
        [DataRow("invalid", "1.2.3", typeof(ArgumentException))]
        [DataRow("!", "1.2.3", typeof(ArgumentException))]
        [DataRow("invalid", "equals", typeof(ArgumentException))]
        public void TestInvalidOperators(string requireOperator, string version, Type expected)
        {
            Assert.That.ThrowsException(expected, () =>
            {
#pragma warning disable CA1806
                new BConstraint(requireOperator, version);
#pragma warning restore CA1806
            });
        }

        [TestMethod]
        public void TestComparableBranches()
        {
            var versionRequire = new BConstraint(">", "0.12");
            var versionProvide = new BConstraint("==", "dev-foo");

            Assert.IsFalse(versionRequire.Matches(versionProvide));
            Assert.IsFalse(versionRequire.MatchSpecific(versionProvide, true));

            versionRequire = new BConstraint("<", "0.12");
            versionProvide = new BConstraint("==", "dev-foo");

            Assert.IsFalse(versionRequire.Matches(versionProvide));
            Assert.IsTrue(versionRequire.MatchSpecific(versionProvide, true));
        }
    }
}
