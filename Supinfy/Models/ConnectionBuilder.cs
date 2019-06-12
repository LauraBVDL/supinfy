using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Supinfy.Models
{
    public class ConnectionBuilder
    {
        public SqlConnectionStringBuilder connection;

        public ConnectionBuilder()
        {
            this.connection = new SqlConnectionStringBuilder();
            this.connection.DataSource = "supinfy.database.windows.net"; //You can use my database here
            this.connection.UserID = "LauraResponsable";
            this.connection.Password = "Griffon879";
            this.connection.InitialCatalog = "supinfy";
        }

        public int GetLastId(SqlConnection connect,  String table)
        {
            String sql;
            int id;
            id = -1;
            sql = "SELECT MAX(ID) FROM " + table;
            using (SqlCommand commandx = new SqlCommand(sql, connect)) using (SqlDataReader reader = commandx.ExecuteReader()) if (reader.Read()) reader.GetInt32(0);
            return id;
        }
    }
}