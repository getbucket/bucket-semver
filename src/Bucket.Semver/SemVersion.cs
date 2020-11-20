/*
 * This file is part of the Bucket package. Borrow and improve from https://github.com/maxhauser/semver.
 *
 * (c) Yu Meng Han <menghanyu1994@gmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://github.com/getbucket/semver/wiki
 */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Bucket.Semver
{
    /// <summary>
    /// A semantic version implementation.
    /// Conforms to v2.0.0 of http://semver.org/ .
    /// </summary>
    public sealed class SemVersion : IComparable<SemVersion>, IComparable
    {
        private static Regex parseEx =
            new Regex(
                @"^(?<major>\d+)" +
                @"(\.(?<minor>\d+))?" +
                @"(\.(?<patch>\d+))?" +
                @"(\.(?<revision>\d+))?" +
                @"(\-(?<pre>[0-9A-Za-z\-\.]+))?" +
                @"(\+(?<build>[0-9A-Za-z\-\.]+))?$",
                RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

        private static IDictionary<string, int> stabilities = new Dictionary<string, int>
        {
            { "dev", 20 },
            { "alpha", 15 },
            { "a", 15 },
            { "beta", 10 },
            { "b", 10 },
            { "rc", 5 },
            { "#", 4 },
            { "pl", 3 },
            { "p", 3 },
            { "stable", 0 },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SemVersion" /> class.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="patch">The patch version.</param>
        /// <param name="prerelease">The prerelease version (eg. "alpha").</param>
        /// <param name="build">The build eg ("nightly.232").</param>
        public SemVersion(int major, int minor = 0, int patch = 0, string prerelease = "", string build = "")
            : this(major, minor, patch, 0, prerelease, build)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemVersion"/> class.
        /// </summary>
        /// <param name="version">The <see cref="Version"/> that is used to initialize
        /// the Major, Minor, Patch and Build properties.</param>
        public SemVersion(Version version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Major = version.Major;
            Minor = Math.Max(0, version.Minor);
            Patch = Math.Max(0, version.Build);
            Revision = Math.Max(0, version.Revision);

            Prerelease = string.Empty;
            Build = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemVersion" /> class.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="patch">The patch version.</param>
        /// <param name="revision">The revision version.</param>
        /// <param name="prerelease">The prerelease version (eg. "alpha").</param>
        /// <param name="build">The build eg ("nightly.232").</param>
        private SemVersion(int major, int minor = 0, int patch = 0, int revision = 0, string prerelease = "", string build = "")
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;

            Prerelease = prerelease ?? string.Empty;
            Build = build ?? string.Empty;
        }

        /// <summary>
        /// Gets the major version.
        /// </summary>
        /// <value>
        /// The major version.
        /// </value>
        public int Major { get; private set; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        /// <value>
        /// The minor version.
        /// </value>
        public int Minor { get; private set; }

        /// <summary>
        /// Gets the patch version.
        /// </summary>
        /// <value>
        /// The patch version.
        /// </value>
        public int Patch { get; private set; }

        /// <summary>
        /// Gets the revision version.
        /// </summary>
        public int Revision { get; private set; }

        /// <summary>
        /// Gets the pre-release version.
        /// </summary>
        /// <value>
        /// The pre-release version.
        /// </value>
        public string Prerelease { get; private set; }

        /// <summary>
        /// Gets the build version.
        /// </summary>
        /// <value>
        /// The build version.
        /// </value>
        public string Build { get; private set; }

        /// <summary>
        /// Implicit conversion from string to SemVersion.
        /// </summary>
        /// <param name="version">The semantic version.</param>
        /// <returns>The SemVersion object.</returns>
#pragma warning disable CA2225
        public static implicit operator SemVersion(string version)
#pragma warning restore CA2225
        {
            return Parse(version);
        }

        /// <summary>
        /// The override of the equals operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator ==(SemVersion left, SemVersion right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// The override of the un-equal operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is not equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator !=(SemVersion left, SemVersion right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// The override of the greater operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is greater than right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator >(SemVersion left, SemVersion right)
        {
            return Compare(left, right) > 0;
        }

        /// <summary>
        /// The override of the greater than or equal operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is greater than or equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator >=(SemVersion left, SemVersion right)
        {
            return left == right || left > right;
        }

        /// <summary>
        /// The override of the less operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is less than right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator <(SemVersion left, SemVersion right)
        {
            return Compare(left, right) < 0;
        }

        /// <summary>
        /// The override of the less than or equal operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is less than or equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator <=(SemVersion left, SemVersion right)
        {
            return left == right || left < right;
        }

        /// <summary>
        /// Parses the specified string to a semantic version.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <param name="strict">If set to <c>true</c> minor and patch version are required, else they default to 0.</param>
        /// <returns>The SemVersion object.</returns>
        /// <exception cref="InvalidOperationException">When a invalid version string is passed.</exception>
        public static SemVersion Parse(string version, bool strict = false)
        {
            var match = parseEx.Match(version);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid version.", nameof(version));
            }

            var major = int.Parse(match.Groups["major"].Value);

            var minorMatch = match.Groups["minor"];
            int minor = 0;
            if (minorMatch.Success)
            {
                minor = int.Parse(minorMatch.Value);
            }
            else if (strict)
            {
                throw new InvalidOperationException("Invalid version (no minor version given in strict mode)");
            }

            var patchMatch = match.Groups["patch"];
            int patch = 0;
            if (patchMatch.Success)
            {
                patch = int.Parse(patchMatch.Value);
            }
            else if (strict)
            {
                throw new InvalidOperationException("Invalid version (no patch version given in strict mode)");
            }

            var revision = match.Groups["revision"].Success ? int.Parse(match.Groups["revision"].Value) : 0;
            var prerelease = match.Groups["pre"].Value;
            var build = match.Groups["build"].Value;

            return new SemVersion(major, minor, patch, revision, prerelease, build);
        }

        /// <summary>
        /// Parses the specified string to a semantic version.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <param name="semver">When the method returns, contains a SemVersion instance equivalent
        /// to the version string passed in, if the version string was valid, or <c>null</c> if the
        /// version string was not valid.</param>
        /// <param name="strict">If set to <c>true</c> minor and patch version are required, else they default to 0.</param>
        /// <returns><c>False</c> when a invalid version string is passed, otherwise <c>true</c>.</returns>
        public static bool TryParse(string version, out SemVersion semver, bool strict = false)
        {
            try
            {
                semver = Parse(version, strict);
                return true;
            }
#pragma warning disable CA1031
            catch (Exception)
            {
                semver = null;
                return false;
            }
#pragma warning restore CA1031
        }

        /// <summary>
        /// Tests the specified versions for equality.
        /// </summary>
        /// <param name="versionA">The first version.</param>
        /// <param name="versionB">The second version.</param>
        /// <returns>If versionA is equal to versionB <c>true</c>, else <c>false</c>.</returns>
        public static bool Equals(SemVersion versionA, SemVersion versionB)
        {
            if (ReferenceEquals(versionA, null))
            {
                return ReferenceEquals(versionB, null);
            }

            return versionA.Equals(versionB);
        }

        /// <summary>
        /// Compares the specified versions.
        /// </summary>
        /// <param name="versionA">The version to compare to.</param>
        /// <param name="versionB">The version to compare against.</param>
        /// <returns>If versionA &lt; versionB <c>&lt; 0</c>, if versionA &gt; versionB <c>&gt; 0</c>,
        /// if versionA is equal to versionB <c>0</c>.</returns>
        public static int Compare(SemVersion versionA, SemVersion versionB)
        {
            if (ReferenceEquals(versionA, null))
            {
                return ReferenceEquals(versionB, null) ? 0 : -1;
            }

            return versionA.CompareTo(versionB);
        }

        /// <summary>
        /// Compares the specified versions.
        /// </summary>
        /// <param name="versionA">The version to compare to.</param>
        /// <param name="versionB">The version to compare against.</param>
        /// <param name="operator">The operation of compare.</param>
        /// <returns>True if the compliance with operation.</returns>
        public static bool Compare(SemVersion versionA, SemVersion versionB, string @operator)
        {
            switch (@operator)
            {
                case "==":
                    return versionA == versionB;
                case ">":
                    return versionA > versionB;
                case ">=":
                    return versionA >= versionB;
                case "<":
                    return versionA < versionB;
                case "<=":
                    return versionA <= versionB;
                case "!=":
                    return versionA != versionB;
                default:
                    throw new ArgumentException($"Operator is valid value \"{@operator}\" Valid values are: ==, >, >=, <, <=, !=.");
            }
        }

        /// <summary>
        /// Make a copy of the current instance with optional altered fields.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="patch">The patch version.</param>
        /// <param name="prerelease">The prerelease text.</param>
        /// <param name="build">The build text.</param>
        /// <returns>The new version object.</returns>
        public SemVersion Change(int? major = null, int? minor = null, int? patch = null,
            string prerelease = null, string build = null)
        {
            return new SemVersion(
                major ?? Major,
                minor ?? Minor,
                patch ?? Patch,
                prerelease ?? Prerelease,
                build ?? Build);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var version = string.Empty + Major + "." + Minor + "." + Patch;

            if (Revision > 0)
            {
                version += "." + Revision;
            }

            if (!string.IsNullOrEmpty(Prerelease))
            {
                version += "-" + Prerelease;
            }

            if (!string.IsNullOrEmpty(Build))
            {
                version += "+" + Build;
            }

            return version;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
        /// other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero
        ///  This instance precedes <paramref name="obj" /> in the sort order.
        ///  Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. i
        ///  Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            return CompareTo((SemVersion)obj);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
        /// other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero
        ///  This instance precedes <paramref name="other" /> in the sort order.
        ///  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. i
        ///  Greater than zero This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo(SemVersion other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            var r = CompareByPrecedence(other);
            if (r != 0)
            {
                return r;
            }

            r = CompareComponent(Build, other.Build);
            return r;
        }

        /// <summary>
        /// Compares to semantic versions by precedence. This does the same as a Equals, but ignores the build information.
        /// </summary>
        /// <param name="other">The semantic version.</param>
        /// <returns><c>true</c> if the version precedence matches.</returns>
        public bool PrecedenceMatches(SemVersion other)
        {
            return CompareByPrecedence(other) == 0;
        }

        /// <summary>
        /// Compares to semantic versions by precedence. This does the same as a Equals, but ignores the build information.
        /// </summary>
        /// <param name="other">The semantic version.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero
        ///  This instance precedes <paramref name="other" /> in the version precedence.
        ///  Zero This instance has the same precedence as <paramref name="other" />. i
        ///  Greater than zero This instance has creater precedence as <paramref name="other" />.
        /// </returns>
        public int CompareByPrecedence(SemVersion other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            var r = Major.CompareTo(other.Major);
            if (r != 0)
            {
                return r;
            }

            r = Minor.CompareTo(other.Minor);
            if (r != 0)
            {
                return r;
            }

            r = Patch.CompareTo(other.Patch);
            if (r != 0)
            {
                return r;
            }

            r = Revision.CompareTo(other.Revision);
            if (r != 0)
            {
                return r;
            }

            r = CompareComponent(Prerelease, other.Prerelease, true);
            return r;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = (SemVersion)obj;

            return Major == other.Major &&
                Minor == other.Minor &&
                Patch == other.Patch &&
                Revision == other.Revision &&
                string.Equals(Prerelease, other.Prerelease, StringComparison.Ordinal) &&
                string.Equals(Build, other.Build, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = Major.GetHashCode();
                result = (result * 31) + Minor.GetHashCode();
                result = (result * 31) + Patch.GetHashCode();
                result = (result * 31) + Revision.GetHashCode();
                result = (result * 31) + Prerelease.GetHashCode();
                result = (result * 31) + Build.GetHashCode();
                return result;
            }
        }

        private static int CompareComponent(string a, string b, bool lower = false)
        {
            var aEmpty = string.IsNullOrEmpty(a);
            var bEmpty = string.IsNullOrEmpty(b);
            if (aEmpty && bEmpty)
            {
                return 0;
            }

            if (aEmpty)
            {
                return lower ? 1 : -1;
            }

            if (bEmpty)
            {
                return lower ? -1 : 1;
            }

            string Normalize(string input)
            {
                input = Regex.Replace(input, @"(?<num>[0-9]+)", ".${num}.");
                return Regex.Replace(input, @"\.{2,}", ".").Trim('.');
            }

            a = Normalize(a);
            b = Normalize(b);

            var aComps = a.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var bComps = b.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            var minLen = Math.Min(aComps.Length, bComps.Length);
            for (int i = 0; i < minLen; i++)
            {
                var ac = aComps[i];
                var bc = bComps[i];

                if (CompareStability(ac, bc, out int result))
                {
                    // In the case of equal stability, subsequent
                    // characters will continue to be compared.
                    if (result == 0 && (i + 1) < minLen)
                    {
                        continue;
                    }

                    return result;
                }

                var aIsNum = int.TryParse(ac, out int aNum);
                var bIsnum = int.TryParse(bc, out int bNum);

                if (aIsNum && bIsnum)
                {
                    result = aNum.CompareTo(bNum);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                else
                {
                    if (aIsNum)
                    {
                        return -1;
                    }

                    if (bIsnum)
                    {
                        return 1;
                    }

                    result = string.CompareOrdinal(ac, bc);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }

            return aComps.Length.CompareTo(bComps.Length);
        }

        /// <summary>
        /// Compare special version characters.
        /// </summary>
        /// <remarks>
        /// If a part contains special version strings these are handled in the following order:
        /// any string not found in this list , dev , alpha(a) = beta(b), rc , # , pl(p), stable.
        /// </remarks>
        private static bool CompareStability(string a, string b, out int result)
        {
            var aIsStability = stabilities.TryGetValue(a.ToLower(), out int aWeight);
            var bIsStability = stabilities.TryGetValue(b.ToLower(), out int bWeight);

            result = 0;
            if (!aIsStability && !bIsStability)
            {
                return false;
            }

            if (aIsStability && !bIsStability)
            {
                result = 1;
                return true;
            }

            if (!aIsStability && bIsStability)
            {
                result = -1;
                return true;
            }

            if (aWeight == bWeight)
            {
                return true;
            }

            result = aWeight > bWeight ? -1 : 1;
            return true;
        }
    }
}
