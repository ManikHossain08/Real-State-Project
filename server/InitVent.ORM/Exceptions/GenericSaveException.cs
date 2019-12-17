// -----------------------------------------------------------------------
// <copyright file="GenericSaveException.cs" company="InitVent Consulting Services.">
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
    /// This exception will be thrown for general exception in NHibernate.
    /// </summary>
    public class GenericSaveException : Exception
    {
        public GenericSaveException(string message, Exception innerException) : base(message, innerException) 
        {
        }
    }
}
