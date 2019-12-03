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
    /// The operation types of compare.
    /// </summary>
    internal enum OperatorTypes
    {
        /// <summary>
        /// Are equal.
        /// </summary>
        Equal,

        /// <summary>
        /// Are less.
        /// </summary>
        Less,

        /// <summary>
        /// Are less equal.
        /// </summary>
        LessEqual,

        /// <summary>
        /// Are greate.
        /// </summary>
        Greate,

        /// <summary>
        /// Are greate equal
        /// </summary>
        GreateEqual,

        /// <summary>
        /// Are non equal
        /// </summary>
        NonEqual,
    }
}
