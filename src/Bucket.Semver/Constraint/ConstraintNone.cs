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
    /// Defines the absence of a constraint.
    /// </summary>
    public sealed class ConstraintNone : BaseConstraint
    {
        /// <inheritdoc />
        public override bool Matches(IConstraint provider)
        {
            return true;
        }

        /// <inheritdoc />
        protected override string GetDefaultString()
        {
            return "[]";
        }
    }
}
