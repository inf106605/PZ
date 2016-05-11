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
            lock (Globals.database._lock)
            {
                const string QUERY = "INSERT INTO " + TABLE + " (" + ID + ", " + BEGIN_TIME + ", " + GAME_LOG + ") VALUES (null, @begin_time, '')";
                SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
                command.Parameters.Add(new SQLiteParameter("@begin_time", DateTime.Now));
                command.ExecuteNonQuery();
                UInt32 id = Globals.database.GetLastInssertedId();
                return id;
            }
        }

    }
}
