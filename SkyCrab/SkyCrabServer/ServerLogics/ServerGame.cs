using SkyCrab.Common_classes.Games;
using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Players;
using SkyCrab.Common_classes.Games.Pouches;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.PresentationLayer.Messages.Game;
using SkyCrabServer.Databases;
using SkyCrabServer.GameLogs;
using System;
using System.Collections.Generic;

namespace SkyCrabServer.ServerLogics
{
    class ServerGame
    {

        private static readonly Random rand = new Random();

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
                InitializeGame();
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private void InitializeGame()
        {
            ChooseFirstPlayer();
            for (uint i = 0; i != game.Players.Length; ++i)
                FillRack(i);
            GameLog.OnChoosePlayer(game);
            //TODO
        }

        private void ChooseFirstPlayer()
        {
            game.CurrentPlayerNumber = (uint)rand.Next(game.Players.Length);
            foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
            {
                playerInRoom.IsReady = false;
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                NextTurnMsg.AsyncPost(otherServerPlayer.connection, game.CurrentPlayer.Player.Id);
            }
        }

        private void FillRack(uint playerNumber)
        {
            PlayerInGame playerInGame = game.Players[playerNumber];
            int tilesToDraw = Rack.IntendedTilesCount - playerInGame.Rack.Tiles.Count;
            List<Letter> letters = new List<Letter>();
            List<Letter> blanks = new List<Letter>();
            for (int i = 0; i != tilesToDraw; ++i)
            {
                Tile drawedTile = game.Puoches[0].DrawRandowmTile();
                playerInGame.Rack.PutTile(drawedTile);
                letters.Add(drawedTile.Letter);
                blanks.Add(LetterSet.BLANK);
            }
            foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
            {
                playerInRoom.IsReady = false;
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                DrawedLetters drawedLetters = new DrawedLetters();
                drawedLetters.playerId = playerInGame.Player.Id;
                if (playerInRoom.Player.Id == playerInGame.Player.Id)
                    drawedLetters.letters = letters;
                else
                    drawedLetters.letters = blanks;
                NewTilesMsg.AsyncPost(otherServerPlayer.connection, drawedLetters);
            }
            GameLog.OnDrawLetters(game, playerNumber, letters);
        }

        public void OnQuitGame()
        {
            if (!InGame)
                return;
            PlayerInGame playerInGame = game.GetPlayer(serverPlayer.player.Id);
            if (!game.IsFinished)
                playerInGame.Walkover = true;
            ScoreTable.Create(game, playerInGame);
            if (game.ActivePlayersNumber == 0)
                OnEndGame(game);
            game = null;
        }

        private static void OnEndGame(Game game)
        {
            GameTable.Finish(game.Id);
        }

    }
}
