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

namespace Bucket.Semver
{
    /// <summary>
    /// The version comparator facade.
    /// </summary>
    public static class Comparator
    {
        /// <summary>
        /// Evaluates the expression: <paramref name="a"/> &gt; <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The left version.</param>
        /// <param name="b">The right version.</param>
        /// <returns>True if the <paramref name="a"/> greater than <paramref name="b"/>.</returns>
        public static bool GreaterThan(string a, string b)
        {
            return Compare(a, ">", b);
        }

        /// <summary>
        /// Evaluates the expression: <paramref name="a"/> &gt;= <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The left version.</param>
        /// <param name="b">The right version.</param>
        /// <returns>True if the <paramref name="a"/> greater than or equal <paramref name="b"/>.</returns>
        public static bool GreaterThanOrEqual(string a, string b)
        {
            return Compare(a, ">=", b);
        }

        /// <summary>
        /// Evaluates the expression: <paramref name="a"/> &lt; <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The left version.</param>
        /// <param name="b">The right version.</param>
        /// <returns>True if the <paramref name="a"/> less than <paramref name="b"/>.</returns>
        public static bool LessThan(string a, string b)
        {
            return Compare(a, "<", b);
        }

        /// <summary>
        /// Evaluates the expression: <paramref name="a"/> &lt;= <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The left version.</param>
        /// <param name="b">The right version.</param>
        /// <returns>True if the <paramref name="a"/> less than or equal <paramref name="b"/>.</returns>
        public static bool LessThanOrEqual(string a, string b)
        {
            return Compare(a, "<=", b);
        }

        /// <summary>
        /// Evaluates the expression: <paramref name="a"/> == <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The left version.</param>
        /// <param name="b">The right version.</param>
        /// <returns>True if the <paramref name="a"/> equal to <paramref name="b"/>.</returns>
        public static bool Equal(string a, string b)
        {
            return Compare(a, "==", b);
        }

        /// <summary>
        /// Evaluates the expression: <paramref name="a"/> != <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The left version.</param>
        /// <param name="b">The right version.</param>
        /// <returns>True if the <paramref name="a"/> not equal to <paramref name="b"/>.</returns>
        public static bool NotEqual(string a, string b)
        {
            return Compare(a, "!=", b);
        }

        /// <summary>
        /// Evaluates the expression: <paramref name="a"/> <paramref name="operator"/> <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The left version.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="b">The right version.</param>
        /// <returns>True if the <paramref name="a"/> <paramref name="operator"/> <paramref name="b"/>.</returns>
        public static bool Compare(string a, string @operator, string b)
        {
            var constraint = new Constraint.Constraint(@operator, b);
            return constraint.Matches(new Constraint.Constraint("==", a));
        }
    }
}
