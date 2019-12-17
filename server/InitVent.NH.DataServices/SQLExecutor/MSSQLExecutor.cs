using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;


namespace iMFAS.Domain.Repository.SQLExecutor
{
    /// <summary>
    /// Implementaion of SQLExecutor for MM SQL Server
    /// </summary>
    public class MSSQLExecutor : ASQLExecutor
    {
        DbConnection con;
        SqlDataReader dr;

        public MSSQLExecutor(string ConnectionString)
            :base(ConnectionString)
        {

        }

        /// <summary>
        /// Prepares database connection and executes the read query
        /// </summary>
        /// <param name="query">the query to execute</param>
        /// <returns>the data reader of the executed query</returns>
        public override DbDataReader ExecuteReader(string query)
        {
            con = new SqlConnection(connectionstring);
            SqlCommand com = new SqlCommand();
            com.Connection =(SqlConnection) con;
            com.CommandText = query;

            com.CommandTimeout = 600;

            con.Open();

            dr=com.ExecuteReader();

            return dr;
        }
        
        /// <summary>
        /// This method releases the opened resources
        /// </summary>
        public override void ReleaseResource()
        {
            if (dr != null)
            {
                dr.Close();
            }

            if (con != null)
            {
                con.Close();
            }
        }       
    }
}
