// -----------------------------------------------------------------------
// <copyright file="IPersistentSet.cs" company="InitVent Consulting Services.">
//     Copyright (c) InitVent Consulting Services. All rights reserved.
// </copyright>
// <author>S.M. Saiful Islam</author>
// -----------------------------------------------------------------------

namespace InitVent.ORM
{
    using System;

    /// <summary>
    /// ORM-neutral types
    /// </summary>
    public interface IPersistentSet<T> : Iesi.Collections.Generic.ISet<T> { }
}
