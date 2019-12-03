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

namespace Bucket.Semver
{
    /// <summary>
    /// Bucket runtime exception.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        public ParseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        public ParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
