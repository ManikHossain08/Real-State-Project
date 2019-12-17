using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Configuration;

namespace iMFAS.Domain.Repository.SQLExecutor
{
    /// <summary>
    /// This class defines abstract construct for executing raw SQL query
    /// </summary>
    public abstract class ASQLExecutor
    {
        /// <summary>
        /// Connection string
        /// </summary>
        protected string connectionstring;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ConnectionString">Connection String</param>
        public ASQLExecutor(string ConnectionString)
        {
            connectionstring = ConnectionString;
        }

        /// <summary>
        /// This method executes the SQL query and returns Data Reader object
        /// </summary>
        /// <param name="query">Query String</param>
        /// <returns>Data Reader Object</returns>
        public abstract DbDataReader ExecuteReader(string query);

        /// <summary>
        /// Releases connection resources
        /// </summary>
        public abstract void ReleaseResource();
  
    }
}
