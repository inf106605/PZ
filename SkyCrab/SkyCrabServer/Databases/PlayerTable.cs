using System;
using System.Data.SQLite;

namespace SkyCrabServer.Databases
{
    static class PlayerTable
    {

        private const string TABLE = "player";
        private const string ID = "player_id";


        public static bool CheckIdExists(UInt32 id)
        {
            const string QUERY = "SELECT 1 FROM " + TABLE + " WHERE " + ID + "=@id";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@id", id));
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

        public static UInt32 Create()
        {
            lock (Globals.database._lock)
            {
                const string QUERY = "INSERT INTO " + TABLE + " (" + ID + ") VALUES (null)";
                SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
                command.ExecuteNonQuery();
                UInt32 id = Globals.database.GetLastInssertedId();
                return id;
            }
        }

    }
}
