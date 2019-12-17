// -----------------------------------------------------------------------
// <copyright file="ISessionManager.cs" company="InitVent Consulting Services.">
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
    /// This is an interface defined for different instances that could automatically handle NHibernate session.
    /// </summary>
    public interface ISessionManager
    {
        GenericSession CurrentSession();

        void CloseSession();
    }
}
