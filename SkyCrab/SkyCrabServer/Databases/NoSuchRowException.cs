using System.Data.SQLite;

namespace SkyCrabServer.Databases
{
    sealed class NoSuchRowException : SkyCrabServerException
    {

        public NoSuchRowException(SQLiteCommand command) :
            base("Query: \"" + GetQuery(command) + "\" has no result!")
        {
        }

        private static string GetQuery(SQLiteCommand command)
        {
            string query = command.CommandText;
            foreach (SQLiteParameter param in command.Parameters)
                query = query.Replace(param.ParameterName, param.Value.ToString());
            return query;
        }

}
}
