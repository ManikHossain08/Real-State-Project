// -----------------------------------------------------------------------
// <copyright file="GenericSession.cs" company="InitVent Consulting Services.">
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
    using NHibernate.Linq;

    /// <summary>
    /// This generic session is created to wrap the NHibernate session.
    /// So other project that references this session doesn't need to include NHibernate library.
    /// </summary>
    public class GenericSession : IDisposable
    {
        private ISession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericSession"/> class.
        /// Create a generic session using a NHibernate session.
        /// </summary>
        /// <param name="session">NHibernate session.</param>
        public GenericSession(ISession session)
        {
            this.session = session;
        }

        public ISession NHSession
        {
            get
            {
                return this.session;
            }
        }

        public IQueryable<T> Query<T>()
        {
            return this.session.Query<T>();
        }

        public void Save(object obj)
        {
            this.session.Save(obj);
            this.session.Flush();
        }

        public void Update(object obj)
        {
            this.session.Update(obj);
            this.session.Flush();
        }

        public void SaveOrUpdate(object obj)
        {
            this.session.SaveOrUpdate(obj);
            this.session.Flush();
        }

        public void Delete(object obj)
        {
            this.session.Delete(obj);
            this.session.Flush();
        }

        public void DeleteAll(IEnumerable<object> obj)
        {
            foreach (object o in obj)
            {
                this.session.Delete(o);
            }

            this.session.Flush();
        }

        public void Flush()
        {
            this.session.Flush();
        }

        public void Dispose()
        {
            this.session.Dispose();
            this.session = null;
        }

        public void Close()
        {
            if (this.session != null && this.session.IsOpen)
            {
                this.session.Close();
            }
        }

        public bool IsOpen()
        {
            return this.session.IsOpen;
        }
    }
}
