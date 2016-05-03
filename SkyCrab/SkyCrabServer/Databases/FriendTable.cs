using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace SkyCrabServer.Databases
{
    static class FriendTable
    {

        private const string TABLE = "friend";
        private const string PLAYER_ID = "player_id";
        private const string FRIEND_ID = "friend_id";


        public static bool Exists(UInt32 playerId, UInt32 friendId)
        {
            const string QUERY = "SELECT 1 FROM " + TABLE + " WHERE " + PLAYER_ID + "=@playerId AND " + FRIEND_ID + "=@friendId";
            UInt32 id = PlayerTable.Create();
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
            command.Parameters.Add(new SQLiteParameter("@friendId", friendId));
            SQLiteDataReader reader = command.ExecuteReader();
            try
            {
                return reader.Read();
            }
            finally
            {
                reader.Close();
            }
        }

        public static List<UInt32> GetByPlayerId(UInt32 playerId)
        {
            const string QUERY = "SELECT " + FRIEND_ID + " FROM " + TABLE + " WHERE " + PLAYER_ID + "=@playerId";
            UInt32 id = PlayerTable.Create();
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
            SQLiteDataReader reader = command.ExecuteReader();
            try
            {
                List<UInt32> friendsIds = new List<UInt32>();
                while (reader.Read())
                    friendsIds.Add((UInt32)reader.GetInt32(0));
                return friendsIds;
            }
            finally
            {
                reader.Close();
            }
        }

        public static void Create(UInt32 playerId, UInt32 friendId)
        {
            const string QUERY = "INSERT INTO " + TABLE + " (" + PLAYER_ID + ", " + FRIEND_ID + ") VALUES (@playerId, @friendId)";
            UInt32 id = PlayerTable.Create();
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
            command.Parameters.Add(new SQLiteParameter("@friendId", friendId));
            command.ExecuteNonQuery();
        }

        public static void Delete(UInt32 playerId, UInt32 friendId)
        {
            const string QUERY = "DELETE FROM " + TABLE + " WHERE " + PLAYER_ID + "=@playerId AND " + FRIEND_ID + "=@friendId";
            UInt32 id = PlayerTable.Create();
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
            command.Parameters.Add(new SQLiteParameter("@friendId", friendId));
            command.ExecuteNonQuery();
        }

    }
}
