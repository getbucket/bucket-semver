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

namespace Bucket.Semver.Constraint
{
    /// <summary>
    /// Represent a constraint.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// Whether the matching condition is met.
        /// </summary>
        /// <param name="provider">Provide matching objects.</param>
        /// <returns>True if the provider satisfies the condition.</returns>
        bool Matches(IConstraint provider);

        /// <summary>
        /// Gets the constraint pretty string.
        /// </summary>
        /// <returns>Returns the pretty string.</returns>
        string GetPrettyString();
    }
}
