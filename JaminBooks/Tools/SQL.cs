using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class SQL
    {
        public static string ConnectionString;

        public class Param
        {
            public string Name;
            public object Value;

            public Param(string Name, object Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
        }

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
