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

#pragma warning disable SA1602

using System.Runtime.Serialization;

namespace Bucket.Semver
{
    /// <summary>
    /// The stability flag.
    /// </summary>
#pragma warning disable CA1717
    public enum Stabilities
#pragma warning restore CA1717
    {
        [EnumMember(Value = "stable")]
        Stable = 0,

        [EnumMember(Value = "RC")]
        RC = 5,

        [EnumMember(Value = "beta")]
        Beta = 10,

        [EnumMember(Value = "alpha")]
        Alpha = 15,

        [EnumMember(Value = "dev")]
        Dev = 20,
    }
}
