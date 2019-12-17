// -----------------------------------------------------------------------
// <copyright file="ForeignKeyConstraintException.cs" company="InitVent Consulting Services">
//     Copyright (c) InitVent Consulting Services. All rights reserved.
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
    /// This exception will be thrown if there is any foreign key constraint error.
    /// </summary>
    public class ForeignKeyConstraintException : Exception
    {
        public ForeignKeyConstraintException(string message, Exception innerException) : base(message, innerException) 
        { 
        }
    }
}
