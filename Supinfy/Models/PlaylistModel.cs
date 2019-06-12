using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Supinfy.Models
{
    public class PlaylistModel
    {
        public PlaylistModel next;
        public static PlaylistModel starter = null, last = null;
        public String playlistName;
        public short length;
        public int userId;
        public int[] idSong;

        public PlaylistModel(int userId, String name)
        {
            int i;
            this.userId = userId;
            this.playlistName = name;
            this.idSong = new int[16];
            this.length = 0;
            ConnectionBuilder builder;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "INSERT INTO PL_OWNER(PLAYLIST_NAME, USER_ID) VALUES('" + this.playlistName + "', '" + this.userId + "')";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    i = command.ExecuteNonQuery();
                    if (i < 1) throw new Exception("USER CREATION FAILED");
                    this.next = null;
                    if (PlaylistModel.starter == null) PlaylistModel.starter = this;
                    else PlaylistModel.last.next = this;
                    PlaylistModel.last = this;
                }
            }
        }

        public PlaylistModel(String name)
        {
            ConnectionBuilder builder;
            int t;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                this.idSong = new int[16];
                this.length = 0;
                connect.Open();
                sql = "SELECT TOP 1 USER_ID FROM PL_OWNER WHERE PLAYLIST_NAME = '" + name + "'";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) this.userId = reader.GetInt32(0);
                        else throw new Exception("LIST DOES NOT EXIST");
                    }
                }
                sql = "SELECT ISNULL(ID_SONG, 0) FROM Playlists WHERE PL_NAME = '" + name + "' ORDER BY NUMBER_IN_LIST ASC";
                using (SqlCommand command2 = new SqlCommand(sql, connect))
                {
                    using (SqlDataReader reader2 = command2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            t = reader2.GetInt32(0);
                            if (t <= 0) break;
                            this.idSong = this.Extend(this.idSong);
                            this.idSong[this.length] = t;
                            this.length++;
                            this.next = null;
                        }
                        this.playlistName = name;
                        if (PlaylistModel.starter == null) PlaylistModel.starter = this;
                        else PlaylistModel.last.next = this;
                        PlaylistModel.last = this;
                    }
                }
            }
        }

        public static int Add(String songName, String listName) {
            ConnectionBuilder builder;
            int n = 0, k = 0, i = 0;
            builder = new ConnectionBuilder();
            using (SqlConnection connect = new SqlConnection(builder.connection.ConnectionString))
            {
                String sql;
                connect.Open();
                sql = "SELECT ISNULL(MAX(NUMBER_IN_LIST), 0) FROM Playlists WHERE PL_NAME = '" + listName +"'";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) n = reader.GetInt32(0) + 1;
                    }
                }
                sql = "SELECT ID FROM Songs WHERE TITLE = '" + songName + "'";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) k = reader.GetInt32(0);
                    }
                }
                sql = "INSERT INTO Playlists (PL_NAME, NUMBER_IN_LIST, ID_SONG) VALUES ('" + listName + "', '" + n + "', '" + k + "')";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    i = command.ExecuteNonQuery();
                }
            }
            return i;
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

        public static PlaylistModel List(byte mode, int userId)
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
                            new PlaylistModel(reader.GetString(1));
                        }
                    }
                }
            }
            return PlaylistModel.starter;
        }
    }
}