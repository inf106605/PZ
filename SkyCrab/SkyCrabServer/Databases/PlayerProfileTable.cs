using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace SkyCrabServer.Databases
{
    static class PlayerProfileTable
    {

        private const string TABLE = "player_profile";
        private const string LOGIN = "login";
        private const string PASSWORD_HASH = "password_hash";
        private const string NICK = "nick";
        private const string E_MAIL = "\"e-mail\"";
        private const string REGISTRATION_DATA = "register_data";
        private const string LAST_ACT_DATA = "last_act_data";
        private const string PLAYER_ID = "player_id";

        private const string SHORT_COLUMNS = NICK + ", " + REGISTRATION_DATA + ", " + LAST_ACT_DATA + ", " + PLAYER_ID;
        private const string LONG_COLUMNS = LOGIN + ", " + NICK + ", " + E_MAIL + ", " + REGISTRATION_DATA + ", " + LAST_ACT_DATA + ", " + PLAYER_ID;

        public static object _lock = new object();


        public static bool IdExists(UInt32 playerId)
        {
            const string QUERY = "SELECT 1 FROM " + TABLE + " WHERE " + PLAYER_ID + "=@playerId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
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

        public static bool LoginExists(string login)
        {
            const string QUERY = "SELECT 1 FROM " + TABLE + " WHERE " + LOGIN + "=@login";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@login", login));
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

        public static bool EMailExists(string eMail, UInt32 playerId)
        {
            const string QUERY = "SELECT 1 FROM " + TABLE + " WHERE " + E_MAIL + "=@eMail AND " + PLAYER_ID + "<>@playerId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@eMail", eMail));
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
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

        public static UInt32 Create(PlayerProfile playerProfile, string passwordHash)
        {
            const string QUERY = "INSERT INTO " + TABLE + " (" + LOGIN + ", " + PASSWORD_HASH + ", " + NICK + ", " + E_MAIL + ", " + REGISTRATION_DATA + ", " + LAST_ACT_DATA + ", " + PLAYER_ID + ") VALUES (@login, @password_hash, @nick, @eMail, @registrationData, @lastActData, @playerId)";
            UInt32 id = PlayerTable.Create();
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@login", playerProfile.Login));
            command.Parameters.Add(new SQLiteParameter("@password_hash", passwordHash));
            command.Parameters.Add(new SQLiteParameter("@nick", playerProfile.Nick));
            command.Parameters.Add(new SQLiteParameter("@eMail", playerProfile.EMail));
            command.Parameters.Add(new SQLiteParameter("@registrationData", playerProfile.Registration));
            command.Parameters.Add(new SQLiteParameter("@lastActData", playerProfile.LastActivity));
            command.Parameters.Add(new SQLiteParameter("@playerId", id));
            command.ExecuteNonQuery();
            return id;
        }

        internal static void Modify(uint playerId, PlayerProfile playerProfile, string passwordHash)
        {
            const string QUERY = "UPDATE " + TABLE + " SET " + PASSWORD_HASH + "=@passwordHash, " + NICK + "=@nick, " + E_MAIL + "=@eMail WHERE " + PLAYER_ID + "=@playerId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@passwordHash", passwordHash));
            command.Parameters.Add(new SQLiteParameter("@nick", playerProfile.Nick));
            command.Parameters.Add(new SQLiteParameter("@eMail", playerProfile.EMail));
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
            command.ExecuteNonQuery();
        }

        public static Player GetShortById(UInt32 playerId)
        {
            const string QUERY = "SELECT " + SHORT_COLUMNS + " FROM " + TABLE + " WHERE " + PLAYER_ID + "=@playerId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
            SQLiteDataReader reader = command.ExecuteReader();
            try
            {
                if (!reader.Read())
                    return null;
                Player player = GetShort(reader);
                return player;
            }
            finally
            {
                reader.Close();
            }
        }

        public static Player GetLongByLogin(string login)
        {
            const string QUERY = "SELECT " + LONG_COLUMNS + " FROM " + TABLE + " WHERE " + LOGIN + "=@login";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@login", login));
            SQLiteDataReader reader = command.ExecuteReader();
            try
            {
                if (!reader.Read())
                    return null;
                Player player = GetLong(reader);
                return player;
            }
            finally
            {
                reader.Close();
            }
        }

        public static List<Player> FindShort(string searchPhraze)
        {
            const string QUERY = "SELECT " + SHORT_COLUMNS + " FROM " + TABLE + " WHERE " + NICK + " LIKE @searchPhraze";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@searchPhraze", '%' + searchPhraze + '%')); //TODO escape special characters
            SQLiteDataReader reader = command.ExecuteReader();
            try
            {
                List<Player> players = new List<Player>();
                while (reader.Read() && players.Count != ListTranscoder<object>.MAX_COUNT)
                    players.Add(GetShort(reader));
                return players;
            }
            finally
            {
                reader.Close();
            }
        }

        public static string GetPasswordHash(UInt32 playerId)
        {
            const string QUERY = "SELECT " + PASSWORD_HASH + " FROM " + TABLE + " WHERE " + PLAYER_ID + "=@playerId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
            SQLiteDataReader reader = command.ExecuteReader();
            try
            {
                if (!reader.Read())
                    return null;
                return reader.GetString(0);
            }
            finally
            {
                reader.Close();
            }
        }

        private static Player GetShort(SQLiteDataReader reader)
        {
            UInt32 playerId = (UInt32)reader.GetInt32(3);
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.Nick = reader.GetString(0);
            playerProfile.Registration = reader.GetDateTime(1);
            if (Globals.players.ContainsKey(playerId))
                playerProfile.LastActivity = DateTime.Now;
            else
                playerProfile.LastActivity = reader.GetDateTime(2);
            Player player = new Player(playerId, playerProfile);
            return player;
        }

        private static Player GetLong(SQLiteDataReader reader)
        {
            UInt32 playerId = (UInt32)reader.GetInt32(5);
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.Login = reader.GetString(0);
            playerProfile.Nick = reader.GetString(1);
            playerProfile.EMail = reader.GetString(2);
            playerProfile.Registration = reader.GetDateTime(3);
            if (Globals.players.ContainsKey(playerId))
                playerProfile.LastActivity = DateTime.Now;
            else
                playerProfile.LastActivity = reader.GetDateTime(4);
            Player player = new Player(playerId, playerProfile);
            return player;
        }

        public static void UpdateLastActivity(UInt32 playerId)
        {
            const string QUERY = "UPDATE " + TABLE + " SET " + LAST_ACT_DATA + "=@lastActivity WHERE " + PLAYER_ID + "=@playerId";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@playerId", playerId));
            command.Parameters.Add(new SQLiteParameter("@lastActivity", DateTime.Now));
            command.ExecuteNonQuery();
        }

    }
}
