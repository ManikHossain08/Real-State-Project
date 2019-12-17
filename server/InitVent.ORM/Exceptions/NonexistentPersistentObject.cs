// -----------------------------------------------------------------------
// <copyright file="NonexistentPersistentObject.cs" company="InitVent Consulting Services.">
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
    /// This exception will be thrown when we couldn't load the object by object id.
    /// </summary>
    public class NonexistentPersistentObject : Exception
    {
        public NonexistentPersistentObject(string typeName, object id) : base(string.Format("There is no persistent object of type '{0}' whose id is '{1}'", typeName, id)) 
        {
        }
    }
}
