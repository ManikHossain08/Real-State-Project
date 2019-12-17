// -----------------------------------------------------------------------
// <copyright file="PersistentObject.cs" company="InitVent Consulting Services.">
//     Copyright (c) InitVent Consulting Services. All rights reserved.
// </copyright>
// <author>S.M. Saiful Islam</author>
// -----------------------------------------------------------------------

namespace InitVent.ORM
{
    using System;
    using System.Collections.Generic;
    using NHibernate;
    using System.Reflection;
    using InitVent.ORM.Exceptions;
    using System.Data;

    /**
     * This class provides services for any persistent object. This includes the ability to save and delete the object.
     */
    public class PersistentObject<T>
    {
        protected string DataSerivcesClassName { get; set; }

        protected GenericSession CurrentSession
        {
            get
            {
                return (GenericSession)Type.GetType(this.DataSerivcesClassName).GetProperty("CurrentSession", BindingFlags.Public | BindingFlags.Static).GetValue(null, null);
            }
        }

        public static T Load(object id, GenericSession session)
        {
            // Get the current session...
            ISession s = session.NHSession;

            // Try to load the object...
            // Note that Get forces a retrieval from the DB, whereas Load returns a proxy
            T po = (T)s.Get(typeof(T), id);

            // Is the object null?
            if (po == null)
            {
                throw new NonexistentPersistentObject(typeof(T).ToString(), id);
            }

            return po;
        }

        public virtual T Load(object id)
        {
            return Load(id, this.CurrentSession);
        }

        public virtual void Save(GenericSession session)
        {
            ISession s = null;
            ITransaction tx = null;

            try
            {
                // Get the current session...
                s = session.NHSession;

                // Create a Transaction...
                tx = s.BeginTransaction();

                // Save the object...
                s.SaveOrUpdate(this);

                // End the transaction...
                tx.Commit();
            }
            catch (StaleObjectStateException sose)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new ConcurrentModificationException(sose.Message, sose);
            }
            catch (Exception e)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new GenericSaveException(e.Message, e);
            }
        }

        public virtual void Save()
        {
            this.Save(this.CurrentSession);
        }

        public virtual void Delete(GenericSession session)
        {
            ISession s = null;
            ITransaction tx = null;

            try
            {
                // Get the current session...
                s = session.NHSession;

                // Create a Transaction...
                tx = s.BeginTransaction();

                // Delete the object...
                s.Delete(this);

                // End the transaction...
                tx.Commit();
            }
            catch (ADOException adoe)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new ForeignKeyConstraintException(adoe.Message, adoe);
            }
        }

        public virtual void Delete()
        {
            this.Delete(this.CurrentSession);
        }

        public virtual void ExecuteNonQuery(string query, IDictionary<string, string> parameters, CommandType type)
        {
            ISession s = null;
            ITransaction tx = null;

            try
            {
                // Get the current session...
                s = this.CurrentSession.NHSession;

                // Create a Transaction...
                tx = s.BeginTransaction();
                using (var command = s.Connection.CreateCommand())
                {
                    s.Transaction.Enlist(command);
                    command.CommandText = query;
                    command.CommandType = type;
                    command.CommandTimeout = 300;
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in parameters)
                        {
                            var param = command.CreateParameter();
                            param.ParameterName = keyValuePair.Key;
                            param.Value = keyValuePair.Value;
                            command.Parameters.Add(param);
                        }
                    }
                    int count = command.ExecuteNonQuery();

                }
                // End the transaction...
                tx.Commit();
            }
            catch (StaleObjectStateException sose)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new ConcurrentModificationException(sose.Message, sose);
            }
            catch (Exception e)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new GenericSaveException(e.Message, e);
            }
        }

        public virtual void ExecuteNonQuery(string SQL)
        {
            ISession s = null;
            ITransaction tx = null;

            try
            {
                // Get the current session...
                s = this.CurrentSession.NHSession;

                // Create a Transaction...
                tx = s.BeginTransaction();
                using (var command = s.Connection.CreateCommand())
                {
                    s.Transaction.Enlist(command);
                    command.CommandText = SQL;
                    //var versionIdParameter = command.CreateParameter();
                    //versionIdParameter.ParameterName = "id";
                    //versionIdParameter.Value = id;
                    //command.Parameters.Add(versionIdParameter);
                    int count = command.ExecuteNonQuery();

                }

                // End the transaction...
                tx.Commit();
            }
            catch (StaleObjectStateException sose)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new ConcurrentModificationException(sose.Message, sose);
            }
            catch (Exception e)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new GenericSaveException(e.Message, e);
            }
        }

        public virtual IDataReader ExecuteQuery(string SQL)
        {
            ISession s = null;
            ITransaction tx = null;
            IDataReader oReader = null;
            try
            {
                // Get the current session...
                s = this.CurrentSession.NHSession;

                // Create a Transaction...
                //tx = s.BeginTransaction();
                using (var command = s.Connection.CreateCommand())
                {
                    s.Transaction.Enlist(command);
                    command.CommandText = SQL;
                    //var versionIdParameter = command.CreateParameter();
                    //versionIdParameter.ParameterName = "id";
                    //versionIdParameter.Value = id;
                    //command.Parameters.Add(versionIdParameter);
                    oReader = command.ExecuteReader();

                }

                // End the transaction...
                //tx.Commit();
            }
            catch (StaleObjectStateException sose)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new ConcurrentModificationException(sose.Message, sose);
            }
            catch (Exception e)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new GenericSaveException(e.Message, e);
            }
            return oReader;
        }

        public virtual IDataReader ExecuteQuery(string query, IDictionary<string, string> parameters, CommandType type)
        {
            ISession s = null;
            ITransaction tx = null;
            IDataReader oReader = null;
            try
            {
                s = this.CurrentSession.NHSession;
                using (var command = s.Connection.CreateCommand())
                {
                    s.Transaction.Enlist(command);
                    command.CommandText = query;
                    command.CommandType = type;
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in parameters)
                        {
                            var param = command.CreateParameter();
                            param.ParameterName = keyValuePair.Key;
                            param.Value = keyValuePair.Value;
                            command.Parameters.Add(param);
                        }
                    }
                    oReader = command.ExecuteReader();
                }
            }
            catch (StaleObjectStateException sose)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new ConcurrentModificationException(sose.Message, sose);
            }
            catch (Exception e)
            {
                if (tx != null)
                {
                    tx.Rollback();
                }

                throw new GenericSaveException(e.Message, e);
            }
            return oReader;
        }
    }
}
