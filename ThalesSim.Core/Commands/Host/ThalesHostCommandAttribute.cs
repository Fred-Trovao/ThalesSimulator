﻿/*
 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;

namespace ThalesSim.Core.Commands.Host
{
    /// <summary>
    /// Attribute used to decorate host command implementations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ThalesHostCommandAttribute : Attribute
    {
        /// <summary>
        /// Get/set the command code.
        /// </summary>
        public string CommandCode { get; set; }

        /// <summary>
        /// Get/set the response code.
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Get/set the response code after printer I/O is done.
        /// </summary>
        public string ResponseCodeAfterIo { get; set; }

        /// <summary>
        /// Get/set the command description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Creates a new instance of this class for
        /// commands that do not perform printer I/O.
        /// </summary>
        /// <param name="commandCode">Command code.</param>
        /// <param name="responseCode">Response code.</param>
        /// <param name="description">Command description.</param>
        public ThalesHostCommandAttribute(string commandCode, string responseCode, string description) : this(commandCode, responseCode, string.Empty, description) { }

        /// <summary>
        /// Creates a new instance of this class for
        /// commands that also perform printer I/O.
        /// </summary>
        /// <param name="commandCode">Command code.</param>
        /// <param name="responseCode">Response code.</param>
        /// <param name="responseCodeAfterIo">Response code after I/O is performed.</param>
        /// <param name="description">Command description.</param>
        public ThalesHostCommandAttribute (string commandCode, string responseCode, string responseCodeAfterIo, string description)
        {
            CommandCode = commandCode;
            ResponseCode = responseCode;
            ResponseCodeAfterIo = responseCodeAfterIo;
            Description = description;
        }
    }
}
