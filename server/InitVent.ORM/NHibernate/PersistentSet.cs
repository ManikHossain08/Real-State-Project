// -----------------------------------------------------------------------
// <copyright file="PersistentSet.cs" company="InitVent Consulting Services.">
//     Copyright (c) InitVent Consulting Services. All rights reserved.
// </copyright>
// <author>S.M. Saiful Islam</author>
// -----------------------------------------------------------------------

namespace InitVent.ORM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A customized Persistent Set class.
    /// </summary>
    public class PersistentSet<T> : Iesi.Collections.Generic.HashedSet<T>, IPersistentSet<T>
    {
        public PersistentSet() : base() { }

        public PersistentSet(ICollection<T> initialValues) : base(initialValues) { }
    }
}
