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

namespace Bucket.Semver
{
    /// <summary>
    /// Represents a version parser.
    /// </summary>
    public interface IVersionParser
    {
        /// <summary>
        /// Normalizes a version string to be able to perform comparisons on it.
        /// </summary>
        /// <param name="version">The version needs to normalize.</param>
        /// <param name="fullVersion">An optional complete version string to give more context.(use for debug log).</param>
        /// <returns>Return's version can be able to perform comparisons on it.</returns>
        string Normalize(string version, string fullVersion = null);

        /// <summary>
        /// Normalizes a branch name to be able to perform comparisons on it.
        /// </summary>
        /// <param name="branch">The branch name.</param>
        /// <returns>Returns normalized branch name.</returns>
        string NormalizeBranch(string branch);

        /// <summary>
        /// Parses a constraint string into MultiConstraint and/or Constraint objects.
        /// </summary>
        /// <param name="constraints">The version-constrained string.</param>
        /// <returns>Returns the <see cref="IConstraint"/> objects.</returns>
        IConstraint ParseConstraints(string constraints);
    }
}
