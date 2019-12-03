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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bucket.Semver.Constraint
{
    /// <summary>
    /// Define a constraint.
    /// </summary>
    [DebuggerDisplay("Constraint: {ToString()}")]
    public class Constraint : BaseConstraint
    {
        /// <summary>
        /// Operator to enum translation table.
        /// </summary>
        private static IDictionary<string, OperatorTypes> transStrOp = new Dictionary<string, OperatorTypes>
        {
            { "=",  OperatorTypes.Equal },
            { "==",  OperatorTypes.Equal },
            { "<",  OperatorTypes.Less },
            { "<=",  OperatorTypes.LessEqual },
            { ">",  OperatorTypes.Greate },
            { ">=",  OperatorTypes.GreateEqual },
            { "!=",  OperatorTypes.NonEqual },
            { "<>",  OperatorTypes.NonEqual },
        };

        /// <summary>
        /// Enum to operator translation table.
        /// </summary>
        private static IDictionary<OperatorTypes, string> transOpStr = new Dictionary<OperatorTypes, string>
        {
            { OperatorTypes.Equal,       "==" },
            { OperatorTypes.Less,        "<" },
            { OperatorTypes.LessEqual,   "<=" },
            { OperatorTypes.Greate,      ">" },
            { OperatorTypes.GreateEqual, ">=" },
            { OperatorTypes.NonEqual,    "!=" },
        };

        private readonly OperatorTypes operatorType;
        private readonly string version;

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint"/> class.
        /// </summary>
        /// <param name="operatorString">The operator string.</param>
        /// <param name="version">The version.</param>
        public Constraint(string operatorString, string version)
        {
            if (!transStrOp.TryGetValue(operatorString, out OperatorTypes @operator))
            {
                throw new ArgumentException(
                    $"Invalid operator {operatorString} given, expected one of: {string.Join(", ", GetSupportedOperators())}");
            }

            operatorType = @operator;
            this.version = version;
        }

        /// <inheritdoc />
        public override bool Matches(IConstraint provider)
        {
            if (provider is Constraint constraint)
            {
                return MatchSpecific(constraint);
            }

            // turn matching around to find a match, this instance from Constraint
            return provider == null || provider.Matches(this);
        }

        /// <summary>
        /// Whether the matching condition is met.
        /// </summary>
        /// <param name="provider">The constraint instance.</param>
        /// <param name="compareBranches">Whether is compare branche.</param>
        /// <returns>True if the provider satisfies the condition.</returns>
        public bool MatchSpecific(Constraint provider, bool compareBranches = false)
        {
            var opString = transOpStr[operatorType];
            var providerOPString = transOpStr[provider.operatorType];
            var noEqualOp = opString.Replace("=", string.Empty);
            var providerNoEqualOp = providerOPString.Replace("=", string.Empty);

            var isEqualOp = operatorType == OperatorTypes.Equal;
            var isNonEqualOp = operatorType == OperatorTypes.NonEqual;
            var isProviderEqualOp = provider.operatorType == OperatorTypes.Equal;
            var isProviderNonEqualOp = provider.operatorType == OperatorTypes.NonEqual;

            // '!=' operator is match when other operator is not '=='
            // operator or version is not match these kinds of comparisons
            // always have a solution
            if (isNonEqualOp || isProviderNonEqualOp)
            {
                return (!isEqualOp && !isProviderEqualOp)
                    || VersionCompare(provider.version, version, "!=", compareBranches);
            }

            // an example for the condition is <= 2.0 & < 1.0
            // these kinds of comparisons always have a solution
            if (operatorType != OperatorTypes.Equal && noEqualOp == providerNoEqualOp)
            {
                return true;
            }

            if (VersionCompare(provider.version, version, opString, compareBranches))
            {
                // special case, e.g. require >= 1.0 and provide < 1.0
                // 1.0 >= 1.0 but 1.0 is outside of the provided interval
                if (provider.version == version
                      && providerOPString == providerNoEqualOp
                      && opString != noEqualOp)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override string GetDefaultString()
        {
            return transOpStr[operatorType] + " " + version;
        }

        /// <summary>
        /// Compliance with operating conditions.
        /// </summary>
        /// <param name="left">The left vaule.</param>
        /// <param name="right">The right value.</param>
        /// <param name="operatorString">The operator string.</param>
        /// <param name="compareBranches">Whether is compare branch.</param>
        /// <returns>True if compliance with operating conditions.</returns>
        private static bool VersionCompare(string left, string right, string operatorString, bool compareBranches = false)
        {
            if (!transStrOp.TryGetValue(operatorString, out OperatorTypes @operator))
            {
                throw new ArgumentException(
                    $"Invalid operator {operatorString} given, expected one of: {string.Join(", ", GetSupportedOperators())}");
            }

            var leftIsBranch = IsBranch(left);
            var rightIsBranch = IsBranch(right);

            if (leftIsBranch && rightIsBranch)
            {
                return @operator == OperatorTypes.Equal && left == right;
            }

            // when branches are not comparable, we make sure dev branches never match anything
            if (!compareBranches && (leftIsBranch || rightIsBranch))
            {
                return false;
            }

            if (compareBranches && (leftIsBranch || rightIsBranch))
            {
                int result = 0;
                if (leftIsBranch && rightIsBranch)
                {
                    result = string.Compare(left, right, StringComparison.CurrentCultureIgnoreCase);
                }
                else if (leftIsBranch && !rightIsBranch)
                {
                    result = -1;
                }
                else if (!leftIsBranch && rightIsBranch)
                {
                    result = 1;
                }

                switch (operatorString)
                {
                    case "==":
                        return result == 0;
                    case ">":
                        return result >= 1;
                    case ">=":
                        return result >= 1 || result == 0;
                    case "<":
                        return result <= -1;
                    case "<=":
                        return result <= -1 || result == 0;
                    case "!=":
                        return result != 0;
                }
            }

            return SemVersion.Compare(left, right, transOpStr[@operator]);
        }

        private static bool IsBranch(string version)
        {
            if (string.IsNullOrEmpty(version) || version.Length < 4)
            {
                return false;
            }

            return version.Substring(0, 4) == "dev-";
        }

        private static string[] GetSupportedOperators()
        {
            return transStrOp.Keys.ToArray();
        }
    }
}
