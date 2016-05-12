using SkyCrab.Common_classes.Games;
using SkyCrabServer.Databases;

namespace SkyCrabServer.GameLogs
{
    class GameLog
    {
        public static void OnGameStart(Game game)
        {
            string log = RoomName(game) +
                    Rules(game) +
                    Players(game) +
                    "\n";
            GameTable.AddToLog(game.Id, log);
        }

        private static string RoomName(Game game)
        {
            return "ROOM:\n\t" + game.Room.Name + "\n";
        }

        private static string Rules(Game game)
        {
            return "RULES:\n" +
                    ((game.Room.Rules.maxTurnTime.value == 0) ? "" : ("\tmax turn time:\t" + game.Room.Rules.maxTurnTime.value + "\n")) +
                    (game.Room.Rules.fivesFirst.value ? "\tfives first\n" : "") +
                    (game.Room.Rules.restrictedExchange.value ? "\trestricted exchange\n" : "");
        }

        private static string Players(Game game)
        {
            string players = "PLAYERS:\n";
            for (int i = 0; i != game.Players.Length; ++i)
                players += "\tplayer #" + (i + 1) + ":\t" + game.Players[i].Player.Nick + "\n";
            return players;
        }
    }
}
