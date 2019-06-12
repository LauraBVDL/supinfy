using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Supinfy.Models
{
    public class SoundModel
    {
        public SoundModel next;
        public static SoundModel starter = null, last = null;
        public String title, filename;
        public int numberOfPlayings, id;

        public SoundModel(String title, String filename)
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
                        if (SoundModel.starter == null) SoundModel.starter = this;
                        else SoundModel.last.next = this;
                        SoundModel.last = this;
                    }
                }
            }
        }

        public SoundModel(String title)
        {
            ConnectionBuilder builder;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "SELECT TOP 1 * FROM Songs WHERE TITLE = '" + title + "'";
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
                                if (SoundModel.starter == null) SoundModel.starter = this;
                                else SoundModel.last.next = this;
                                SoundModel.last = this;
                            }
                        }
                    }
                    catch (Exception e) {; }
                }
            }
        }

        public static String GetFileName(int i)
        {
            ConnectionBuilder builder;
            String fileName = null;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "SELECT TOP 1 DATA FROM Songs WHERE ID='" + i + "'";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) fileName = reader.GetString(0);
                    }
                }
            }
            return fileName;
        }

        public static String Increment(String title)
        {
            ConnectionBuilder builder;
            String resp = "ERROR";
            int i;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "UPDATE Songs SET NUMBER_OF_PLAYINGS = NUMBER_OF_PLAYINGS + 1 WHERE TITLE = '" + title + "'";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    i = command.ExecuteNonQuery();
                    if (i > 0) resp = "INCREASED";
                }
            }
            return resp;
        }

        public static SoundModel List()
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
                            Console.WriteLine("va dans while");
                            new SoundModel(reader.GetString(1));
                        }
                    }
                }
            }
            return SoundModel.starter;
        }
    }
}