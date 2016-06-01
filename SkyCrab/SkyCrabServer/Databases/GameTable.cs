using System;
using System.Data.SQLite;

namespace SkyCrabServer.Databases
{
    static class GameTable
    {

        private const string TABLE = "game";
        private const string ID = "game_id";
        private const string BEGIN_TIME = "begin_time";
        private const string END_TIME = "end_time";
        private const string GAME_LOG = "game_log";


        public static UInt32 Create()
        {
            const string QUERY = "INSERT INTO " + TABLE + " (" + ID + ", " + BEGIN_TIME + ", " + GAME_LOG + ") VALUES (null, @begin_time, '')";
            lock (Globals.database._lock)
            {
                SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
                command.Parameters.Add(new SQLiteParameter("@begin_time", DateTime.Now));
                command.ExecuteNonQuery();
                UInt32 gameId = Globals.database.GetLastInssertedId();
                return gameId;
            }
        }

        public static void AddToLog(UInt32 gameId, string log)
        {
            const string QUERY = "UPDATE " + TABLE + " SET " + GAME_LOG + " = " + GAME_LOG + " || @log WHERE " + ID + "=@gameId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@gameId", gameId));
            command.Parameters.Add(new SQLiteParameter("@log", log));
            command.ExecuteNonQuery();
        }

        public static void Finish(UInt32 gameId)
        {
            const string QUERY = "UPDATE " + TABLE + " SET " + END_TIME + "=@endime WHERE " + ID + "=@gameId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@gameId", gameId));
            command.Parameters.Add(new SQLiteParameter("@endime", DateTime.Now));
            command.ExecuteNonQuery();
        }

        public static bool IdExists(UInt32 gameId)
        {
            const string QUERY = "SELECT 1 FROM " + TABLE + " WHERE " + ID + "=@gameId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@gameId", gameId));
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

        public static bool IsEnded(UInt32 gameId)
        {
            const string QUERY = "SELECT 1 FROM " + TABLE + " WHERE " + ID + "=@gameId AND " + END_TIME + " IS NOT NULL";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@gameId", gameId));
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

        public static string GetLogById(UInt32 gameId)
        {
            const string QUERY = "SELECT " + GAME_LOG + " FROM " + TABLE + " WHERE " + ID + "=@gameId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@gameId", gameId));
            SQLiteDataReader reader = command.ExecuteReader();
            try
            {
                if (!reader.Read())
                    return null;
                string gameLog = reader.GetString(0);
                return gameLog;
            }
            finally
            {
                reader.Close();
            }
        }

    }
}
