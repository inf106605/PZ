using SkyCrab.Common_classes.Games;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.PresentationLayer.Messages.Game;
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
            Task.Factory.StartNew(StartGameTimerTaskBody);
        }

        public void CancelStartingGame()
        {
            if (startGameTimer == null)
                return;
            startGameSemaphore.Release();
            startGameTimer.Wait();
            startGameSemaphore.WaitOne(0);
            startGameTimer = null;
        }

        private void StartGameTimerTaskBody()
        {
            if (startGameSemaphore.WaitOne(5000))
                return;
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                foreach (PlayerInRoom playerInRoom in room.Players)
                    playerInRoom.IsReady = false;
                StartGame();
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private void StartGame()
        {
            Game game = new Game(1234, room, false);//TODO
            foreach (PlayerInRoom playerInRoom in room.Players)
            {
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                GameStartedMsg.AsyncPost(otherServerPlayer.connection, game.Id);
            }
        }

    }
}
