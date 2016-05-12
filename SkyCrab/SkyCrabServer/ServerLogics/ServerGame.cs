using SkyCrab.Common_classes.Games;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.PresentationLayer.Messages.Game;
using SkyCrabServer.Databases;
using SkyCrabServer.GameLogs;
using System;

namespace SkyCrabServer.ServerLogics
{
    class ServerGame
    {

        private readonly ServerPlayer serverPlayer;
        private readonly ServerRoom serverRoom;
        public Game game;


        public bool InGame
        {
            get
            {
                return game != null;
            }
        }


        public ServerGame(ServerPlayer serverPlayer, ServerRoom serverRoom)
        {
            this.serverPlayer = serverPlayer;
            this.serverRoom = serverRoom;
        }

        
        public void StartGame()
        {
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                if (!serverRoom.InRoom)
                    return;
                if (!serverRoom.room.AllPlayersReady)
                    return;
                UInt32 gameId = GameTable.Create();
                game = new Game(gameId, serverRoom.room, false);
                foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
                {
                    playerInRoom.IsReady = false;
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever...");
                    GameStartedMsg.AsyncPost(otherServerPlayer.connection, game.Id);
                }
                GameLog.OnGameStart(game);
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        public void OnQuitGame()
        {
            //TODO
        }

    }
}
