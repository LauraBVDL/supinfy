using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Supinfy.Models
{
    public class Playlist
    {
        public Playlist next;
        public static Playlist starter = null, last = null;
        public String playlistName;
        public short length;
        public int userId;
        public int[] idSong;

        public Playlist(int userId)
        {
            int i;
            this.userId = userId;
            idSong = new int[16];
            this.length = 0;
            ConnectionBuilder builder;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "INSERT INTO PL_OWNER(PLAYLIST_NAME, USER_ID) VALUES('" +this.playlistName + "', '" + this.userId + "')";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    i = command.ExecuteNonQuery();
                    if (i < 1) throw new Exception("USER CREATION FAILED");
                    this.next = null;
                    if (Playlist.last == null) Playlist.last = Playlist.starter = this;
                }
            }
        }

        public Playlist(String name)
        {
            ConnectionBuilder builder;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                this.length = 0;
                connect.Open();
                sql = "SELECT USER_ID FROM PL_OWNER WHERE PLAYLIST_NAME = '" + name + "'";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.userId = reader.GetInt32(0);
                            sql = "SELECT ID_SONG FROM Playlists WHERE PL_NAME = '" + name + "' ORDER BY NUMBER_IN_LIST ASC";
                            using (SqlCommand command2 = new SqlCommand(sql, connect))
                            {
                                using (SqlDataReader reader2 = command.ExecuteReader())
                                {
                                    while (reader2.Read())
                                    {
                                        this.idSong = this.Extend(this.idSong);
                                        this.idSong[this.length] = reader2.GetInt32(2);
                                        this.length++;
                                        this.next = null;
                                        if (Playlist.last == null) Playlist.last = Playlist.starter = this;
                                    }
                                    this.playlistName = name;
                                }
                            }
                        } else throw new Exception("LIST DOES NOT EXIST");
                    }
                }
            }
        }

        public int[] Extend(int[] oldTable)
        {
            int[] newTable;
            int dimension, i;
            if ((this.length % 16) == 0 && this.length > 0)
            {
                dimension = oldTable.Length + 16;
                newTable = new int[dimension];
                for (i = 0; i < oldTable.Length; i++) newTable[i] = oldTable[i];
                oldTable = newTable;
            }
            return oldTable;
        }

        //FAIRE LA VIEW CORRESPONDANTE EN UTILISANT LE CHAINAGE
        //CI DESSOUS TENIR COMPTE DU MODE

        public static Playlist List(byte mode, int userId)
        {
            ConnectionBuilder builder;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "SELECT * FROM PL_OWNER ORDER BY PLAYLIST_NAME ASC";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            new Playlist(reader.GetString(0));
                        }
                    }
                }
            }
            return Playlist.starter;
        }
    }
}