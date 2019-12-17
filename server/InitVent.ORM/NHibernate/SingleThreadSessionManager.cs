// -----------------------------------------------------------------------
// <copyright file="SingleThreadSessionManager.cs" company="Microsoft">
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

    /// <summary>
    /// Most likely this kind of session manager will be used in unit testing or other single thread application.
    /// The application itself needs to take care of the session opening and closing.
    /// </summary>
    public class SingleThreadSessionManager : ISessionManager
    {
        private ISessionFactory theSessionFactory;

        private GenericSession theSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleThreadSessionManager"/> class.
        /// </summary>
        /// <param name="sf">An SessionFactory object.</param>
        public SingleThreadSessionManager(ISessionFactory sf)
        {
            this.theSessionFactory = sf;
        }

        public GenericSession CurrentSession()
        {
            if (this.theSession == null || !this.theSession.IsOpen())
            {
                this.theSession = new GenericSession(this.theSessionFactory.OpenSession());
            }

            return this.theSession;
        }

        public void CloseSession()
        {
            if (this.theSession != null)
            {
                this.theSession.Close();
                this.theSession = null;
            }
        }
    }
}
