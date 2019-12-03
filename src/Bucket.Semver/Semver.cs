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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.Semver
{
    /// <summary>
    /// Semver version tool.
    /// </summary>
    public static class Semver
    {
        private static IVersionParser versionParser = new VersionParser();

        /// <summary>
        /// Sets the version parser instance.
        /// </summary>
        /// <param name="versionParser">The version parser instance.</param>
        public static void SetParser(IVersionParser versionParser)
        {
            Semver.versionParser = versionParser;
        }

        /// <summary>
        /// Gets the version parser instance.
        /// </summary>
        /// <returns>Returns the version parser instance.</returns>
        public static IVersionParser GetParser()
        {
            return versionParser;
        }

        /// <summary>
        /// Determine if given version satisfies given constraints.
        /// </summary>
        /// <param name="version">The given version.</param>
        /// <param name="constraints">The constraints version.</param>
        /// <returns>True if the versions meet each other.</returns>
        public static bool Satisfies(string version, string constraints)
        {
            var provider = new Constraint.Constraint("==", versionParser.Normalize(version));
            var constraint = versionParser.ParseConstraints(constraints);
            return constraint.Matches(provider);
        }

        /// <summary>
        /// Return all versions that satisfy given constraints.
        /// </summary>
        /// <param name="versions">An array of given versions.</param>
        /// <param name="constraints">The constraints version.</param>
        /// <returns>Returns an array of satisfy versions.</returns>
        public static string[] SatisfiesBy(IEnumerable<string> versions, string constraints)
        {
            return Filter(versions, (version) => Satisfies(version, constraints));
        }

        /// <summary>
        /// Sort given array of versions.
        /// </summary>
        /// <param name="versions">An array of given versions.</param>
        /// <param name="desc">Whether is sort by desc. otherwise asc.</param>
        /// <returns>Returns an array of sorted versions.</returns>
        public static string[] Sort(IEnumerable<string> versions, bool desc = false)
        {
            var direction = desc ? -1 : 1;
            var sorted = new List<(string Normalized, string Version)>();

            foreach (var version in versions)
            {
                sorted.Add((versionParser.Normalize(version), version));
            }

            sorted.Sort((left, right) =>
            {
                if (left.Normalized == right.Normalized)
                {
                    return 0;
                }

                return Comparator.LessThan(left.Normalized, right.Normalized) ? -direction : direction;
            });

            return (from item in sorted select item.Version).ToArray();
        }

        /// <summary>
        /// Each value in the source array is passed to the callback function.
        /// If the callback function is equal to the <paramref name="expected"/>
        /// value, the current value in the input array is added to the result array.
        /// </summary>
        /// <remarks>
        /// This code copied from https://github.com/CatLib/Core/blob/master/src/CatLib.Core/Support/Arr.cs.
        /// </remarks>
        /// <typeparam name="T">The type of array.</typeparam>
        /// <param name="sources">The specified array.</param>
        /// <param name="predicate">The callback.</param>
        /// <param name="expected">The expected value.</param>
        /// <returns>Returns an filtered array.</returns>
        private static T[] Filter<T>(IEnumerable<T> sources, Predicate<T> predicate, bool expected = true)
        {
            var collection = new LinkedList<T>();
            foreach (var source in sources)
            {
                if (predicate.Invoke(source) == expected)
                {
                    collection.AddLast(source);
                }
            }

            return collection.ToArray();
        }
    }
}
