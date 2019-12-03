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

namespace Bucket.Semver.Constraint
{
    /// <summary>
    /// Defines a conjunctive or disjunctive set of constraints.
    /// </summary>
    public sealed class ConstraintMulti : BaseConstraint
    {
        private readonly IConstraint[] constraints;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintMulti"/> class.
        /// </summary>
        /// <param name="constraints">An array of constraints.</param>
        public ConstraintMulti(params IConstraint[] constraints)
        {
            this.constraints = constraints ?? Array.Empty<IConstraint>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the constraints should be treated as "and" conjunctive or disjunctive.
        /// </summary>
        public bool IsConjunctive { get; set; } = true;

        /// <inheritdoc />
        public override bool Matches(IConstraint provider)
        {
            if (IsConjunctive)
            {
                // With the "and" conjunctive, all matches
                // will be considered as matches.
                foreach (var constraint in constraints)
                {
                    if (!constraint.Matches(provider))
                    {
                        return false;
                    }
                }

                return true;
            }

            // As long as there is a match, it is
            // considered a match
            foreach (var constraint in constraints)
            {
                if (constraint.Matches(provider))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets an array of constraints.
        /// </summary>
        /// <returns>Returns an array of constraints.</returns>
        public IConstraint[] GetConstraints()
        {
            return constraints;
        }

        /// <inheritdoc />
        protected override string GetDefaultString()
        {
            var constraintList = new LinkedList<string>();
            foreach (var constraint in constraints)
            {
                constraintList.AddLast(constraint.ToString());
            }

            return $"[{string.Join(IsConjunctive ? " " : " || ", constraintList)}]";
        }
    }
}
