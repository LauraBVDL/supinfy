using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Supinfy.Models
{
    public class Sound
    {
        public Sound next;
        public static Sound starter = null, last = null;
        public String title, filename;
        public int numberOfPlayings, id;
         
        public Sound(String title, String filename)
        {
            ConnectionBuilder builder;
            int i;
            this.numberOfPlayings = 0;
            this.title = title; this.filename = filename;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "INSERT INTO Songs(TITLE, DATA, NUMBER_OF_PLAYINGS) VALUES('" + title + "', '" + filename + "', '" +
                    this.numberOfPlayings + "')";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    i = command.ExecuteNonQuery();
                    if (i > 0)
                    {
                        this.id = builder.GetLastId(connect, "Songs");
                        this.next = null;
                        if (Sound.last == null) Sound.last = Sound.starter = this;
                    }
                }
            }
        }

        public Sound(String title)
        {
            ConnectionBuilder builder;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "SELECT * FROM Songs WHERE TITLE = '" + title + "' LIMIT 1";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                this.id = reader.GetInt32(0);
                                this.title = reader.GetString(1);
                                this.filename = reader.GetString(2);
                                this.numberOfPlayings = reader.GetInt32(3);
                                this.next = null;
                                if (Sound.last == null) Sound.last = Sound.starter = this;
                            }
                        }
                    }
                    catch (Exception e) {; }
                }
            }
        }

        public static Sound List()
        {
            ConnectionBuilder builder;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "SELECT * FROM Songs ORDER BY TITLE ASC";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            new Sound(reader.GetString(1));
                        }
                    }
                }
            }
            return Sound.starter;
        }
    }
}