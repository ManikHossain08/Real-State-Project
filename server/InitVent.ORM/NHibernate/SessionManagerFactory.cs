// -----------------------------------------------------------------------
// <copyright file="SessionManagerFactory.cs" company="Microsoft">
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
    using NHibernate;
    using System.ServiceModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SessionManagerFactory
    {
        public ISessionManager GetSessionManager(ISessionFactory sf)
        {
            if (OperationContext.Current != null)
            {
                return new ContextSessionManager(sf);
            }
            else
            {
                return new SingleThreadSessionManager(sf);
            }
        }
    }
}
