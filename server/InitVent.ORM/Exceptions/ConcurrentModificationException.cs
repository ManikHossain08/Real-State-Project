// -----------------------------------------------------------------------
// <copyright file="ConcurrentModificationException.cs" company="InitVent Consulting Services">
//     Copyright (c) InitVent Consulting Services All rights reserved.
// </copyright>
// <author>S.M. Saiful Islam</author>
// -----------------------------------------------------------------------

namespace InitVent.ORM.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This exception will be throw if there is any concurrent modification conflict.
    /// </summary>
    public class ConcurrentModificationException : Exception
    {
        public ConcurrentModificationException(string message, Exception innerException) : base(message, innerException) 
        { 
        }
    }
}
