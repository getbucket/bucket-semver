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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bucket.Semver
{
    /// <summary>
    /// Bucket version parser.
    /// </summary>
    public class VersionParser : IVersionParser
    {
        /// <summary>
        /// Represents a maximum version that can be parsed, usually a special tag.
        /// </summary>
        public const string VersionMax = "9999999";

        /// <summary>
        /// Represents the major version of a branch, usually a special tag.
        /// </summary>
        public const string VersionMaster = "9999999-dev";

        /// <summary>
        /// Pre-release extraction rules are applicable to semver.
        /// - Only stabilities as recognized by Bucket are allowed to precede a numerical identifier.
        /// - Numerical-only pre-release identifiers are not supported.
        /// - This is a superset of the Semver semantic version(Applied to pre-release fields).
        /// </summary>
        private const string RegexPreReleaseString = @"[._-]?(?<pre>(?<stability>stable|beta|b|RC|alpha|a|patch|pl|p)(?<identifier>(?:[.-]?\d+)+)?)?([.-]?(?<is_branch>dev))?";

        /// <summary>
        /// Build extraction rules are applicable to semver.
        /// </summary>
        private const string RegexBuildString = @"(\+(?<build>[0-9A-Za-z\-\.]+))?";

#pragma warning disable CA1802
        private static readonly RegexOptions RegexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
#pragma warning restore CA1802

        /// <summary>
        /// Version extraction rules applicable to semver.
        /// Represents a classic versioning.
        /// - This is a subset of the Semver semantic version(Added revision field).
        /// </summary>
        private static readonly Regex RegexBucketVersion =
            new Regex(
                @"^v?(?<major>\d{1,5})" +
                @"(?<minor>\.\d+)?" +
                @"(?<patch>\.\d+)?" +
                @"(?<revision>\.\d+)?" +
                RegexPreReleaseString +
                RegexBuildString + "$",
                RegexOptions);

        /// <summary>
        /// Represents a date(time) based versioning.
        /// </summary>
        private static readonly Regex RegexDateTimeVersion =
            new Regex(
                @"^v?(?<calver>(?<year>\d{4})" +
                @"(?<time>(?:[.:-]?\d{2}){1,6})" +
                @"(?<micro>[.:-]?\d{1,3})?)" +
                RegexPreReleaseString +
                RegexBuildString + "$",
                RegexOptions);

        /// <summary>
        /// Indicates the main branch.
        /// </summary>
        private static readonly string[] Masters = new[] { "master", "trunk", "default" };
        private static readonly string[] StabilitiesString = new[] { "stable", "RC", "beta", "alpha", "dev" };

        private static readonly Dictionary<string, Stabilities> StringToStabilities = new Dictionary<string, Stabilities>
        {
            { "stable", Stabilities.Stable },
            { "patch", Stabilities.Stable },
            { "beta", Stabilities.Beta },
            { "rc", Stabilities.RC },
            { "alpha",  Stabilities.Alpha },
            { "dev", Stabilities.Dev },
        };

        private static readonly Dictionary<string, string> NormalizeStabilities = new Dictionary<string, string>
        {
            { "stable", "stable" },
            { "patch", "patch" },
            { "pl", "patch" },
            { "p", "patch" },
            { "beta", "beta" },
            { "b", "beta" },
            { "rc", "RC" },
            { "alpha", "alpha" },
            { "a", "alpha" },
            { "dev", "dev" },
        };

        /// <summary>
        /// Returns the stability of a version.
        /// </summary>
        /// <param name="version">The semversion version.</param>
        /// <returns>The version stability.</returns>
        public static Stabilities ParseStability(string version)
        {
            version = Regex.Replace(version, "#.+$", string.Empty, RegexOptions);
            version = version.ToLower();
            if (string.IsNullOrEmpty(version) ||
                (version.Length >= 4 && (version.Substring(0, 4) == "dev-"
                    || version.Substring(version.Length - 4, 4) == "-dev")))
            {
                return Stabilities.Dev;
            }

            var matched = Regex.Match(version, RegexPreReleaseString + RegexBuildString + "$", RegexOptions);
            if (matched.Groups["is_branch"].Success)
            {
                return Stabilities.Dev;
            }

            var stability = string.Empty;
            if (matched.Groups["stability"].Success)
            {
                stability = matched.Groups["stability"].Value;
            }

            return ParseStabilityString(stability);
        }

        /// <summary>
        /// Normalizes a version string to be able to perform comparisons on it.
        /// </summary>
        /// <param name="version">The version needs to normalize.</param>
        /// <param name="fullVersion">An optional complete version string to give more context.(use for debug log).</param>
        /// <returns>Return's version can be able to perform comparisons on it.</returns>
        public string Normalize(string version, string fullVersion = null)
        {
            version = version.Trim();
            fullVersion = fullVersion ?? version;

            // strip off aliasing. ex: dev-master as 1.0.0 => dev-master
            var match = Regex.Match(version, "^([^,\\s]+)? as ([^,\\s]+)$", RegexOptions);
            if (match.Success && match.Groups[1].Success)
            {
                version = match.Groups[1].Value;
            }

            // match master-like branches.
            if (Regex.IsMatch(version, $"^(?:dev-)?(?:{string.Join("|", Masters)})$", RegexOptions))
            {
                return VersionMaster;
            }

            // if the requirement is branch-like, use the full name
            if (version.Length >= 4 && version.Substring(0, 4).ToLower() == "dev-")
            {
                return version.Length == 4 ? "dev-" : $"dev-{version.Substring(4)}";
            }

            // strip off build metadata, ex: 1.0.0-beta.5+foo => 1.0.0-beta.5
            match = Regex.Match(version, "^([^,\\s+]+)\\+[^\\s]+$", RegexOptions);
            if (match.Success && match.Groups[1].Success)
            {
                version = match.Groups[1].Value;
            }

            // match classical versioning
            var matched = false;
            match = RegexBucketVersion.Match(version);
            if (match.Success)
            {
                version = match.Groups["major"] +
                            (match.Groups["minor"].Success ? match.Groups["minor"].Value : ".0") +
                            (match.Groups["patch"].Success ? match.Groups["patch"].Value : ".0") +
                            (match.Groups["revision"].Success ? match.Groups["revision"].Value : ".0");

                matched = true;
            }
            else
            {
                match = RegexDateTimeVersion.Match(version);
                if (match.Success && match.Groups["calver"].Success)
                {
                    version = Regex.Replace(match.Groups["calver"].Value, "\\D", ".", RegexOptions);
                    matched = true;
                }
            }

            // add version modifiers if a version was matched.
            if (matched)
            {
                if (match.Groups["stability"].Success)
                {
                    var stability = NormalizeStabilityString(match.Groups["stability"].Value);
                    if (stability == "stable")
                    {
                        return version;
                    }

                    var identifier = match.Groups["identifier"].Success ?
                        match.Groups["identifier"].Value.TrimStart('.', '-') :
                        string.Empty;
                    version += "-" + stability + identifier;
                }

                if (match.Groups["is_branch"].Success)
                {
                    version += "-dev";
                }

                return version;
            }

            match = Regex.Match(version, @"(.*?)[.-]?dev$", RegexOptions);
            if (match.Success)
            {
                try
                {
                    return NormalizeBranch(match.Groups[1].Value);
                }
#pragma warning disable CA1031
                catch (Exception)
                {
                    // ignore
                }
#pragma warning restore CA1031
            }

            var extraMessage = string.Empty;

            if (Regex.IsMatch(fullVersion, " as " + Regex.Escape(version) + "$", RegexOptions))
            {
                extraMessage = $" in \"{fullVersion}\", the alias must be an exact version";
            }
            else if (Regex.IsMatch(fullVersion, "^" + Regex.Escape(version) + " as ", RegexOptions))
            {
                extraMessage = $" in \"{fullVersion}\", the alias source must be an exact version, if it is a branch name you should prefix it with dev-";
            }

            throw new ParseException($"Invalid version string \"{version}\"" + extraMessage);
        }

        /// <summary>
        /// Normalizes a branch name to be able to perform comparisons on it.
        /// </summary>
        /// <param name="branch">The branch name.</param>
        /// <returns>Returns normalized branch name.</returns>
        public string NormalizeBranch(string branch)
        {
            branch = branch.Trim();

            if (Array.Exists(Masters, (item) => item == branch))
            {
                return Normalize(branch);
            }

            var pattern = @"^v?(\d+|[xX*])" +
                          @"(\.(?:\d+|[xX*]))?" +
                          @"(\.(?:\d+|[xX*]))?" +
                          @"(\.(?:\d+|[xX*]))?$";

            var match = Regex.Match(branch, pattern, RegexOptions);
            if (match.Success)
            {
                var version = new StringBuilder();
                for (var i = 1; i < 5; ++i)
                {
                    version.Append(match.Groups[i].Success ? match.Groups[i].Value.Replace("x", "*").Replace("X", "*") : ".*");
                }

                return version.ToString().Replace("*", VersionMax) + "-dev";
            }

            if (branch.Length >= 4)
            {
                return (branch.Substring(0, 4) != "dev-") ?
                    $"dev-{branch.TrimStart('-')}" :
                    branch;
            }

            return $"dev-{branch.TrimStart('-')}";
        }

        /// <summary>
        /// Parses a constraint string into MultiConstraint and/or Constraint objects.
        /// </summary>
        /// <param name="constraints">The version-constrained string.</param>
        /// <returns>Returns the <see cref="IConstraint"/> objects.</returns>
        public IConstraint ParseConstraints(string constraints)
        {
            constraints = constraints ?? string.Empty;
            var prettyConstraint = constraints;

            // match only stabilities flag , ex: @dev
            var match = Regex.Match(constraints, $"^([^,\\s]*?)@({string.Join("|", StabilitiesString)})$", RegexOptions);
            if (match.Success)
            {
                constraints = (match.Groups[1].Success && !string.IsNullOrEmpty(match.Groups[1].Value))
                    ? match.Groups[1].Value : "*";
            }

            // match dev version , ex: 1.0.x-dev#abcd123
            match = Regex.Match(constraints, $"^(dev-[^,\\s@]+?|[^,\\s@]+?\\.[xX*]-dev)#.+$", RegexOptions);
            if (match.Success)
            {
                constraints = match.Groups[1].Value;
            }

            // match or constraints , ex: 1.0.x || 2.0.x
            var orConstraints = Regex.Split(constraints.Trim(), @"\s*\|\|?\s*");
            var constraintObjects = new LinkedList<IConstraint>();
            var orGroups = new List<IConstraint>();
            IConstraint constraintObject;
            foreach (var constraint in orConstraints)
            {
                var andConstraints = Regex.Split(constraint, "(?<!^|as|[=>< ,]) *(?<!-)[, ](?!-) *(?!,|as|$)");

                if (andConstraints.Length > 1)
                {
                    foreach (var andConstraint in andConstraints)
                    {
                        Array.ForEach(ParseConstraint(andConstraint), (item) => constraintObjects.AddLast(item));
                    }
                }
                else
                {
                    Array.ForEach(ParseConstraint(andConstraints[0]), (item) => constraintObjects.AddLast(item));
                }

                if (constraintObjects.Count == 1)
                {
                    constraintObject = constraintObjects.First.Value;
                }
                else
                {
                    constraintObject = new ConstraintMulti(constraintObjects.ToArray());
                }

                orGroups.Add(constraintObject);
                constraintObjects.Clear();
            }

            constraintObject = null;
            if (orGroups.Count == 1)
            {
                constraintObject = orGroups[0];
            }
            else if (orGroups.Count == 2
                && orGroups[0] is ConstraintMulti mult1
                && orGroups[1] is ConstraintMulti mult2
                && mult1.GetConstraints().Length == 2
                && mult2.GetConstraints().Length == 2)
            {
                var left = mult1.ToString();
                var right = mult2.ToString();
                var posLeft = 0;
                var posRight = 0;

                // If the interval before and after the constraint is the same then we merge the constraint.
                // For example:
                // [>= 2.5.0.0-dev < 3.0.0.0-dev]
                //                   |---------|
                //
                // [>= 3.0.0.0-dev < 4.0.0.0-dev]
                //     |---------|
                //
                // If the versions of the underlined positions are equal then we think that we can merge:
                // [>= 2.5.0.0-dev < 4.0.0.0-dev] have the same effect.
                if (left.Length > 4
                    && right.Length > 4
                    && left.Substring(0, 3) == "[>=" && right.Substring(0, 3) == "[>="
                    && (posLeft = left.IndexOf("<", 4, StringComparison.CurrentCulture)) != -1
                    && (posRight = right.IndexOf("<", 4, StringComparison.CurrentCulture)) != -1)
                {
                    // [>= 2.5.0.0-dev < 3.0.0.0-dev]
                    //                 --           -
                    // -2 means: "< " occupy 2 characters.
                    // -1 means: "]" characters.
                    var leftLength = left.Length - posLeft - 2 - 1;

                    // [>= 3.0.0.0-dev < 4.0.0.0-dev]
                    // |--|           -
                    // -4 means: "[>= 3" occupy 4 characters.
                    // -1 means: " " occupy 1 characters.
                    var rightLength = posRight - 4 - 1;
                    if (left.Substring(posLeft + 2, leftLength) == right.Substring(4, rightLength))
                    {
                        leftLength = posLeft - 4 - 1;
                        rightLength = right.Length - posRight - 2 - 1;
                        constraintObject = new ConstraintMulti(
                            new Constraint.Constraint(">=", left.Substring(4, leftLength)),
                            new Constraint.Constraint("<", right.Substring(posRight + 2, rightLength)));
                    }
                }
            }

            if (constraintObject == null)
            {
                constraintObject = new ConstraintMulti(orGroups.ToArray())
                {
                    IsConjunctive = false,
                };
            }

            if (constraintObject is BaseConstraint baseConstraint)
            {
                baseConstraint.SetPrettyString(prettyConstraint);
            }

            return constraintObject;
        }

        /// <summary>
        /// Normalized stability identification string.
        /// </summary>
        /// <param name="stability">The stability string.</param>
        /// <returns>Normalized stability string.</returns>
        private static string NormalizeStabilityString(string stability)
        {
            if (!string.IsNullOrEmpty(stability)
                && NormalizeStabilities.TryGetValue(stability.ToLower(), out string result))
            {
                return result;
            }

            return stability;
        }

        /// <summary>
        /// Expand shorthand stability string to <see cref="Stabilities"/>.
        /// </summary>
        /// <param name="stability">The shorthand stability string.</param>
        /// <returns>Returns the <see cref="Stabilities"/> value.</returns>
        private static Stabilities ParseStabilityString(string stability)
        {
            if (string.IsNullOrEmpty(stability))
            {
                return Stabilities.Stable;
            }

            stability = stability.ToLower();

            if (!NormalizeStabilities.TryGetValue(stability, out stability))
            {
                return Stabilities.Stable;
            }

            // maybe RC , so we need Tolower().
            if (StringToStabilities.TryGetValue(stability.ToLower(), out Stabilities result))
            {
                return result;
            }

            return Stabilities.Stable;
        }

        /// <summary>
        /// Increment, decrement, or simply pad a version number.
        /// </summary>
        /// <remarks>This method is internal support method for <see cref="ParseConstraint"/>.</remarks>
        /// <param name="matched">Array with version parts in array indexes 0,1,2,3.</param>
        /// <param name="position">0(major),1(minor),2(patch),3(revision) - which segment of the version to increment/decrement.</param>
        /// <param name="increment">Increased value.</param>
        /// <param name="pad">The string to pad version parts after segment.</param>
        /// <returns>Returns the new version.</returns>
        private static string ManipulateVersionString(GroupCollection matched, int position, int increment = 0, int pad = 0)
        {
            position = Math.Max(0, position);
            var parts = new[] { "major", "minor", "patch", "revision" };

            var results = new int[4];
            for (var i = 3; i >= 0; --i)
            {
                var segment = parts[i];

                if (i > position)
                {
                    results[i] = pad;
                }
                else if (i == position && increment > 0)
                {
                    var origin = matched[segment].Success ? int.Parse(matched[segment].Value) : 0;
                    results[i] = origin + increment;

                    if (results[i] < 0)
                    {
                        results[i] = pad;
                        --position;

                        if (i == 0)
                        {
                            return null;
                        }
                    }
                }
                else if (matched[segment].Success)
                {
                    results[i] = int.Parse(matched[segment].Value);
                }
            }

            return $"{results[0]}.{results[1]}.{results[2]}.{results[3]}";
        }

        /// <summary>
        /// Parses one constraint string.
        /// </summary>
        /// <param name="constraint">The version-constrained string.</param>
        /// <returns>Returns the <see cref="IConstraint"/> objects.</returns>
        private IConstraint[] ParseConstraint(string constraint)
        {
            // Using the stability variant of @ only works for Basic Comparators
            // So we need record and removed variant value before other inspections begin.
            string stabilityModifier = null;
            var match = Regex.Match(constraint, $"^([^,\\s]+?)@({string.Join("|", StabilitiesString)})$", RegexOptions);
            if (match.Success)
            {
                constraint = match.Groups[1].Value;
                if (match.Groups[2].Success && match.Groups[2].Value != "stable")
                {
                    stabilityModifier = match.Groups[2].Value;
                }
            }

            // If you are using * wild, then we return directly to improve efficiency.
            if (Regex.IsMatch(constraint, "^v?[xX*](\\.[xX*])*$", RegexOptions))
            {
                return new[] { new ConstraintNone() };
            }

            const string regexVersionNumberString = @"(?<major>\d+)" +
                                            @"(?:\.(?<minor>\d+))?" +
                                            @"(?:\.(?<patch>\d+))?" +
                                            @"(?:\.(?<revision>\d+))?";

            const string regexVersionString = @"v?" +
                                            regexVersionNumberString +
                                            RegexPreReleaseString +
                                            RegexBuildString;

            IConstraint ComputeLowerVersion(Match matched, string version)
            {
                var stabilitySuffix = string.Empty;
                if (!matched.Groups["stability"].Success && !matched.Groups["is_branch"].Success)
                {
                    stabilitySuffix = "-dev";
                }

                var lowVersion = Normalize(version + stabilitySuffix);
                return new Constraint.Constraint(">=", lowVersion);
            }

            var empty = string.IsNullOrEmpty(constraint);

            // Tilde Range
            //
            // Like wildcard constraints, unsuffixed tilde constraints say that they must be greater than the previous
            // version, to ensure that unstable instances of the current version are allowed. However, if a stability
            // suffix is added to the constraint, then a >= match on the current version is used instead.
            //
            // ex: ~1.0, ~1.2.2-stable, ~201905.0-beta
            if (!empty && constraint[0] == '~')
            {
                match = Regex.Match(constraint, $"^~{regexVersionString}$", RegexOptions);
                if (match.Success)
                {
                    // Work out which position in the version we are operating at
                    var position = 0;
                    if (match.Groups["revision"].Success)
                    {
                        position = 3;
                    }
                    else if (match.Groups["patch"].Success)
                    {
                        position = 2;
                    }
                    else if (match.Groups["minor"].Success)
                    {
                        position = 1;
                    }

                    var lowerConstraint = ComputeLowerVersion(match, constraint.Substring(1));

                    // For high constraint, we increment the position of one more significance
                    var highVersion = ManipulateVersionString(match.Groups, position - 1, 1) + "-dev";
                    var highConstraint = new Constraint.Constraint("<", highVersion);

                    return new[] { lowerConstraint, highConstraint };
                }
            }

            // Caret Range
            //
            // Allows changes that do not modify the left-most non-zero digit in the [major, minor, patch] tuple.
            //
            // ex: ^1.0, ^1.2.3-beta.
            if (!empty && constraint[0] == '^')
            {
                match = Regex.Match(constraint, $"^\\^{regexVersionString}$", RegexOptions);
                if (match.Success)
                {
                    var position = 2;
                    if (!match.Groups["minor"].Success || int.Parse(match.Groups["major"].Value) != 0)
                    {
                        position = 0;
                    }
                    else if (!match.Groups["patch"].Success || int.Parse(match.Groups["minor"].Value) != 0)
                    {
                        position = 1;
                    }

                    var lowerConstraint = ComputeLowerVersion(match, constraint.Substring(1));

                    var highVersion = ManipulateVersionString(match.Groups, position, 1) + "-dev";
                    var highConstraint = new Constraint.Constraint("<", highVersion);

                    return new[] { lowerConstraint, highConstraint };
                }
            }

            // X Range
            //
            // Any of X, x, or * may be used to "stand in" for one of the numeric values in
            // the [major, minor, patch] tuple. A partial version range is treated as an X-Range,
            // so the special character is in fact optional.
            //
            // ex: v1.0.0 - v2.1.3
            bool IsWildcard(char c)
            {
                return c == '*' || c == 'x' || c == 'X';
            }

            if (!empty && IsWildcard(constraint[constraint.Length - 1]))
            {
                const string regexXRangVersionString = @"v?(?<major>\d+)" +
                                                       @"(?:\.(?<minor>\d+))?" +
                                                       @"(?:\.(?<patch>\d+))?" +
                                                       @"(?:\.[xX*])+$";
                match = Regex.Match(constraint, regexXRangVersionString, RegexOptions);
                if (match.Success)
                {
                    var position = 0;
                    if (match.Groups["patch"].Success)
                    {
                        position = 2;
                    }
                    else if (match.Groups["minor"].Success)
                    {
                        position = 1;
                    }

                    var lowVersion = ManipulateVersionString(match.Groups, position) + "-dev";
                    var highVersion = ManipulateVersionString(match.Groups, position, 1) + "-dev";

                    if (lowVersion == "0.0.0.0-dev")
                    {
                        return new[] { new Constraint.Constraint("<", highVersion) };
                    }

                    return new[]
                    {
                        new Constraint.Constraint(">=", lowVersion),
                        new Constraint.Constraint("<", highVersion),
                    };
                }
            }

            // Hyphen Range
            //
            // Specifies an inclusive set. If a partial version is provided as the first version in the inclusive range,
            // then the missing pieces are replaced with zeroes. If a partial version is provided as the second version in
            // the inclusive range, then all versions that start with the supplied parts of the tuple are accepted, but
            // nothing that would be greater than the provided tuple parts.
            if (!empty && constraint.Contains(" - "))
            {
                var segment = Regex.Split(constraint, " +- +");

                // Only one interval is allowed at the same time.
                if (segment.Length == 2)
                {
                    var leftMatch = Regex.Match(segment[0], $"^(?<from>{regexVersionString})$", RegexOptions);
                    var rightMatch = Regex.Match(segment[1], $"^(?<to>{regexVersionString})$", RegexOptions);
                    if (leftMatch.Success && rightMatch.Success)
                    {
                        var lowerConstraint = ComputeLowerVersion(leftMatch, leftMatch.Groups["from"].Value);
                        IConstraint highConstraint;
                        if ((rightMatch.Groups["minor"].Success && rightMatch.Groups["patch"].Success)
                            || rightMatch.Groups["stability"].Success
                            || rightMatch.Groups["is_branch"].Success)
                        {
                            var highVersion = Normalize(rightMatch.Groups["to"].Value);
                            highConstraint = new Constraint.Constraint("<=", highVersion);
                        }
                        else
                        {
                            var mockVersion = new StringBuilder();
                            mockVersion.Append(rightMatch.Groups["major"].Value);
                            if (rightMatch.Groups["minor"].Success)
                            {
                                mockVersion.Append(".").Append(rightMatch.Groups["minor"].Value);
                            }

                            var mockMatch = Regex.Match(mockVersion.ToString(), regexVersionNumberString, RegexOptions);
                            var position = rightMatch.Groups["minor"].Success ? 1 : 0;
                            var highVersion = ManipulateVersionString(mockMatch.Groups, position, 1) + "-dev";
                            highConstraint = new Constraint.Constraint("<", highVersion);
                        }

                        return new[] { lowerConstraint, highConstraint };
                    }
                }
            }

            // Basic Comparators
            //
            // We compare the following regular operators : <> , !=, >=, <=, ==, <, >
            //
            // ex: > 1.2.3, <= 2.0.0-beta
            Exception exception = null;
            if (!empty)
            {
                match = Regex.Match(constraint, @"^(?<operator><>|!=|>=?|<=?|==?)?\s*(?<version>.*)$", RegexOptions);
                if (match.Success)
                {
                    try
                    {
                        var matchVersion = match.Groups["version"].Value;
                        var normalizeVersion = Normalize(matchVersion);
                        if (!string.IsNullOrEmpty(stabilityModifier) && ParseStability(normalizeVersion) == Stabilities.Stable)
                        {
                            normalizeVersion += $"-{stabilityModifier}";
                        }
                        else if ((match.Groups["operator"].Value == "<"
                                || match.Groups["operator"].Value == ">=")
                                && matchVersion.Length > 4
                                && matchVersion.Substring(0, 4) != "dev-")
                        {
                            var preMatch = Regex.Match(matchVersion, $"{RegexPreReleaseString}{RegexBuildString}$", RegexOptions);
                            if (!preMatch.Success || (!preMatch.Groups["stability"].Success && !preMatch.Groups["is_branch"].Success))
                            {
                                normalizeVersion += "-dev";
                            }
                        }

                        return new[]
                        {
                            new Constraint.Constraint(
                                string.IsNullOrEmpty(match.Groups["operator"].Value) ? "=" : match.Groups["operator"].Value,
                                normalizeVersion),
                        };
                    }
#pragma warning disable CA1031
                    catch (Exception e)
                    {
                        exception = e;
                    }
#pragma warning restore CA1031
                }
            }

            var message = $"Could not parse version constraint \"{constraint}\"";
            if (exception != null)
            {
                message += $" : {exception.Message}";
            }

            throw new ParseException(message);
        }
    }
}
