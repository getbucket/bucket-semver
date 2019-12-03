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

using Bucket.Semver.Constraint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using BConstraint = Bucket.Semver.Constraint.Constraint;

namespace Bucket.Semver.Tests
{
    [TestClass]
    public class TestsVersionParser
    {
        private IVersionParser parser;

        [TestInitialize]
        public void Inite()
        {
            parser = new VersionParser();
        }

        [TestMethod]
        [DataRow(Stabilities.Stable, "1")]
        [DataRow(Stabilities.Stable, "1.0")]
        [DataRow(Stabilities.Stable, "1.*-stable")]
        [DataRow(Stabilities.Stable, "3.2.1")]
        [DataRow(Stabilities.Stable, "v3.2.1")]
        [DataRow(Stabilities.Stable, "3.1.2-p1")]
        [DataRow(Stabilities.Stable, "3.1.2-pl2")]
        [DataRow(Stabilities.Stable, "3.1.2-patch")]
        [DataRow(Stabilities.Alpha, "1.2.0a1")]
        [DataRow(Stabilities.Alpha, "1.2_a1")]
        [DataRow(Stabilities.Alpha, "3.1.2-alpha5")]
        [DataRow(Stabilities.Alpha, "1.0.0-alpha11+cs-1.1.0")]
        [DataRow(Stabilities.Dev, "v2.0.x-dev")]
        [DataRow(Stabilities.Dev, "v2.0.x-dev#abc123")]
        [DataRow(Stabilities.Dev, "v2.0.x-dev#trunk/@123")]
        [DataRow(Stabilities.Dev, "dev-master")]
        [DataRow(Stabilities.Dev, "3.1.2-dev")]
        [DataRow(Stabilities.Dev, "dev-feature+issue-1")]
        [DataRow(Stabilities.Beta, "3.1.2-beta")]
        [DataRow(Stabilities.Beta, "2.0B1")]
        [DataRow(Stabilities.RC, "3.0-RC2")]
        [DataRow(Stabilities.RC, "2.0.0rc1")]
        public void TestParseStability(Stabilities expected, string version)
        {
            Assert.AreEqual(expected, VersionParser.ParseStability(version));
        }

        [TestMethod]
        [DataRow(VersionParser.VersionMaster, "dev-master as 1.0.0")]
        [DataRow(VersionParser.VersionMaster, "master")]
        [DataRow(VersionParser.VersionMaster, "trunk")]
        [DataRow(VersionParser.VersionMaster, "default")]
        [DataRow("1.0.0.0-beta5", "1.0.0-beta.5+foo")]
        [DataRow("1.0.0.0", "1.0.0")]
        [DataRow("1.2.3.4", "1.2.3.4")]
        [DataRow("1.0.0.0-RC1-dev", "1.0.0RC1dev")]
        [DataRow("1.0.0.0-RC15-dev", "1.0.0-rC15-dev")]
        [DataRow("1.0.0.0-RC15-dev", "1.0.0.RC.15-dev")]
        [DataRow("1.0.0.0-RC1", "1.0.0-rc1")]
        [DataRow("1.0.0.0-patch3-dev", "1.0.0-pl3-dev")]
        [DataRow("1.0.0.0-dev", "1.0-dev")]
        [DataRow("0.0.0.0", "0")]
        [DataRow("10.4.13.0-beta", "10.4.13-beta")]
        [DataRow("10.4.13.0-beta2", "10.4.13beta2")]
        [DataRow("10.4.13.0-beta2", "10.4.13beta.2")]
        [DataRow("10.4.13.0-beta", "10.4.13-b")]
        [DataRow("10.4.13.0-beta5", "10.4.13-b5")]
        [DataRow("1.0.0.0", "v1.0.0.0")]
        [DataRow("2010.01.0.0", "2010.01")]
        [DataRow("2010.01.02.0", "2010.01.02")]
        [DataRow("2010.1.555.0", "2010.1.555")]
        [DataRow("2010.10.200.0", "2010.10.200")]
        [DataRow("1.0.0.0-patch3-dev", "1.0.0-pl3-dev")]
        [DataRow("20100102", "v20100102")]
        [DataRow("2010.01.02", "2010-01-02")]
        [DataRow("2010.1.555.0", "2010.1.555")]
        [DataRow("2010.01.02.5", "2010-01-02.5")]
        [DataRow("20100102.203040", "20100102-203040")]
        [DataRow("20100102.9999999.9999999.9999999-dev", "20100102.x-dev")]
        [DataRow("20100102.203040.9999999.9999999-dev", "20100102.203040.x-dev")]
        [DataRow("20100102203040.10", "20100102203040-10")]
        [DataRow("20100102.203040-patch1", "20100102-203040-p1")]
        [DataRow("201903.0", "201903.0")]
        [DataRow("201903.9999999.9999999.9999999-dev", "201903.x-dev")]
        [DataRow("201903.0-patch2", "201903.0-p2")]
        [DataRow("9999999-dev", "dev-master")]
        [DataRow("9999999-dev", "dev-trunk")]
        [DataRow("9999999-dev", "dev-default")]
        [DataRow("1.9999999.9999999.9999999-dev", "1.x-dev")]
        [DataRow("dev-feature-foo", "dev-feature-foo")]
        [DataRow("dev-FOOBAR", "DEV-FOOBAR")]
        [DataRow("dev-feature/foo", "dev-feature/foo")]
        [DataRow("dev-feature+issue-1", "dev-feature+issue-1")]
        [DataRow("9999999-dev", "dev-master as 1.0.0")]
        [DataRow("1.0.0.0-beta5", "1.0.0-beta.5+foo")]
        [DataRow("1.0.0.0", "1.0.0.0+foo")]
        [DataRow("1.0.0.0-alpha3.1", "1.0.0.0-alpha3.1+foo")]
        [DataRow("1.0.0.0-alpha2.1", "1.0.0.0-a2.1+foo")]
        [DataRow("1.0.0.0-alpha2.1-3", "1.0.0.0-alpha2.1-3+foo")]
        [DataRow("1.0.0.0", "1.0.0.0+foo as 2.0")]
        public void TestNormalizeSuccessed(string expected, string version)
        {
            Assert.AreEqual(expected, parser.Normalize(version));
        }

        [TestMethod]
        [DataRow("1.9999999.9999999.9999999-dev", "v1.x")]
        [DataRow("1.9999999.9999999.9999999-dev", "v1.*")]
        [DataRow("1.0.9999999.9999999-dev", "v1.0")]
        [DataRow("2.0.9999999.9999999-dev", "v2.0")]
        [DataRow("1.0.9999999.9999999-dev", "v1.0.X")]
        [DataRow("1.0.3.9999999-dev", "v1.0.3.*")]
        [DataRow("2.4.0.9999999-dev", "v2.4.0")]
        [DataRow("2.4.4.9999999-dev", "v2.4.4")]
        [DataRow("9999999-dev", "master")]
        [DataRow("9999999-dev", "trunk")]
        [DataRow("9999999-dev", "default")]
        [DataRow("dev-feature-a", "feature-a")]
        [DataRow("dev-feature-a", "dev-feature-a")]
        [DataRow("dev-FOOBAR", "FOOBAR")]
        [DataRow("dev-feature+issue-1", "feature+issue-1")]
        [DataRow("dev-feature+issue-1", "-feature+issue-1")]
        [DataRow("dev-rpi", "rpi")]
        [DataRow("dev-fooo", "fooo")]
        public void TestNormalizeBranchSuccessed(string expected, string version)
        {
            Assert.AreEqual(expected, parser.NormalizeBranch(version));
        }

#pragma warning disable SA1204
        public static IEnumerable<object[]> TildeConstraints()
#pragma warning restore SA1204
        {
            return new[]
            {
                new object[] { null, new BConstraint(">=", "1.0.0.0-dev"), new BConstraint("<", "2.0.0.0-dev"), "~v1" },
                new object[] { null, new BConstraint(">=", "1.0.0.0-dev"), new BConstraint("<", "2.0.0.0-dev"), "~1.0" },
                new object[] { null, new BConstraint(">=", "1.0.0.0-dev"), new BConstraint("<", "1.1.0.0-dev"), "~1.0.0" },
                new object[] { null, new BConstraint(">=", "1.2.0.0-dev"), new BConstraint("<", "2.0.0.0-dev"), "~1.2" },
                new object[] { null, new BConstraint(">=", "1.2.3.0-dev"), new BConstraint("<", "1.3.0.0-dev"), "~1.2.3" },
                new object[] { null, new BConstraint(">=", "1.2.3.4-dev"), new BConstraint("<", "1.2.4.0-dev"), "~1.2.3.4" },
                new object[] { null, new BConstraint(">=", "1.2.0.0-beta"), new BConstraint("<", "2.0.0.0-dev"), "~1.2-beta" },
                new object[] { null, new BConstraint(">=", "1.2.0.0-beta2"), new BConstraint("<", "2.0.0.0-dev"), "~1.2-b2" },
                new object[] { null, new BConstraint(">=", "1.2.0.0-beta2"), new BConstraint("<", "2.0.0.0-dev"), "~1.2-BETA2" },
                new object[] { null, new BConstraint(">=", "1.2.2.0-dev"), new BConstraint("<", "1.3.0.0-dev"), "~1.2.2-dev" },
                new object[] { null, new BConstraint(">=", "1.2.2.0"), new BConstraint("<", "1.3.0.0-dev"), "~1.2.2-stable" },
                new object[] { null, new BConstraint(">=", "201905.0-dev"), new BConstraint("<", "201906.0.0.0-dev"), "~201905.0" },
                new object[] { null, new BConstraint(">=", "201905.0-beta"), new BConstraint("<", "201906.0.0.0-dev"), "~201905.0-beta" },
                new object[] { null, new BConstraint(">=", "201905.0"), new BConstraint("<", "201906.0.0.0-dev"), "~201905.0-stable" },
                new object[] { null, new BConstraint(">=", "201905.0-beta"), new BConstraint("<", "201906.0.0.0-dev"), "~201905.0-beta" },
                new object[] { null, new BConstraint(">=", "201905.205830.1"), new BConstraint("<", "201905.205831.0.0-dev"), "~201905.205830.1-stable" },
            };
        }

        [TestMethod]
        [DynamicData("TildeConstraints", DynamicDataSourceType.Method)]
        public void TestParseTildeWildcard(string delta, IConstraint min, IConstraint max, string version)
        {
            var expected = CreateWildcardConstraintExpected(min, max);
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(version).ToString(), $"{version} tilde parse faild. {delta}");
        }

#pragma warning disable SA1204
        public static IEnumerable<object[]> CaretConstraints()
#pragma warning restore SA1204
        {
            return new[]
            {
                new object[] { null, new BConstraint(">=", "1.0.0.0-dev"), new BConstraint("<", "2.0.0.0-dev"), "^v1" },
                new object[] { null, new BConstraint(">=", "0.0.0.0-dev"), new BConstraint("<", "1.0.0.0-dev"), "^0" },
                new object[] { null, new BConstraint(">=", "0.0.0.0-dev"), new BConstraint("<", "0.1.0.0-dev"), "^0.0" },
                new object[] { null, new BConstraint(">=", "1.2.0.0-dev"), new BConstraint("<", "2.0.0.0-dev"), "^1.2" },
                new object[] { null, new BConstraint(">=", "1.2.3.0-beta2"), new BConstraint("<", "2.0.0.0-dev"), "^1.2.3-beta.2" },
                new object[] { null, new BConstraint(">=", "1.2.3.4-dev"), new BConstraint("<", "2.0.0.0-dev"), "^1.2.3.4" },
                new object[] { null, new BConstraint(">=", "1.2.3.0-dev"), new BConstraint("<", "2.0.0.0-dev"), "^1.2.3" },
                new object[] { null, new BConstraint(">=", "0.2.3.0-dev"), new BConstraint("<", "0.3.0.0-dev"), "^0.2.3" },
                new object[] { null, new BConstraint(">=", "0.2.3.0-dev"), new BConstraint("<", "0.3.0.0-dev"), "^0.2.3" },
                new object[] { null, new BConstraint(">=", "0.2.0.0-dev"), new BConstraint("<", "0.3.0.0-dev"), "^0.2" },
                new object[] { null, new BConstraint(">=", "0.2.0.0-dev"), new BConstraint("<", "0.3.0.0-dev"), "^0.2.0" },
                new object[] { null, new BConstraint(">=", "0.0.3.0-dev"), new BConstraint("<", "0.0.4.0-dev"), "^0.0.3" },
                new object[] { null, new BConstraint(">=", "0.0.3.0-alpha"), new BConstraint("<", "0.0.4.0-dev"), "^0.0.3-alpha" },
                new object[] { null, new BConstraint(">=", "0.0.3.0-dev"), new BConstraint("<", "0.0.4.0-dev"), "^0.0.3-dev" },
                new object[] { null, new BConstraint(">=", "0.0.3.0"), new BConstraint("<", "0.0.4.0-dev"), "^0.0.3-stable" },
                new object[] { null, new BConstraint(">=", "201905.0-dev"), new BConstraint("<", "201906.0.0.0-dev"), "^201905.0" },
                new object[] { null, new BConstraint(">=", "201905.0-beta"), new BConstraint("<", "201906.0.0.0-dev"), "^201905.0-beta" },
                new object[] { null, new BConstraint(">=", "201905.205830.1"), new BConstraint("<", "201906.0.0.0-dev"), "^201905.205830.1-stable" },
            };
        }

        [TestMethod]
        [DynamicData("CaretConstraints", DynamicDataSourceType.Method)]
        public void TestParseCaretWildcard(string delta, IConstraint min, IConstraint max, string version)
        {
            var expected = CreateWildcardConstraintExpected(min, max);
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(version).ToString(), $"{version} caret parse faild. {delta}");
        }

#pragma warning disable SA1204
        public static IEnumerable<object[]> XRangConstraints()
#pragma warning restore SA1204
        {
            return new[]
            {
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "3.0.0.0-dev"), "v2.*" },
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "3.0.0.0-dev"), "2.*.*" },
                new object[] { null, new BConstraint(">=", "20.0.0.0-dev"), new BConstraint("<", "21.0.0.0-dev"), "20.*" },
                new object[] { null, new BConstraint(">=", "20.0.0.0-dev"), new BConstraint("<", "21.0.0.0-dev"), "20.*.*" },
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "2.1.0.0-dev"), "2.0.*" },
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "3.0.0.0-dev"), "2.x" },
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "3.0.0.0-dev"), "2.x.x" },
                new object[] { null, new BConstraint(">=", "2.2.0.0-dev"), new BConstraint("<", "2.3.0.0-dev"), "2.2.x" },
                new object[] { null, new BConstraint(">=", "2.10.0.0-dev"), new BConstraint("<", "2.11.0.0-dev"), "2.10.X" },
                new object[] { null, new BConstraint(">=", "2.1.3.0-dev"), new BConstraint("<", "2.1.4.0-dev"), "2.1.3.*" },
                new object[] { null, null, new BConstraint("<", "1.0.0.0-dev"), "0.*" },
                new object[] { null, null, new BConstraint("<", "1.0.0.0-dev"), "0.*.*" },
                new object[] { null, null, new BConstraint("<", "1.0.0.0-dev"), "0.x" },
                new object[] { null, null, new BConstraint("<", "1.0.0.0-dev"), "0.x.x" },
            };
        }

        [TestMethod]
        [DynamicData("XRangConstraints", DynamicDataSourceType.Method)]
        public void TestParseXRangWildcard(string delta, IConstraint min, IConstraint max, string version)
        {
            var expected = CreateWildcardConstraintExpected(min, max);
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(version).ToString(), $"{version} xrang parse faild. {delta}");
        }

#pragma warning disable SA1204
        public static IEnumerable<object[]> HyphenConstraints()
#pragma warning restore SA1204
        {
            return new[]
            {
                new object[] { null, new BConstraint(">=", "1.0.0.0-dev"), new BConstraint("<", "3.0.0.0-dev"), "v1 - v2" },
                new object[] { null, new BConstraint(">=", "1.2.3.0-dev"), new BConstraint("<=", "2.3.4.5"), "1.2.3 - 2.3.4.5" },
                new object[] { null, new BConstraint(">=", "1.2.0.0-beta"), new BConstraint("<", "2.4.0.0-dev"), "1.2-beta - 2.3" },
                new object[] { null, new BConstraint(">=", "1.2.0.0-beta"), new BConstraint("<=", "2.3.0.0-dev"), "1.2-beta - 2.3-dev" },
                new object[] { null, new BConstraint(">=", "1.2.0.0-RC"), new BConstraint("<=", "2.3.1.0"), "1.2-RC - 2.3.1" },
                new object[] { null, new BConstraint(">=", "1.2.3.0-alpha"), new BConstraint("<=", "2.3.0.0-RC"), "1.2.3-alpha - 2.3-RC" },
                new object[] { null, new BConstraint(">=", "1.0.0.0-dev"), new BConstraint("<", "2.1.0.0-dev"), "1 - 2.0" },
                new object[] { null, new BConstraint(">=", "1.0.0.0-dev"), new BConstraint("<", "2.2.0.0-dev"), "1 - 2.1" },
                new object[] { null, new BConstraint(">=", "1.2.0.0-dev"), new BConstraint("<=", "2.1.0.0"), "1.2 - 2.1.0" },
                new object[] { null, new BConstraint(">=", "1.3.0.0-dev"), new BConstraint("<=", "2.1.3.0"), "1.3 - 2.1.3" },
                new object[] { null, new BConstraint(">=", "201905.0"), new BConstraint("<=", "202005.0-dev"), "201905.0-stable - 202005.0-dev" },
                new object[] { null, new BConstraint(">=", "201905.02.0-alpha"), new BConstraint("<=", "202005.02.1-dev"), "201905.02.0-alpha - 202005.02.1-dev" },
                new object[] { null, new BConstraint(">=", "201905.02-dev"), new BConstraint("<", "202005.3.0.0-dev"), "201905.02 - 202005.02" },
            };
        }

        [TestMethod]
        [DynamicData("HyphenConstraints", DynamicDataSourceType.Method)]
        public void TestParseHyphenWildcard(string delta, IConstraint min, IConstraint max, string version)
        {
            var expected = CreateWildcardConstraintExpected(min, max);
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(version).ToString(), $"{version} hyphen parse faild. {delta}");
        }

#pragma warning disable SA1204
        public static IEnumerable<object[]> BasicConstraints()
#pragma warning restore SA1204
        {
            return new[]
            {
                new object[] { null, null, new BConstraint("<>", "1.0.0.0"), "<>1.0.0" },
                new object[] { null, null, new BConstraint("!=", "1.0.0.0"), "!=1.0.0" },
                new object[] { null, null, new BConstraint(">", "1.0.0.0"), ">1.0.0" },
                new object[] { null, null, new BConstraint("<", "1.2.3.4-dev"), "<1.2.3.4" },
                new object[] { null, null, new BConstraint("<=", "1.2.3.0"), "<=1.2.3" },
                new object[] { null, null, new BConstraint(">=", "1.2.3.0-dev"), ">=1.2.3" },
                new object[] { null, null, new BConstraint("=", "1.2.3.0"), "=1.2.3" },
                new object[] { null, null, new BConstraint("==", "1.2.3.0"), "==1.2.3" },
                new object[] { null, null, new BConstraint("=", "1.2.3.0"), "1.2.3" },
                new object[] { null, null, new BConstraint("=", "1.0.0.0"), "=1.0" },
                new object[] { null, null, new BConstraint("=", "1.2.3.0-beta5"), "1.2.3b5" },
                new object[] { null, null, new BConstraint("=", "1.2.3.0-alpha1"), "1.2.3a1" },
                new object[] { null, null, new BConstraint("=", "1.2.3.0-patch1234"), "1.2.3p1234" },
                new object[] { null, null, new BConstraint("=", "1.2.3.0-patch1234"), "1.2.3pl1234" },
                new object[] { null, null, new BConstraint(">=", "1.2.3.0-dev"), ">= 1.2.3" },
                new object[] { null, null, new BConstraint("<", "1.2.3.0-dev"), "< 1.2.3" },
                new object[] { null, null, new BConstraint(">", "1.2.3.0"), "> 1.2.3" },
                new object[] { null, null, new BConstraint(">=", "9999999-dev"), ">=dev-master" },
                new object[] { null, null, new BConstraint("=", "9999999-dev"), "dev-master" },
                new object[] { null, null, new BConstraint("=", "dev-feature-a"), "dev-feature-a" },
                new object[] { null, null, new BConstraint("=", "dev-some-fix"), "dev-some-fix" },
                new object[] { null, null, new BConstraint("=", "dev-CAPS"), "dev-CAPS" },
                new object[] { "ignores aliases", null, new BConstraint("=", "9999999-dev"), "dev-master as 1.0.0" },
                new object[] { null, null, new BConstraint("<", "1.2.3.4"), "<1.2.3.4-stable" },
                new object[] { null, null, new BConstraint(">=", "1.2.3.4"), ">=1.2.3.4-stable" },
                new object[] { null, null, new ConstraintNone(), "@dev" },
            };
        }

        [TestMethod]
        [DynamicData("BasicConstraints", DynamicDataSourceType.Method)]
        public void TestParseBasicWildcard(string delta, IConstraint min, IConstraint max, string version)
        {
            var expected = CreateWildcardConstraintExpected(min, max);
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(version).ToString(), $"{version} basic parse faild. {delta}");
        }

#pragma warning disable SA1204
        public static IEnumerable<object[]> IgnoresConstraints()
#pragma warning restore SA1204
        {
            return new[]
            {
                new object[] { "ignore stability flag", null, new BConstraint("=", "1.0.0.0"), "1.0@dev" },
                new object[] { "ignore reference on dev version", null, new BConstraint("=", "1.0.9999999.9999999-dev"), "1.0.x-dev#abcd123" },
                new object[] { "ignore reference on dev version", null, new BConstraint("=", "1.0.9999999.9999999-dev"), "1.0.x-dev#trunk/@123" },
            };
        }

        [TestMethod]
        [DynamicData("IgnoresConstraints", DynamicDataSourceType.Method)]
        public void TestParseIgnoresWildcard(string delta, IConstraint min, IConstraint max, string version)
        {
            var expected = CreateWildcardConstraintExpected(min, max);
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(version).ToString(), $"{version} ignores parse faild. {delta}");
        }

#pragma warning disable SA1204
        public static IEnumerable<object[]> FaildConstraints()
#pragma warning restore SA1204
        {
            return new[]
            {
                new object[] { "empty", null, new ConstraintNone(), string.Empty },
                new object[] { "null", null, new ConstraintNone(), null },
                new object[] { null, null, new BConstraint("=", "1.0.0.0"), "1.0#abcd123" },
                new object[] { null, null, new BConstraint("=", "1.0.0.0"), "1.0#trunk/@123" },
                new object[] { "invalid version", null, null, "1.0.0-meh" },
                new object[] { "operator abuse 1", null, null, ">2.0,,<=3.0" },
                new object[] { "operator abuse 2", null, null, ">2.0 ,, <=3.0" },
                new object[] { "operator abuse 3", null, null, ">2.0 ||| <=3.0" },
            };
        }

        [TestMethod]
        [DynamicData("FaildConstraints", DynamicDataSourceType.Method)]
        [ExpectedException(typeof(ParseException))]
        public void TestParseFaildWildcard(string delta, IConstraint min, IConstraint max, string version)
        {
            var expected = CreateWildcardConstraintExpected(min, max);
            Assert.AreEqual(expected?.ToString(), parser.ParseConstraints(version).ToString(), $"{version} faild parse faild. {delta}");
        }

#pragma warning disable SA1204
        public static IEnumerable<object[]> WildcardConstraints()
#pragma warning restore SA1204
        {
            return new[]
            {
                new object[] { null, null, new ConstraintNone(), "*" },
                new object[] { null, null, new ConstraintNone(), "*.*" },
                new object[] { null, null, new ConstraintNone(), "v*.*.*" },
                new object[] { null, null, new ConstraintNone(), "*.x.*" },
                new object[] { null, null, new ConstraintNone(), "x.X.x.*" },
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "3.0.0.0-dev"), "v2.*" },
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "3.0.0.0-dev"), "2.*.*" },
                new object[] { null, new BConstraint(">=", "20.0.0.0-dev"), new BConstraint("<", "21.0.0.0-dev"), "20.*" },
                new object[] { null, new BConstraint(">=", "20.0.0.0-dev"), new BConstraint("<", "21.0.0.0-dev"), "20.*.*" },
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "2.1.0.0-dev"), "2.0.*" },
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "3.0.0.0-dev"), "2.x" },
                new object[] { null, new BConstraint(">=", "2.0.0.0-dev"), new BConstraint("<", "3.0.0.0-dev"), "2.x.x" },
                new object[] { null, new BConstraint(">=", "2.2.0.0-dev"), new BConstraint("<", "2.3.0.0-dev"), "2.2.x" },
                new object[] { null, new BConstraint(">=", "2.10.0.0-dev"), new BConstraint("<", "2.11.0.0-dev"), "2.10.X" },
                new object[] { null, new BConstraint(">=", "2.1.3.0-dev"), new BConstraint("<", "2.1.4.0-dev"), "2.1.3.*" },
                new object[] { null, null, new BConstraint("<", "1.0.0.0-dev"), "0.*" },
                new object[] { null, null, new BConstraint("<", "1.0.0.0-dev"), "0.*.*" },
                new object[] { null, null, new BConstraint("<", "1.0.0.0-dev"), "0.x" },
                new object[] { null, null, new BConstraint("<", "1.0.0.0-dev"), "0.x.x" },
                new object[] { null, new BConstraint(">=", "0.2.0.0-dev"), new BConstraint("<", "0.3.0.0-dev"), "0.2.*" },
            };
        }

        [TestMethod]
        [DynamicData("WildcardConstraints", DynamicDataSourceType.Method)]
        public void TestParseWildcard(string delta, IConstraint min, IConstraint max, string version)
        {
            var expected = CreateWildcardConstraintExpected(min, max);
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(version).ToString(), $"{version} wildcard parse faild. {delta}");
        }

        [TestMethod]
        public void TestParseCaretConstraintsMulti()
        {
            var first = new ConstraintMulti(
                    new BConstraint(">=", "0.2.0.0-dev"),
                    new BConstraint("<", "0.3.0.0-dev"));

            var second = new ConstraintMulti(
                    new BConstraint(">=", "1.0.0.0-dev"),
                    new BConstraint("<", "2.0.0.0-dev"));

            var expected = new ConstraintMulti(first, second)
            {
                IsConjunctive = false,
            };
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints("^0.2 || ^1.0").ToString());
        }

        [TestMethod]
        public void TestParseConstraintsMultiCollapsesContiguous()
        {
            var first = new BConstraint(">=", "2.5.0.0-dev");
            var second = new BConstraint("<", "4.0.0.0-dev");

            var expected = new ConstraintMulti(first, second);
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints("^2.5 || ^3.0").ToString());
        }

        [TestMethod]
        public void TestParseDoNotCollapseContiguousRangeIfOtherConstraintsAlsoApply()
        {
            var first = new ConstraintMulti(
                    new BConstraint(">=", "0.1.0.0-dev"),
                    new BConstraint("<", "1.0.0.0-dev"));

            var second = new ConstraintMulti(
                    new BConstraint(">=", "1.0.0.0-dev"),
                    new BConstraint("<", "2.0.0.0-dev"),
                    new BConstraint("!=", "1.0.1.0"));
            var expected = new ConstraintMulti(first, second)
            {
                IsConjunctive = false,
            };

            var version = new BConstraint("=", "1.0.1.0");
            Assert.IsFalse(expected.Matches(version), "Generated expectation should not allow version \"1.0.1.0\"");

            var parsed = parser.ParseConstraints("~0.1 || ~1.0 !=1.0.1");
            Assert.IsFalse(parsed.Matches(version), "\"~0.1 || ~1.0 !=1.0.1\" should not allow version \"1.0.1.0\"");

            Assert.AreEqual(expected.ToString(), parsed.ToString());
        }

#pragma warning disable SA1204
        public static IEnumerable<object[]> MultiConstraints()
#pragma warning restore SA1204
        {
            return new[]
            {
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), ">2.0,<=3.0" },
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), ">2.0 <=3.0" },
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), ">2.0  <=3.0" },
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), ">2.0, <=3.0" },
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), ">2.0 ,<=3.0" },
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), ">2.0 , <=3.0" },
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), ">2.0   , <=3.0" },
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), "> 2.0   <=  3.0" },
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), "> 2.0  ,  <=  3.0" },
                new object[] { null, new BConstraint(">", "2.0.0.0"), new BConstraint("<=", "3.0.0.0"), "  > 2.0  ,  <=  3.0 " },
            };
        }

        [TestMethod]
        [DynamicData("MultiConstraints", DynamicDataSourceType.Method)]
        public void TestParseConstraintsMulti(string delta, IConstraint first, IConstraint second, string version)
        {
            var expected = new ConstraintMulti(first, second);
            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(version).ToString(), $"{version} mult parse faild. {delta}");
        }

        [TestMethod]
        public void TestParseConstraintsMultiWithStabilitySuffix()
        {
            var first = new BConstraint(">=", "1.1.0.0-alpha4");
            var second = new BConstraint("<", "1.2.9999999.9999999-dev");
            var expected = new ConstraintMulti(first, second);

            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(">=1.1.0-alpha4,<1.2.x-dev").ToString());

            first = new BConstraint(">=", "1.1.0.0-alpha4");
            second = new BConstraint("<", "1.2.0.0-beta2");
            expected = new ConstraintMulti(first, second);

            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(">=1.1.0-alpha4,<1.2-beta2").ToString());
        }

        [TestMethod]
        [DataRow("Parsing by | split", ">2.0,<2.0.5 | >2.0.6")]
        [DataRow("Parsing by || split", ">2.0,<2.0.5 || >2.0.6")]
        [DataRow("Parsing with spaces.", "> 2.0 , <2.0.5 | >  2.0.6")]
        public void TestParseConstraintsMultiDisjunctiveHasPrioOverConjuctive(string delta, string version)
        {
            var first = new BConstraint(">", "2.0.0.0");
            var second = new BConstraint("<", "2.0.5.0-dev");
            var third = new BConstraint(">", "2.0.6.0");

            var multi = new ConstraintMulti(first, second);
            var expected = new ConstraintMulti(multi, third)
            {
                IsConjunctive = false,
            };

            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(version).ToString(), $"{version} mult conjunctive parse faild. {delta}");
        }

        [TestMethod]
        public void TestParseConstraintsMultiWithStabilities()
        {
            var first = new BConstraint(">", "2.0.0.0");
            var second = new BConstraint("<=", "3.0.0.0-dev");
            var expected = new ConstraintMulti(first, second);

            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(">2.0@stable,<=3.0@dev").ToString());

            first = new BConstraint(">", "2.0.0.0-alpha");
            second = new BConstraint("<=", "3.0.0.0-beta");
            expected = new ConstraintMulti(first, second);

            Assert.AreEqual(expected.ToString(), parser.ParseConstraints(">2.0@alpha,<=3.0@beta").ToString());
        }

        private IConstraint CreateWildcardConstraintExpected(IConstraint min, IConstraint max)
        {
            if (min != null)
            {
                return new ConstraintMulti(min, max);
            }
            else
            {
                return max;
            }
        }
    }
}
