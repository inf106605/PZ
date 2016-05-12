using SkyCrab.Common_classes.Games;
using SkyCrab.Common_classes.Games.Players;
using System.Data.SQLite;

namespace SkyCrabServer.Databases
{
    static class ScoreTable
    {

        private const string TABLE = "score";
        private const string WALKOVER = "walkover";
        private const string PLACE = "place";
        private const string SCORE = "score";
        private const string PLAYER_ID = "player_id";
        private const string GAME_ID = "game_id";
        

        public static void Create(Game game, PlayerInGame playerInGame)  //TODO works only with walkover now
        {
            const string QUERY = "INSERT INTO " + TABLE + " (" + WALKOVER + ", " + PLACE + ", " + SCORE + ", " + PLAYER_ID + ", " + GAME_ID + ") VALUES (@walkower, @place, @score, @playerId, @gameId)";
            SQLiteCommand command = new SQLiteCommand(QUERY, Globals.database.connection);
            command.Parameters.Add(new SQLiteParameter("@walkower", playerInGame.Walkover));
            command.Parameters.Add(new SQLiteParameter("@place", game.CurrentPlayerNumber));  //TODO only for walkover
            command.Parameters.Add(new SQLiteParameter("@score", playerInGame.Points));
            command.Parameters.Add(new SQLiteParameter("@playerId", playerInGame.Player.Id));
            command.Parameters.Add(new SQLiteParameter("@gameId", game.Id));
            command.ExecuteNonQuery();
        }

    }
}
