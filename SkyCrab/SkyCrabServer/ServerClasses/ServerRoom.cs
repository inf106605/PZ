using SkyCrab.Common_classes.Games;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.PresentationLayer.Messages.Game;
using SkyCrabServer.Databases;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrabServer.ServerClasses
{
    sealed class ServerRoom
    {

        public readonly Room room;

        private readonly Semaphore startGameSemaphore = new Semaphore(0, 1);
        private Task startGameTimer;


        public ServerRoom(Room room)
        {
            this.room = room;
        }

        public void CheckAllPlayersReady()
        {
            foreach (PlayerInRoom playerInRoom in room.Players)
                if (!playerInRoom.IsReady)
                    return;
            if (startGameTimer != null)
                return;
            startGameTimer = Task.Factory.StartNew(StartGameTimerTaskBody);
        }

        public void CancelStartingGame()
        {
            if (startGameTimer == null)
                return;
            startGameSemaphore.Release();
            startGameTimer.Wait();
            if (startGameSemaphore.WaitOne(0))
                return;
            startGameTimer = null;
            return;
        }

        private void StartGameTimerTaskBody()
        {
            if (startGameSemaphore.WaitOne(10000))
                return;
            Task.Factory.StartNew(StartGame);
        }

        private void StartGame()
        {
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                startGameTimer = null;
                foreach (PlayerInRoom playerInRoom in room.Players)
                    if (!playerInRoom.IsReady)
                        return;
                UInt32 gameId = GameTable.Create();
                Game game = new Game(gameId, room, false);
                foreach (PlayerInRoom playerInRoom in room.Players)
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

    }
}
