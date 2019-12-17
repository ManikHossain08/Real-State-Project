// -----------------------------------------------------------------------
// <copyright file="ContextSessionManager.cs" company="InitVent Consulting Services.">
//     Copyright (c) InitVent Consulting Services. All rights reserved.
// </copyright>
// <author>S.M. Saiful Islam</author>
// -----------------------------------------------------------------------

namespace InitVent.ORM
{
    using NHibernate;
    using System;
    using System.ServiceModel;
    using System.Collections.Generic;
    using System.Collections.Concurrent;

    public class ContextSessionManager : ISessionManager
    {
        // Each thread visiting DataServices class will have its own context value.
        [ThreadStatic]
        private static InstanceContext theInstanceContext;

        /// <summary>
        /// The instance of NHibernate session factory.
        /// It will manage the creation of NHibernate session.
        /// </summary>
        private ISessionFactory theSessionFactory;

        private IDictionary<InstanceContext, GenericSession> theSessionMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextSessionManager"/> class.
        /// This session manager is created for WCF services.
        /// It will open the session when the service method starts to call DataServices class and it will close the session when the method execution is done.
        /// The session will be reused during the lifecycle of that instance.
        /// </summary>
        /// <param name="sf">NHibernate session factory.</param>
        public ContextSessionManager(ISessionFactory sf)
        {
            this.theSessionFactory = sf;
        }

        /// <summary>
        /// Gets cache dictionary for instance sessions.
        /// </summary>
        public IDictionary<InstanceContext, GenericSession> SessionMap
        {
            get
            {
                if (this.theSessionMap == null)
                {
                    this.theSessionMap = new ConcurrentDictionary<InstanceContext, GenericSession>();
                }

                return this.theSessionMap;
            }
        }

        public GenericSession CurrentSession()
        {
            GenericSession theSession;
            theInstanceContext = OperationContext.Current.InstanceContext;

            if (!this.SessionMap.TryGetValue(theInstanceContext, out theSession))
            {
                theSession = new GenericSession(this.theSessionFactory.OpenSession());
                this.SessionMap.Add(theInstanceContext, theSession);
                theInstanceContext.Closed += delegate(object sender, EventArgs e)
                {
                    this.SessionMap.Remove(theInstanceContext);
                    theSession.Close();
                };
            }

            return theSession;
        }

        /// <summary>
        /// Close current session.
        /// Won't be used for this ContextSessionManager as the session is automatically closed.
        /// It's just an implementation of the interface method.
        /// </summary>
        public void CloseSession()
        {
            this.CurrentSession().Close();
        }
    }
}