using System.Data.SQLite;

namespace SkyCrabServer.Databases
{
    sealed class DatabaseVersionIsIncorrectException : SkyCrabServerException
    {

        public readonly string currentDatabaseVersion;


        public DatabaseVersionIsIncorrectException() :
            base()
        {
            this.currentDatabaseVersion = null;
        }

        public DatabaseVersionIsIncorrectException(string currentDatabaseVersion) :
            base()
        {
            this.currentDatabaseVersion = currentDatabaseVersion;
        }

    }

    static class DatabaseInfoTable
    {

        private const string TABLE = "database_info";
        private const string LOCK = "lock";
        private const string VERSION = "ver";


        public static void Create(SQLiteConnection connection, string version)
        {
            const string QUERY = "INSERT INTO " + TABLE + " (" + LOCK + ", " + VERSION + ") VALUES ('X', @version)";
            SQLiteCommand command = new SQLiteCommand(QUERY, connection);
            command.Parameters.Add(new SQLiteParameter("@version", version));
            command.ExecuteNonQuery();
        }

        public static void ValidateVersion(SQLiteConnection connection, string expectedVersion)
        {
            const string QUERY = "SELECT " + VERSION + " FROM " + TABLE;
            SQLiteCommand command = new SQLiteCommand(QUERY, connection);
            SQLiteDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new DatabaseVersionIsIncorrectException();
                string currentVersion = reader.GetString(0);
                if (expectedVersion != currentVersion)
                    throw new DatabaseVersionIsIncorrectException(currentVersion);
            }
            catch (SQLiteException)
            {
                throw new DatabaseVersionIsIncorrectException();
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

    }
}
