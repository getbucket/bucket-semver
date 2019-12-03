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

namespace Bucket.Semver.Constraint
{
    /// <summary>
    /// a base class that represents a constraint.
    /// </summary>
    public abstract class BaseConstraint : IConstraint
    {
        private string prettyString;

        /// <inheritdoc />
        public abstract bool Matches(IConstraint provider);

        /// <summary>
        /// Sets the constraint pretty string.
        /// </summary>
        /// <param name="prettyString">The pretty string.</param>
        public void SetPrettyString(string prettyString)
        {
            this.prettyString = prettyString;
        }

        /// <inheritdoc />
        public string GetPrettyString()
        {
            if (!string.IsNullOrEmpty(prettyString))
            {
                return prettyString;
            }

            return GetDefaultString();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return GetDefaultString();
        }

        /// <summary>
        /// Get the default description string.
        /// </summary>
        /// <returns>Returns the default description string.</returns>
        protected abstract string GetDefaultString();
    }
}
