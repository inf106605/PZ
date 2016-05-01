using System;
using System.Data.SQLite;
using System.IO;

namespace SkyCrabServer.Databases
{
    sealed class Database : IDisposable
    {

        private const string FILE_NAME = "Database.sqlite";

        private SQLiteConnection connection;

        
        private static bool FileExists
        {
            get { return File.Exists(FILE_NAME); }
        }


        public Database()
        {
            try
            {
                SkyCrab_Server.serverConsole.Lock();
                bool fileExists = FileExists;
                if (!fileExists)
                    CreateFile();
                CreateConnection();
                if (!fileExists)
                    CreateTables();
            }
            finally
            {
                SkyCrab_Server.serverConsole.Unlock();
            }
        }

        private static void CreateFile()
        {
            SkyCrab_Server.serverConsole.WriteLine("Creating database file...");
            SQLiteConnection.CreateFile(FILE_NAME);
        }

        private void CreateConnection()
        {
            SkyCrab_Server.serverConsole.WriteLine("Connecting with database...");
            connection = new SQLiteConnection("Data Source=" + FILE_NAME + "; Version=3;");
            connection.Open();
        }

        private void CreateTables()
        {
            SkyCrab_Server.serverConsole.WriteLine("Creating tables...");
            SQLiteCommand command = new SQLiteCommand(Properties.Resources.create_tables, connection);
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            SkyCrab_Server.serverConsole.WriteLine("Disconnecting with database...");
            connection.Close();
        }

    }
}
