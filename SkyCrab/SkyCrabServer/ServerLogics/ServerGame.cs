using SkyCrab.Common_classes.Games;
using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Players;
using SkyCrab.Common_classes.Games.Pouches;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
using SkyCrabServer.Databases;
using SkyCrabServer.GameLogs;
using System;
using System.Collections.Generic;
using SkyCrab.Common_classes.Games.Boards;
using System.Threading.Tasks;
using System.Threading;
using SkyCrab.Connection.PresentationLayer.Messages.Game.Informations;
using SkyCrab.Connection.PresentationLayer.Messages.Game.Commands;

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
        private Task turnTimeoutTimer;
        private Semaphore turnTimeoutSemaphore = new Semaphore(0, 1);


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

        private void InitializeGame()
        {
            ChooseFirstPlayer();
            DrawTilesForAllPlayers();
            SendNextTurn();
            StartTurnTimerWithAdequatePlayer();
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
            uint tilesToDraw = (uint)(Rack.IntendedTilesCount - playerInGame.Rack.Tiles.Count);
            if (tilesToDraw > game.Puoches[0].Count)
            {
                tilesToDraw = game.Puoches[0].Count;
                if (tilesToDraw == 0 && playerInGame.Rack.Tiles.Count == 0)
                {
                    EndGame();
                    return new List<Letter>();
                }
            }
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
                if ((game.Board.IsEmpty ? tilesToPlace.lettersFromRack.Count < 2 : tilesToPlace.lettersFromRack.Count == 0) || tilesToPlace.lettersFromRack.Count != tilesToPlace.tilesToPlace.Count)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.TOO_LESS_TILES);
                    return;
                }
                if (serverRoom.room.Rules.fivesFirst.value && game.Board.IsEmpty && tilesToPlace.lettersFromRack.Count < (5 - game.FullRoundNumber))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.FIVES_FIRST_VIOLATION);
                    return;
                }
                if (!HasTiles(tilesToPlace.lettersFromRack))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.LETTERS_NOT_FROM_RACK);
                    return;
                }
                if (!AreTheSameLetters(tilesToPlace))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.LETTERS_NOT_MATH);
                    return;
                }
                if (GetTilesOrientation(tilesToPlace.tilesToPlace) == Orientation.NONE)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.WORD_NOT_IN_LINE);
                    return;
                }
                if (!IsInStartingSquare(tilesToPlace.tilesToPlace))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_IN_STARTING_SQUARE);
                    return;
                }
                if (!IsAdjancent(tilesToPlace.tilesToPlace))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_ADJANCENT);
                    return;
                }
                WordOnBoard wordOnBoard;
                try
                {
                    if (!IsMoveCorrect(id, tilesToPlace.tilesToPlace, out wordOnBoard))
                    {
                        foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
                        {
                            ServerPlayer otherServerPlayer; //Schrödinger Variable
                            Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                            if (otherServerPlayer == null)  //WTF!?
                                throw new Exception("Whatever...");
                            PlayerFailedToPlaceTilesMsg.AsyncPost(otherServerPlayer.connection, wordOnBoard);
                        }
                        GameLog.OnWrongMove(game, wordOnBoard);
                        SwitchToNextPlayer(false);
                        return;
                    }
                }
                catch (Exception)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORRECT_MOVE2);
                    return;
                }
                OkMsg.AsyncPost(id, serverPlayer.connection);
                foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
                {
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever...");
                    PlayerPlacedTilesMsg.AsyncPost(otherServerPlayer.connection, wordOnBoard);
                }
                RemoveTiles(tilesToPlace.lettersFromRack, false);
                Int16 points;
                DoPlaceTiles(tilesToPlace, wordOnBoard, out points);
                GameLog.OnPlaceTiles(game, wordOnBoard, points);
                FillRack(game.CurrentPlayerNumber, true);
                if (serverPlayer.serverGame.InGame)
                    SwitchToNextPlayer(false);
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
                        if (tile1.tile.Blank != tile2.tile.Blank)
                            return tile1.tile.Blank ? -1 : 1;
                        if (tile1.tile.Letter.character == tile2.tile.Letter.character)
                            return 0;
                        else if (tile1.tile.Letter.character < tile2.tile.Letter.character)
                            return -1;
                        else
                            return 1;
                    });
            for (int i = 0; i != tilesToPlace.lettersFromRack.Count; ++i)
                if ((tilesToPlace.lettersFromRack[i].letter.character != tilesToPlace.tilesToPlace[i].tile.Letter.character) && (tilesToPlace.lettersFromRack[i].letter.character != ' ' || !tilesToPlace.tilesToPlace[i].tile.Blank))
                    return false;
            return true;
        }

        private bool IsInStartingSquare(List<TileOnBoard> tilesToPlace)
        {
            if (game.Board.IsEmpty)
            {
                foreach (TileOnBoard tileOnBoard in tilesToPlace)
                    if (game.Board.GetSquareType(tileOnBoard.position) == SquareType.START)
                        return true;
                return false;
            }
            return true;
        }

        private bool IsAdjancent(List<TileOnBoard> tilesToPlace)
        {
            if (!game.Board.IsEmpty)
            {
                foreach (TileOnBoard tileOnBoard in tilesToPlace)
                {
                    PositionOnBoard position = new PositionOnBoard();
                    position.x = tileOnBoard.position.x - 1;
                    position.y = tileOnBoard.position.y;
                    try
                    {
                        if (game.Board.GetTile(position) != null)
                            return true;
                    }
                    catch (NoSuchSquareOnBoardException) { }
                    ++position.x;
                    --position.y;
                    try
                    {
                        if (game.Board.GetTile(position) != null)
                            return true;
                    }
                    catch (NoSuchSquareOnBoardException) { }
                    ++position.x;
                    ++position.y;
                    try
                    {
                        if (game.Board.GetTile(position) != null)
                            return true;
                    }
                    catch (NoSuchSquareOnBoardException) { }
                    --position.x;
                    ++position.y;
                    try
                    {
                        if (game.Board.GetTile(position) != null)
                            return true;
                    }
                    catch (NoSuchSquareOnBoardException) { }
                }
                return false;
            }
            return true;
        }

        private bool IsMoveCorrect(Int16 id, List<TileOnBoard> tilesToPlace, out WordOnBoard wordOnBoard)
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
                        ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_CONTINUOUS);
                        return false;
                    }
            bool horizontal = GetTilesOrientation(tilesToPlace) == Orientation.HORIZONTAL; //TODO FIXME wrong orientation when player put only one tile
            if (!CheckWord(boardCopy, tilesToPlace[0].position, horizontal, out wordOnBoard))
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORECT_WORD); //TODO send incorrect words
                return false;
            }
            horizontal = !horizontal;
            WordOnBoard additionalWordOnBoard;
            for (int i = minX; i != maxX; ++i)
                for (int j = minY; j != maxY; ++j)
                    if (!CheckWord(boardCopy, new PositionOnBoard(i, j), horizontal, out additionalWordOnBoard))
                    {
                        ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INCORECT_WORD); //TODO send incorrect words //TODO check all if wrong one is found? (to send all incorrect)
                        return false;
                    }
            return true;
        }

        private static bool CheckWord(Board board, PositionOnBoard position, bool horizontal, out WordOnBoard wordOnBoard)
        {
            wordOnBoard = GetWord(board, position, horizontal);
            string word = wordOnBoard.word.Replace("[", "").Replace("]", "");
            if (word.Length == 1)
                return true;
            return Globals.dictionary.IsWordWalid(word);
        }

        private static WordOnBoard GetWord(Board board, PositionOnBoard position, bool horizontal)
        {
            WordOnBoard wordOnBoard = new WordOnBoard();
            wordOnBoard.position = position;
            wordOnBoard.horizonatal = horizontal;
            wordOnBoard.word = "";
            List<TileOnBoard> tiles = GetTiles(board, position, horizontal);
            foreach (TileOnBoard tileOnBoard in tiles)
                if (tileOnBoard.tile.Blank)
                    wordOnBoard.word += "[" + tileOnBoard.tile.Letter.character + "]";
                else
                    wordOnBoard.word += tileOnBoard.tile.Letter.character;
            return wordOnBoard;
        }

        private void DoPlaceTiles(TilesToPlace tilesToPlace, WordOnBoard wordOnBoard, out Int16 points)
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
            points = CountPoints(wordOnBoard, tilesToPlace.tilesToPlace, horizontal, true);
            horizontal = !horizontal;
            foreach (TileOnBoard tileOnBoard in tilesToPlace.tilesToPlace)
            {
                WordOnBoard additionalWordOnBoard = GetWord(game.Board, tileOnBoard.position, horizontal);
                points += CountPoints(additionalWordOnBoard, tilesToPlace.tilesToPlace, horizontal, false);
            }
            PostGainPoints(serverPlayer.player.Id, points);
        }

        private void PostGainPoints(UInt32 playerId, Int16 gainedPoints)
        {
            PlayerPoints playerPoints = new PlayerPoints();
            playerPoints.playerId = serverPlayer.player.Id;

            playerPoints.pointsDifference = gainedPoints;

            foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
            {
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                PointsChangedMsg.AsyncPost(otherServerPlayer.connection, playerPoints);
            }
        }

        private Int16 CountPoints(WordOnBoard wordOnBoard, List<TileOnBoard> tilesToPlace, bool horizontal, bool mainWord)
        {
            if (wordOnBoard.word.Length < 2)
                return 0;
            List<TileOnBoard> tiles = GetTiles(game.Board, wordOnBoard.position, horizontal);
            Int16 points = 0;
            Int16 wordMultiplier = 1;
            foreach (TileOnBoard tileOnBoard in tiles)
            {
                uint letterPoints = PolishLetterSet.GetLetter(tileOnBoard.tile.Letter.character).points;
                uint letterMultiplier = 1;
                SquareType squareType = game.Board.GetSquareType(tileOnBoard.position);
                if (squareType != SquareType.NORMAL)
                {
                    foreach (TileOnBoard tileToPlace in tilesToPlace)
                    {
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
                                case SquareType.START:
                                case SquareType.WORD2:
                                    wordMultiplier *= 2;
                                    break;
                                case SquareType.WORD3:
                                    wordMultiplier *= 3;
                                    break;
                            }
                        }
                    }
                }
                if (!tileOnBoard.tile.Blank)
                    points += (Int16)(letterPoints * letterMultiplier);
            }
            points *= wordMultiplier;
            if (mainWord && tilesToPlace.Count == Rack.IntendedTilesCount)
                points += 50;
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
                if (letters.Count == 0)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.TOO_LESS_TILES2);
                    return;
                }
                if (game.Puoches[0].Count < letters.Count)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.TOO_LESS_POUCH_LETTERS);
                    return;
                }
                if (serverRoom.room.Rules.restrictedExchange.value && game.Puoches[0].Count <= Rack.IntendedTilesCount)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.RESTR_EXCH_VIOLATION);
                    return;
                }
                if (!HasTiles(letters))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.LETTERS_NOT_FROM_RACK2);
                    return;
                }
                OkMsg.AsyncPost(id, serverPlayer.connection);
                foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
                {
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever...");
                    PlayerExchangedMsg.AsyncPost(otherServerPlayer.connection, (byte)letters.Count);
                }
                RemoveTiles(letters, true);
                List<Letter> newLetters = FillRack(game.CurrentPlayerNumber, false);
                InsertTilesToPouch(letters);
                GameLog.OnExchange(game, letters, newLetters);
                SwitchToNextPlayer(false);
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private bool HasTiles(List<LetterWithNumber> letters)
        {
            letters.Sort((letter1, letter2) =>
                    {
                        if (letter1.number == letter2.number)
                            return 0;
                        else
                            return letter1.number < letter2.number ? 1 : -1;
                    });
            PlayerInGame playerInGame = game.CurrentPlayer;
            List<Letter> allTilesLetters = new List<Letter>();
            foreach (TileOnRack tileOnRack in playerInGame.Rack.Tiles)
                allTilesLetters.Add(tileOnRack.Tile.Letter);
            for (uint i = 0; i != letters.Count; ++i)
            {
                LetterWithNumber letterWithNumber = letters[(int)i];
                if (letterWithNumber.number == 0)
                    continue;
                if (allTilesLetters.Count < letterWithNumber.number)
                    return false;
                if (allTilesLetters[letterWithNumber.number - 1] != letterWithNumber.letter)
                    return false;
                allTilesLetters.RemoveAt((int)i - 1);
            }
            foreach (LetterWithNumber letterWithNumber in letters)
            {
                if (letterWithNumber.number != 0)
                    continue;
                if (!allTilesLetters.Remove(letterWithNumber.letter))
                    return false;
            }
            return true;
        }

        private void RemoveAllTiles(bool backToPouch)
        {
            List<LetterWithNumber> letters = new List<LetterWithNumber>();
            byte i = 0;
            foreach (TileOnRack tileOnRack in game.CurrentPlayer.Rack.Tiles)
            {
                LetterWithNumber letterWithNumber = new LetterWithNumber();
                letterWithNumber.letter = tileOnRack.Tile.Letter;
                letterWithNumber.number = ++i;
                letters.Add(letterWithNumber);
            }
            RemoveTiles(letters, backToPouch);
        }

        private void RemoveTiles(List<LetterWithNumber> letters, bool backToPouch)
        {
            letters.Sort((letter1, letter2) =>
                    {
                        if (letter1.number == letter2.number)
                            return 0;
                        else
                            return letter1.number < letter2.number ? 1 : -1;
                    });
            Rack rack = game.CurrentPlayer.Rack;
            foreach (LetterWithNumber letterWithNumber in letters)
                if (letterWithNumber.number != 0)
                {
                    byte number = letterWithNumber.number;
                    foreach (TileOnRack tileOnRack in rack.Tiles)
                        if (--number == 0)
                        {
                            rack.TakeOff(tileOnRack);
                            break;
                        }
                }
            foreach (LetterWithNumber letterWithNumber in letters)
                if (letterWithNumber.number == 0)
                    foreach (TileOnRack tileOnRack in rack.Tiles)
                        if (tileOnRack.Tile.Letter == letterWithNumber.letter)
                        {
                            rack.TakeOff(tileOnRack);
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
                lostTiles.backToPouch = backToPouch;
                if (serverPlayer.player.Id == otherServerPlayer.player.Id)
                    lostTiles.letters = letters;
                else
                    lostTiles.letters = blanks;
                LossTilesMsg.AsyncPost(otherServerPlayer.connection, lostTiles);
            }
        }

        private void InsertTilesToPouch(List<LetterWithNumber> letters)
        {
            foreach (LetterWithNumber letterWithNumber in letters)
                game.Puoches[0].InsertTile(letterWithNumber.letter);
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
                foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
                {
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever...");
                    PlayerPassedMsg.AsyncPost(otherServerPlayer.connection);
                }
                SwitchToNextPlayer(true);
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
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
            {
                OnEndGame();
            }
            else if (game.CurrentPlayer.Player.Id == serverPlayer.player.Id)
            {
                if (!game.IsFinished)
                    RemoveAllTiles(true);
                SwitchToNextPlayer(true);
            }
            game = null;
        }

        private void OnEndGame()
        {
            game.Room.InGame = false;
        }

        private void SwitchToNextPlayer(bool pass)
        {
            StopTurnTimer();
            game.SwitchToNextPlayer(pass);
            if (IsGameEndedByPassing())
            {
                if (!game.IsFinished)
                    EndGame();
            }
            else
            {
                SendNextTurn();
                StartTurnTimerWithAdequatePlayer();
            }
        }

        private bool IsGameEndedByPassing()
        {
            foreach (PlayerInGame playerInGame in game.Players)
                if (!playerInGame.Walkover && playerInGame.PassCount < 2)
                    return false;
            return true;
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

        private void EndGame()
        {
            foreach (PlayerInGame playerInGame in game.Players)
            {
                Int16 pointDiff = 0;
                if (playerInGame.Rack.Tiles.Count == 0)
                {
                    foreach (PlayerInGame otherPlayerInGame in game.Players)
                        pointDiff += (Int16)otherPlayerInGame.Rack.PointSum;
                }
                else
                {
                    pointDiff = (Int16)(-(Int16)playerInGame.Rack.PointSum);
                }
                if (pointDiff + playerInGame.Points < 0)
                    pointDiff = (Int16)(-(Int16)playerInGame.Points);
                playerInGame.GainPoints(pointDiff);
                PostGainPoints(playerInGame.Player.Id, pointDiff);
            }
			game.FinishGame();
            foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
            {
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                GameEndedMsg.AsyncPost(otherServerPlayer.connection);
            }
            GameTable.Finish(game.Id);
            foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
            {
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                otherServerPlayer.serverGame.OnQuitGame();
            }
        }

        private void StartTurnTimerWithAdequatePlayer()
        {
            if (serverRoom.room.Rules.maxTurnTime.value == 0)
                return;
            ServerPlayer otherServerPlayer; //Schrödinger Variable
            Globals.players.TryGetValue(game.CurrentPlayer.Player.Id, out otherServerPlayer);
            if (otherServerPlayer == null)  //WTF!?
                throw new Exception("Whatever...");
            otherServerPlayer.serverGame.StartTurnTimer();
        }

        private void StartTurnTimer()
        {
            if (turnTimeoutTimer != null)
                return;
            uint turnNumber = game.TurnNumber;
            turnTimeoutTimer = Task.Factory.StartNew(() => TurnTimerTaskBody(turnNumber));
        }

        private void StopTurnTimer()
        {
            if (turnTimeoutTimer == null)
                return;
            turnTimeoutSemaphore.Release();
            turnTimeoutTimer.Wait();
            if (turnTimeoutSemaphore.WaitOne(0))
                return;
            turnTimeoutTimer = null;
            return;
        }

        private void TurnTimerTaskBody(uint turnNumber)
        {
            if (turnTimeoutSemaphore.WaitOne((int)serverRoom.room.Rules.maxTurnTime.value * 1000))
                return;
            Task.Factory.StartNew(() => OnTurnTimeout(turnNumber));
        }

        public void OnTurnTimeout(uint turnNumber)
        {
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                turnTimeoutTimer = null;
                if (!InGame)
                    return;
                if (game.TurnNumber != turnNumber)
                    return;
                foreach (PlayerInRoom playerInRoom in serverRoom.room.Players)
                {
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever...");
                    TimeoutOccerredMsg.AsyncPost(otherServerPlayer.connection);
                }
                SwitchToNextPlayer(true);
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

    }
}
