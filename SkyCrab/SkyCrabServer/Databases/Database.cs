using System;
using System.Data.SQLite;
using System.IO;

namespace SkyCrabServer.Databases
{
    sealed class Database : IDisposable
    {

        private const string FILE_NAME = "Database.sqlite";

        //TODO do property at least
        public SQLiteConnection connection;

        public object _lock = new object();


        private static bool FileExists
        {
            get { return File.Exists(FILE_NAME); }
        }


        public Database()
        {
            try
            {
                Globals.serverConsole.Lock();
                bool fileExists = FileExists;
                if (!fileExists)
                    CreateFile();
                CreateConnection();
                if (!fileExists)
                    CreateTables();
            }
            finally
            {
                Globals.serverConsole.Unlock();
            }
        }

        private static void CreateFile()
        {
            Globals.serverConsole.WriteLine("Creating database file...");
            SQLiteConnection.CreateFile(FILE_NAME);
        }

        private void CreateConnection()
        {
            Globals.serverConsole.WriteLine("Connecting with database...");
            connection = new SQLiteConnection("Data Source=" + FILE_NAME + "; Version=3;");
            connection.Open();
        }

        private void CreateTables()
        {
            Globals.serverConsole.WriteLine("Creating tables...");
            SQLiteCommand command = new SQLiteCommand(Properties.Resources.create_tables, connection);
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            Globals.serverConsole.WriteLine("Disconnecting with database...");
            connection.Close();
        }

        public UInt32 GetLastInssertedId()
        {
            const string QUERY = "SELECT last_insert_rowid();";
            SQLiteCommand command = new SQLiteCommand(QUERY, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            try
            {
                if (!reader.Read())
                    throw new NoSuchRowException(command);
                return (UInt32)reader.GetInt32(0);
            }
            finally
            {
                reader.Close();
            }
        }

    }
}
