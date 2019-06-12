using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;

namespace Supinfy.Models
{
    public class User
    {
        public DateTime creation;
        public String username, lastname, firstname, password, email;
        public int id;
        public byte role;

        public User(String username, String password, String lastname, String firstname, String email) {
            this.username = username;
            this.password = password;
            this.lastname = lastname; this.firstname = firstname;
            this.email = email;
            this.role = 0;
            this.creation = new DateTime();
            this.Write();
        }

        public User(String username) {
            ConnectionBuilder builder;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString)) {
                String sql;
                connect.Open();
                sql = "SELECT * FROM Users WHERE USERNAME = " + username;
                using (SqlCommand command = new SqlCommand(sql, connect)) {
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                this.id = reader.GetInt32(0);
                                this.email = reader.GetString(1);
                                this.username = reader.GetString(2);
                                this.firstname = reader.GetString(3);
                                this.lastname = reader.GetString(4);
                                this.creation = reader.GetDateTime(5);
                                this.role = reader.GetByte(6);
                                this.password = reader.GetString(7);
                            }
                        }
                    } catch (Exception e) { Console.WriteLine("Invalid field"); }
                }
            }
        }

        public void Write() {
            ConnectionBuilder builder;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString)) {
                String sql;
                connect.Open();
                sql = "INSERT INTO Users (USERNAME, PASSWORD, FIRSTNAME, LASTNAME, EMAIL, ROLE) VALUES ('" + this.username + "','" + this.password + "','" + this.firstname + "','" +
                    this.lastname + "','" + this.email + "','" + this.role + "')";
                using (SqlCommand command = new SqlCommand(sql, connect)) command.ExecuteNonQuery();
            }
        }
    }
}