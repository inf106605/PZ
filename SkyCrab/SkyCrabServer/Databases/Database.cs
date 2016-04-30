using System;
using System.Data.SQLite;
using System.IO;

namespace SkyCrabServer.Databases
{
    static class Database
    {

        private const string FILE_NAME = "Database.sqlite";

        private static SQLiteConnection connection;

        
        private static bool FileExists
        {
            get { return File.Exists(FILE_NAME); }
        }


        public static void Connect()
        {
            bool fileExists = false; //TODO FileExists;
            if (!fileExists)
                CreateFile();
            CreateConnection();
            if (!fileExists)
                CreateTables();
            Console.WriteLine();
        }

        private static void CreateFile()
        {
            Console.WriteLine("Creating database file...");
            SQLiteConnection.CreateFile(FILE_NAME);
        }

        private static void CreateConnection()
        {
            Console.WriteLine("Connecting with database...");
            connection = new SQLiteConnection("Data Source=" + FILE_NAME + "; Version=3;");
            connection.Open();
        }

        private static void CreateTables()
        {
            Console.WriteLine("Creating tables...");
            SQLiteCommand command = new SQLiteCommand(Properties.Resources.create_tables, connection);
            command.ExecuteNonQuery();
        }

        public static void Disconnect()
        {
            Console.WriteLine("Disconnecting with database...\n");
            connection.Close();
        }

    }
}
