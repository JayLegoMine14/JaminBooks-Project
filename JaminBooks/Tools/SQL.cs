using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Tools
{   /// <summary>
    /// Manages the connection to the database and the execution of stored procedures
    /// </summary>
    public class SQL
    {
        /// <summary>
        /// The database connection string. Typically set when the server initializes.
        /// </summary>
        public static string ConnectionString;

        /// <summary>
        /// Models a parameter in a stored procedure
        /// </summary>
        public class Param
        {
            /// <summary>
            /// The name of the parameter.
            /// </summary>
            public string Name;
            /// <summary>
            /// The value of the parameter.
            /// </summary>
            public object Value;

            /// <summary>
            /// Instantiate a parameter with the given name and value.
            /// </summary>
            /// <param name="Name">The name of the parameter</param>
            /// <param name="Value">The value of the parameter</param>
            public Param(string Name, object Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
        }

        /// <summary>
        /// Executes the specified stored procedure with the given parameters.
        /// </summary>
        /// <param name="procName">The name of the stored procedure</param>
        /// <param name="parameters">An array of parameters</param>
        /// <returns>A DataTable of results from the query</returns>
        public static DataTable Execute(string procName, params Param[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (Param p in parameters)
                        cmd.Parameters.Add(new SqlParameter(p.Name, p.Value));

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    var ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables.Count == 0)
                        return new DataTable();
                    else
                        return ds.Tables[ds.Tables.Count - 1];
                }
            }
        }
    }
}
