// -----------------------------------------------------------------------
// <copyright file="NHDataServices.cs" company="InitVent Consulting Services.">
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
    using System.ServiceModel;
    using System.Collections.Concurrent;
    using System.Reflection;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using NHibernate;
    using NHibernate.Linq;
    using NHibernate.Context;
    using System.Text.RegularExpressions;
    
    public class NHDataServices
    {
        /// <summary>
        /// NHibernate.Dialect.MySQL5Dialect
        /// NHibernate.Dialect.MsSql2008Dialect
        /// </summary>
        private const string dialectKey = "NHDialect";
        private const string showSql = "ShowSql";
        private const string formatSql = "FormatSql";

        private ISessionFactory sessionFactory;
        private Configuration cfg;
        private ISessionManager sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NHDataServices"/> class.
        /// </summary>
        /// <param name="connectionStringKey">The connection to the data store that this DataServices object is connection to.</param>
        /// <param name="domainAssemblyKey">The domain classes that this DataServices object contains.</param>
        public NHDataServices(string connectionString, string[] domainAssembly, string[] dataServicesList = null)
        {
            this.connectionString = connectionString;
            this.domainAssembly = domainAssembly;
            this.dataServicesList = dataServicesList;
            this.Init();

            // Get session manager so this data service class doesn't need to take care of session.
            this.sessionManager = (new SessionManagerFactory()).GetSessionManager(this.GetSessionFactory());
        }

        private string connectionString;
        private string[] domainAssembly;
        private string[] dataServicesList;
        private IDictionary<string, string> schemaNameUpdateMapping
        {
            get
            {
                if (this.dataServicesList != null)
                {
                    return this.dataServicesList.ToDictionary(
                        d => (string)Type.GetType(d).GetField("defaultSchema").GetValue(null),
                        d => NHDataServices.ExtractDatabase(System.Configuration.ConfigurationManager.ConnectionStrings[(string)Type.GetType(d).GetField("dataServicesKey").GetValue(null)].ToString()));
                }
                else
                {
                    return null;
                }
            }
        }

        public GenericSession CurrentSession
        {
            get
            {
                return this.sessionManager.CurrentSession();
            }
        }

        public void DropTables()
        {
            new SchemaExport(this.cfg).Drop(true, true);
        }

        public void CreateSchema()
        {
            new SchemaExport(this.cfg).Create(true, true);
        }

        public ISessionFactory GetSessionFactory()
        {
            if (this.sessionFactory != null)
            {
                return this.sessionFactory;
            }
            else
            {
                this.sessionFactory = this.cfg.BuildSessionFactory();
                return this.sessionFactory;
            }
        }

        public void CloseSession()
        {
            this.sessionManager.CloseSession();
        }

        /// <summary>
        /// This method will be only in logging for some diagnostic work.
        /// For single thread session manager, the number returned will always be one.
        /// </summary>
        /// <returns>Count of concurrent sessions.</returns>
        public int GetConcurrentSessionCount()
        {
            if (this.sessionManager is ContextSessionManager)
            {
                return ((ContextSessionManager)this.sessionManager).SessionMap.Keys.Count();
            }
            else
            {
                return 1;
            }
        }

        public static string ExtractDatabase(string connectionString)
        {
            return Regex.Match(connectionString, @"Database[\s]*\=[\s]*(?<database>[\w\d\.]+)").Groups["database"].Value;
        }

        private void Init()
        {
            this.cfg = new Configuration();
            try
            {
                var appSettings = System.Configuration.ConfigurationManager.AppSettings;

                // Optional settings.
                this.cfg.SetProperty(NHibernate.Cfg.Environment.ShowSql, appSettings[showSql] == null ? "true" : appSettings[showSql]);
                this.cfg.SetProperty(NHibernate.Cfg.Environment.FormatSql, appSettings[formatSql] == null ? "false" : appSettings[formatSql]);
                
                // Mandatory settings.
                // cfg.SetProperty(NHibernate.Cfg.Environment.CurrentSessionContextClass, "wcf_operation");
                this.cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, appSettings[dialectKey].ToString());
                this.cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, this.connectionString);
                this.cfg.SetProperty(NHibernate.Cfg.Environment.CommandTimeout, appSettings["commandTimeOut"]);

                // Add domain object assembly.
                foreach (string assembly in this.domainAssembly)
                {
                    this.cfg.AddAssembly(assembly);
                }

                // Update Schema Name
                if (this.schemaNameUpdateMapping != null)
                {
                    foreach (string originalSchemaName in schemaNameUpdateMapping.Keys)
                    {
                        cfg.ClassMappings.Where(cm => cm.Table.Schema == originalSchemaName).ForEach(cm => cm.Table.Schema = schemaNameUpdateMapping[originalSchemaName]);
                    }
                }
                // cfg.CurrentSessionContext<WcfOperationSessionContext>();
            }
            catch (Exception e)
            {
                throw new NHConfigurationException(string.Format("Please check your appsettings in your configruation file. You are missing one of the configuration property or the domain assemblies you are referencing are not correct. Inner exception message: {0}", e.Message), e);
            }
        }
    }
}
