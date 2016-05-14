using SkyCrab.Common_classes.Games;
using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Players;
using SkyCrab.Common_classes.Games.Pouches;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
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
                    otherServerPlayer.serverGame.game = game;
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
            DrawTilesForAllPlayers();
            SendNextTurn();
        }

        private void ChooseFirstPlayer()
        {
            game.CurrentPlayerNumber = (uint)rand.Next(game.Players.Length);
            GameLog.OnChoosePlayer(game);
        }

        private void DrawTilesForAllPlayers()
        {
            for (uint i = game.CurrentPlayerNumber; i != game.Players.Length; ++i)
                FillRack(i);
            for (uint i = 0; i != game.CurrentPlayerNumber; ++i)
                FillRack(i);
        }

        private List<Letter> FillRack(uint playerNumber)
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
            GameLog.OnDrawLetters(game, playerNumber, letters);
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
            return letters;
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

        public void ExchangeTiles(Int16 id, List<LetterWithNumber> letters)
        {
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                if (!InGame)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_IN_GAME3);
                    return;
                }
                if (game.CurrentPlayer.Player.Id != serverPlayer.player.Id)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_YOUR_TURN2);
                    return;
                }
                if (game.Puoches[0].Count >= letters.Count)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE3);
                    return;
                }
                if (serverRoom.room.Rules.restrictedExchange.value && game.Puoches[0].Count <= Rack.IntendedTilesCount)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE3);
                    return;
                }
                if (!HasTiles(letters))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE3);
                    return;
                }
                OkMsg.AsyncPost(id, serverPlayer.connection);
                RemoveTiles(letters);
                List<Letter> newLetters = FillRack(game.CurrentPlayerNumber);
                GameLog.OnExchange(game, letters, newLetters);
                SwitchToNextPlayer();
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private bool HasTiles(List<LetterWithNumber> letters)
        {
            //TODO check number
            PlayerInGame playerInGame = game.CurrentPlayer;
            List<Letter> allTilesLetters = new List<Letter>();
            foreach (TileOnRack tileOnRack in playerInGame.Rack.Tiles)
                allTilesLetters.Add(tileOnRack.Tile.Letter);
            foreach (LetterWithNumber letterWithNumber in letters)
                if (!allTilesLetters.Remove(letterWithNumber.letter))
                    return false;
            return true;
        }

        private void RemoveTiles(List<LetterWithNumber> letters)
        {
            //TODO take number into account
            Rack rack = game.CurrentPlayer.Rack;
            foreach (LetterWithNumber letterWithNumber in letters)
                foreach (TileOnRack tileOnRack in rack.Tiles)
                    if (tileOnRack.Tile.Letter == letterWithNumber.letter)
                        rack.TakeOff(tileOnRack);
            foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
            {
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                LostLetters lostTiles = new LostLetters();
                lostTiles.playerId = serverPlayer.player.Id;
                lostTiles.letters = letters;
                LossTilesMsg.AsyncPost(otherServerPlayer.connection, lostTiles);
            }
        }

        public void Pass(Int16 id)
        {
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                if (!InGame)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_IN_GAME4);
                    return;
                }
                if (game.CurrentPlayer.Player.Id != serverPlayer.player.Id)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_YOUR_TURN3);
                    return;
                }
                OkMsg.AsyncPost(id, serverPlayer.connection);
                GameLog.OnPass(game);
                SwitchToNextPlayer();
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private static void OnEndGame(Game game)
        {
            GameTable.Finish(game.Id);
        }

        private void SwitchToNextPlayer()
        {
            game.SwitchToNextPlayer();
            SendNextTurn();
        }

        private void SendNextTurn()
        {
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

    }
}
