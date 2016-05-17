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
using SkyCrab.Common_classes.Games.Boards;

namespace SkyCrabServer.ServerLogics
{
    class ServerGame
    {

        private enum Orientation
        {
            HORIZONTAL,
            VERTICAL,
            NONE
        }


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

        public ServerGame() : base()
        {
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
                FillRack(i, true);
            for (uint i = 0; i != game.CurrentPlayerNumber; ++i)
                FillRack(i, true);
        }

        private List<Letter> FillRack(uint playerNumber, bool addToLog)
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
            if (addToLog)
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

        public void PlaceTiles(Int16 id, TilesToPlace tilesToPlace)
        {
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                if (!InGame)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_IN_GAME2);
                    return;
                }
                if (game.CurrentPlayer.Player.Id != serverPlayer.player.Id)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_YOUR_TURN);
                    return;
                }
                if (tilesToPlace.lettersFromRack.Count == 0 || tilesToPlace.lettersFromRack.Count != tilesToPlace.tilesToPlace.Count)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE2);
                    return;
                }
                if (serverRoom.room.Rules.fivesFirst.value && game.Board.IsEmpty && tilesToPlace.lettersFromRack.Count < (5 - game.FullRoundNumber))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE2);
                    return;
                }
                if (!AreTheSameLetters(tilesToPlace))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE2);
                    return;
                }
                if (!HasTiles(tilesToPlace.lettersFromRack))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE2);
                    return;
                }
                if (GetTilesOrientation(tilesToPlace.tilesToPlace) == Orientation.NONE)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE2);
                    return;
                }
                try
                {
                    WordOnBoard wrongWordOnBoard;
                    if (!IsMoveCorrect(tilesToPlace.tilesToPlace, out wrongWordOnBoard))
                    {
                        ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE2); //TODO send incorrect words
                        GameLog.OnWrongMove(game, wrongWordOnBoard);
                        SwitchToNextPlayer();
                        return;
                    }
                }
                catch (Exception)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE2);
                    return;
                }
                OkMsg.AsyncPost(id, serverPlayer.connection);
                RemoveTiles(tilesToPlace.lettersFromRack);
                WordOnBoard wordOnBoard;
                uint points;
                DoPlaceTiles(tilesToPlace, out wordOnBoard, out points);
                GameLog.OnPlaceTiles(game, wordOnBoard, points);
                List<Letter> newLetters = FillRack(game.CurrentPlayerNumber, true);
                SwitchToNextPlayer();
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private Orientation GetTilesOrientation(List<TileOnBoard> tilesToPlace)
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            foreach (TileOnBoard tileOnBoard in tilesToPlace)
            {
                minX = Math.Min(minX, tileOnBoard.position.x);
                maxX = Math.Max(maxX, tileOnBoard.position.x);
                minY = Math.Min(minY, tileOnBoard.position.y);
                maxY = Math.Max(maxY, tileOnBoard.position.y);
            }
            if (minX != maxX && minY != maxY)
                return Orientation.NONE;
            else if (minX == maxX)
                return Orientation.VERTICAL;
            else
                return Orientation.HORIZONTAL;
        }

        private bool AreTheSameLetters(TilesToPlace tilesToPlace)
        {
            tilesToPlace.lettersFromRack.Sort((letter1, letter2) =>
                    {
                        if (letter1.letter.character == letter2.letter.character)
                            return 0;
                        else if (letter1.letter.character < letter2.letter.character)
                            return -1;
                        else
                            return 1;
                    });
            tilesToPlace.tilesToPlace.Sort((tile1, tile2) =>
                    {
                        if (tile1.tile.Letter.character == tile2.tile.Letter.character)
                            return 0;
                        else if (tile1.tile.Letter.character < tile2.tile.Letter.character)
                            return -1;
                        else
                            return 1;
                    });
            for (int i = 0; i != tilesToPlace.lettersFromRack.Count; ++i)
                if (tilesToPlace.lettersFromRack[i].letter.character != tilesToPlace.tilesToPlace[i].tile.Letter.character)
                    return false;
            return true;
        }

        private bool IsMoveCorrect(List<TileOnBoard> tilesToPlace, out WordOnBoard wordOnBoard)
        {
            Board boardCopy = (Board)game.Board.Clone();
            foreach (TileOnBoard tileOnBoard in tilesToPlace)
                boardCopy.PutTile(tileOnBoard);
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            foreach (TileOnBoard tileOnBoard in tilesToPlace)
            {
                minX = Math.Min(minX, tileOnBoard.position.x);
                maxX = Math.Max(maxX, tileOnBoard.position.x);
                minY = Math.Min(minY, tileOnBoard.position.y);
                maxY = Math.Max(maxY, tileOnBoard.position.y);
            }
            ++maxX;
            ++maxY;
            for (int i = minX; i != maxX; ++i)
                for (int j = minY; j != maxY; ++j)
                    if (boardCopy.GetTile(new PositionOnBoard(i, j)) == null)
                    {
                        wordOnBoard = null;
                        return false;
                    }
            bool horizontal = GetTilesOrientation(tilesToPlace) == Orientation.HORIZONTAL;
            if (!CheckWord(boardCopy, tilesToPlace[0].position, horizontal, out wordOnBoard))
                return false;
            horizontal = !horizontal;
            WordOnBoard additionalWordOnBoard;
            for (int i = minX; i != maxX; ++i)
                for (int j = minY; j != maxY; ++j)
                    if (!CheckWord(boardCopy, new PositionOnBoard(i, j), horizontal, out additionalWordOnBoard))
                        return false;
            return true;
        }

        private static bool CheckWord(Board board, PositionOnBoard position, bool horizontal, out WordOnBoard wordOnBoard)
        {
            wordOnBoard = GetWord(board, position, horizontal);
            if (wordOnBoard.word.Length == 1)
                return true;
            return Globals.dictionary.IsWordWalid(wordOnBoard.word);
        }

        private static WordOnBoard GetWord(Board board, PositionOnBoard position, bool horizontal)
        {
            WordOnBoard wordOnBoard = new WordOnBoard();
            wordOnBoard.position = position;
            wordOnBoard.horizonatal = horizontal;
            wordOnBoard.word = "";
            List<TileOnBoard> tiles = GetTiles(board, position, horizontal);
            foreach (TileOnBoard tileOnBoard in tiles)
                wordOnBoard.word += tileOnBoard.tile.Letter.character;
            return wordOnBoard;
        }

        private void DoPlaceTiles(TilesToPlace tilesToPlace, out WordOnBoard wordOnBoard, out uint points)
        {
            foreach (TileOnBoard tileOnBoard in tilesToPlace.tilesToPlace)
                game.Board.PutTile(tileOnBoard);
            tilesToPlace.playerId = serverPlayer.player.Id;
            foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
            {
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                PlaceTilesMsg.AsyncPost(otherServerPlayer.connection, tilesToPlace);
            }
            bool horizontal = GetTilesOrientation(tilesToPlace.tilesToPlace) == Orientation.HORIZONTAL;
            wordOnBoard = GetWord(game.Board, tilesToPlace.tilesToPlace[0].position, horizontal);
            points = CountPoints(wordOnBoard, tilesToPlace.tilesToPlace, horizontal);
            horizontal = !horizontal;
            foreach (TileOnBoard tileOnBoard in tilesToPlace.tilesToPlace)
            {
                WordOnBoard additionalWordOnBoard = GetWord(game.Board, tileOnBoard.position, horizontal);
                points += CountPoints(additionalWordOnBoard, tilesToPlace.tilesToPlace, horizontal);
            }
            PlayerPoints playerPoints = new PlayerPoints();
            playerPoints.playerId = serverPlayer.player.Id;
            playerPoints.points = (int)points;
            foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
            {
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                GainPointsMsg.AsyncPost(otherServerPlayer.connection, playerPoints);
            }
        }

        private uint CountPoints(WordOnBoard wordOnBoard, List<TileOnBoard> tilesToPlace, bool horizontal)
        {
            List<TileOnBoard> tiles = GetTiles(game.Board, wordOnBoard.position, horizontal);
            uint points = 0;
            uint wordMultiplier = 1;
            foreach (TileOnBoard tileOnBoard in tiles)
            {
                uint letterPoints = tileOnBoard.tile.Letter.points;
                uint letterMultiplier = 1;
                SquareType squareType = game.Board.GetSquareType(tileOnBoard.position);
                if (squareType != SquareType.START && squareType != SquareType.NORMAL)
                    foreach (TileOnBoard tileToPlace in tilesToPlace)
                        if (tileToPlace.position == tileOnBoard.position)
                        {
                            switch (squareType)
                            {
                                case SquareType.LETTER2:
                                    letterMultiplier = 2;
                                    break;
                                case SquareType.LETTER3:
                                    letterMultiplier = 3;
                                    break;
                                case SquareType.WORD2:
                                    wordMultiplier *= 2;
                                    break;
                                case SquareType.WORD3:
                                    wordMultiplier *= 3;
                                    break;
                            }
                        }
                points += letterPoints * letterMultiplier;
            }
            points *= wordMultiplier;
            return points;
        }

        private static List<TileOnBoard> GetTiles(Board board, PositionOnBoard position, bool horizontal)
        {
            if (horizontal)
                return GetHorizontalTiles(board, position);
            else
                return GetVerticalTiles(board, position);
        }

        private static List<TileOnBoard> GetHorizontalTiles(Board board, PositionOnBoard position)
        {
            try
            {
                do
                    --position.x;
                while (board.GetTile(position) != null);
            }
            catch (NoSuchSquareOnBoardException)
            {
            }
            List<TileOnBoard> tilesOnBoard = new List<TileOnBoard>();
            try
            {
                while (true)
                {
                    ++position.x;
                    TileOnBoard tile = board.GetTileOnBoard(position);
                    if (tile == null)
                        break;
                    else
                        tilesOnBoard.Add(tile);
                }
            }
            catch (NoSuchSquareOnBoardException)
            {
            }
            return tilesOnBoard;
        }

        private static List<TileOnBoard> GetVerticalTiles(Board board, PositionOnBoard position)
        {
            try
            {
                do
                    --position.y;
                while (board.GetTile(position) != null);
            }
            catch (NoSuchSquareOnBoardException)
            {
            }
            List<TileOnBoard> tilesOnBoard = new List<TileOnBoard>();
            try
            {
                while (true)
                {
                    ++position.y;
                    TileOnBoard tile = board.GetTileOnBoard(position);
                    if (tile == null)
                        break;
                    else
                        tilesOnBoard.Add(tile);
                }
            }
            catch (NoSuchSquareOnBoardException)
            {
            }
            return tilesOnBoard;
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
                if (game.Puoches[0].Count < letters.Count)
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
                List<Letter> newLetters = FillRack(game.CurrentPlayerNumber, false);
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
                    {
                        rack.TakeOff(tileOnRack);
                        game.Puoches[0].InsertTile(tileOnRack.Tile.Letter);
                        break;
                    }
            List<LetterWithNumber> blanks = new List<LetterWithNumber>();
            foreach (LetterWithNumber letterWithNumber in letters)
            {
                LetterWithNumber blank = new LetterWithNumber();
                blank.number = letterWithNumber.number;
                blank.letter = LetterSet.BLANK;
                blanks.Add(blank);
            }
            foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
            {
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                LostLetters lostTiles = new LostLetters();
                lostTiles.playerId = serverPlayer.player.Id;
                if (serverPlayer.player.Id == otherServerPlayer.player.Id)
                    lostTiles.letters = letters;
                else
                    lostTiles.letters = blanks;
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
