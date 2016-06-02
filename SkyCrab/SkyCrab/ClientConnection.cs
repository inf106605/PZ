using SkyCrab.Classes.ScrabbleGameFolder;
using SkyCrab.Classes.Menu.Guest;
using SkyCrab.Classes.Menu.LoggedPlayer;
using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Chats;
using SkyCrab.Common_classes.Players;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SkyCrab.Connection.PresentationLayer.Messages.Game;
using SkyCrab.Common_classes.Games.Pouches;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Common_classes.Games;
using SkyCrab.Common_classes.Games.Tiles;
using SkyCrab.Common_classes.Games.Players;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Games;

namespace SkyCrab
{
    class ClientConnection : AbstractClientConnection
    {
        public ClientConnection(string host, int port, int readTimeout) :
           base(host, port, readTimeout)
        {

        }

        protected override void ProcessMessages()
        {
            foreach (MessageInfo messageInfo in messages.GetConsumingEnumerable())
            {
                processingMessagesOk = true;

                switch(messageInfo.messageId)
                {
                    case MessageId.PING:
                        {
                            AnswerPing(messageInfo.id, messageInfo.message);
                            break;
                        }
                    case MessageId.NO_PONG:
                        {
                            DisplayMessageBox("Serwer nie odpowiada!");
                            AsyncDispose();
                            break;
                        }

                    case MessageId.PLAYER_JOINED:
                        {
                           lock(SkyCrabGlobalVariables.roomLock)
                            {
                                SkyCrabGlobalVariables.MessagesLog += " [ " + ((Player)messageInfo.message).Nick + " dołączył do pokoju. ]" + Environment.NewLine;
                                SkyCrabGlobalVariables.room.room.AddPlayer((Player)messageInfo.message);
                            }
                            break;
                        }

                    case MessageId.PLAYER_LEAVED:
                        {
                            lock(SkyCrabGlobalVariables.roomLock)
                            {
                                if(SkyCrabGlobalVariables.game != null && SkyCrabGlobalVariables.game.Players != null)
                                     SkyCrabGlobalVariables.MessagesLog += " [ " + SkyCrabGlobalVariables.game.GetPlayer((uint)messageInfo.message).Player.Nick + " opuścił pokój. ]" + Environment.NewLine;
                                SkyCrabGlobalVariables.room.room.RemovePlayer((uint)messageInfo.message);
                                SkyCrabGlobalVariables.game.GetPlayer((uint)messageInfo.message).Walkover = true;
                            }
                            break;
                        }

                    case MessageId.PLAYER_READY:
                        {
                            lock(SkyCrabGlobalVariables.roomLock)
                                SkyCrabGlobalVariables.room.room.SetPlayerReady((uint)messageInfo.message, true);
                            break;
                        }
                    case MessageId.PLAYER_NOT_READY:
                        {
                            lock(SkyCrabGlobalVariables.roomLock)
                                SkyCrabGlobalVariables.room.room.SetPlayerReady((uint)messageInfo.message, false);
                            break;
                        }
                    case MessageId.NEW_ROOM_OWNER:
                        {
                            lock(SkyCrabGlobalVariables.roomLock)
                            {
                                SkyCrabGlobalVariables.room.room.OwnerId = (UInt32)messageInfo.message;
                            }
                            break;
                        }
                    case MessageId.CHAT:
                        {
                            SkyCrabGlobalVariables.chatMessages = new ChatMessage();
                            SkyCrabGlobalVariables.chatMessages = (ChatMessage)messageInfo.message;

                            if (SkyCrabGlobalVariables.chatMessages.PlayerId == 0)
                            {
                                SkyCrabGlobalVariables.MessagesLog += "<SERWER>: " + SkyCrabGlobalVariables.chatMessages.Message + Environment.NewLine;
                            }
                            else
                            {
                                PlayerInRoom tempPlayer = SkyCrabGlobalVariables.room.room.GetPlayer(SkyCrabGlobalVariables.chatMessages.PlayerId);
                                SkyCrabGlobalVariables.MessagesLog += tempPlayer.Player.Nick + ": " + SkyCrabGlobalVariables.chatMessages.Message + Environment.NewLine;
                            }

                            SkyCrabGlobalVariables.chatMessages = null;
                            break;
                        }

                    case MessageId.GAME_STARTED:
                        {
                            SkyCrabGlobalVariables.MessagesLog += ">> ROZPOCZĘCIE NOWEJ GRY <<" + Environment.NewLine;
                            SkyCrabGlobalVariables.game = new Game((uint)messageInfo.message, SkyCrabGlobalVariables.room.room, true);
                            SkyCrabGlobalVariables.isGame = true;
                            SkyCrabGlobalVariables.GameId = (uint)messageInfo.message;
                            break;
                        }

                    case MessageId.GAME_ENDED:
                        {
                            SkyCrabGlobalVariables.MessagesLog += ">> ZAKOŃCZENIE GRY <<" + Environment.NewLine;
                            SkyCrabGlobalVariables.MessagesLog += "Wyniki graczy: " + Environment.NewLine;
                            if (SkyCrabGlobalVariables.game.Players != null)
                            {
                                List<PlayerInGame> playerInGame = new List<PlayerInGame>(SkyCrabGlobalVariables.game.Players);
                                playerInGame.Sort((player1, player2) =>
                                {
                                    if (player1.Walkover != player2.Walkover)
                                    {
                                        return player1.Walkover ? 1 : -1;
                                    }
                                    else
                                    {
                                        if (player1.Points == player2.Points)
                                            return 0;

                                        else
                                        {
                                            return player1.Points > player2.Points ? -1 : 1; // -1 element wcześniejszy , 1 element późniejszy
                                    }
                                    }
                                });

                                uint currentPlace = 0;
                                uint currentPoints = uint.MaxValue;
                                foreach (var player in playerInGame)
                                {
                                    if (!player.Walkover)
                                    {
                                        if (player.Points != currentPoints)
                                        {
                                            ++currentPlace;
                                            currentPoints = player.Points;
                                        }
                                        SkyCrabGlobalVariables.MessagesLog += currentPlace + ") ";
                                    }
                                    SkyCrabGlobalVariables.MessagesLog += player.Player.Nick + " : " + player.Points;
                                    if (player.Walkover)
                                        SkyCrabGlobalVariables.MessagesLog += " (WALKOWER)";
                                    SkyCrabGlobalVariables.MessagesLog += Environment.NewLine;
                                }

                                MessageBoxResult result = MessageBox.Show("Czy chcesz zapisać etapy rozgrywki ?", "Komunikat zapisu przebiegu gry", MessageBoxButton.YesNo);
                                switch (result)
                                {
                                    case MessageBoxResult.Yes:
                                        {
                                            var getGameLogMsgAnswer = GetGameLogMsg.SyncPost(App.clientConn, SkyCrabGlobalVariables.GameId, 1000);

                                            if (!getGameLogMsgAnswer.HasValue)
                                            {
                                                MessageBox.Show("Brak odpowiedzi od serwera!");
                                            }

                                            var answerValue = getGameLogMsgAnswer.Value;

                                            if (answerValue.messageId == MessageId.ERROR)
                                            {
                                                ErrorCode errorCode = (ErrorCode)answerValue.message;

                                                switch (errorCode)
                                                {
                                                    case ErrorCode.NO_SUCH_GAME:
                                                        {
                                                            MessageBox.Show("Nie ma Cię w grze!");
                                                            break;
                                                        }
                                                    case ErrorCode.GAME_NOT_ENDED:
                                                        {
                                                            MessageBox.Show("Gra jeszcze się nie skończyła!");
                                                            break;
                                                        }
                                                }
                                            }

                                            if (answerValue.messageId == MessageId.GAME_LOG)
                                            {
                                                string gameLogString = (string)answerValue.message;

                                                SaveFileDialog saveFileDialog = new SaveFileDialog();
                                                saveFileDialog.Filter = "Text File | *.txt";
                                                if (saveFileDialog.ShowDialog() == true)
                                                    File.WriteAllText(saveFileDialog.FileName, gameLogString);
                                            }

                                            break;
                                        }
                                    case MessageBoxResult.No:
                                        break;
                                }

                                SkyCrabGlobalVariables.room.room.SetPlayerReady(SkyCrabGlobalVariables.player.Id, false);
                                SkyCrabGlobalVariables.game.Room.SetPlayerReady(SkyCrabGlobalVariables.player.Id, false);
                                SkyCrabGlobalVariables.isGame = false;
                                SkyCrabGlobalVariables.game = null;
                            }
                            break;
                        }
                    
                    case MessageId.NEXT_TURN:
                        {
                            SkyCrabGlobalVariables.game.CurrentPlayerId = (uint)messageInfo.message;

                            if (SkyCrabGlobalVariables.player.Id == (uint)messageInfo.message)
                            {
                                SkyCrabGlobalVariables.isMyRound = true;
                                SkyCrabGlobalVariables.myMaxTurnTime = TimeSpan.FromSeconds(SkyCrabGlobalVariables.game.Room.Rules.maxTurnTime.value);
                            }
                            else
                            {
                                SkyCrabGlobalVariables.isMyRound = false;
                            }

                            break;
                        }
                    case MessageId.TIMEOUT_OCCURRED:
                        {
                            SkyCrabGlobalVariables.MessagesLog += "[ " + SkyCrabGlobalVariables.game.CurrentPlayer.Player.Nick + " wyczerpał cały limit czasu na ruch. ]" + Environment.NewLine;

                            SkyCrabGlobalVariables.isMyRound = false;
                            SkyCrabGlobalVariables.timeSpanMyRound = false;
                            break;
                        }
                 
                    case MessageId.NEW_TILES:
                        {
                            DrawedLetters newTiles = (DrawedLetters)messageInfo.message;
                            /*
                            String formWord = "";

                            if (newTiles.letters.Count == 1)
                                formWord = " nową płytkę";
                            if (newTiles.letters.Count >= 2 && newTiles.letters.Count <= 4)
                                formWord = " nowe płytki";
                            else
                                formWord = " nowych płytek";

                            SkyCrabGlobalVariables.MessagesLog += "[ Gracz  " + SkyCrabGlobalVariables.game.GetPlayer(newTiles.playerId).Player.Nick + " otrzymał " + newTiles.letters.Count + " " + formWord +  " . ]" + Environment.NewLine;
                            */

                            foreach (var item in newTiles.letters)
                                SkyCrabGlobalVariables.game.GetPlayer(newTiles.playerId).Rack.PutTile(new Tile(item));

                           
                            if (newTiles.playerId == SkyCrabGlobalVariables.player.Id)
                            {
                                SkyCrabGlobalVariables.newTile = newTiles;
                                if (SkyCrabGlobalVariables.newTile.playerId == SkyCrabGlobalVariables.player.Id)
                                    SkyCrabGlobalVariables.isGetNewTile = true;
                                else
                                    SkyCrabGlobalVariables.isGetNewTile = false;
                            }
                            else if(newTiles.playerId != SkyCrabGlobalVariables.player.Id)
                            {
                                SkyCrabGlobalVariables.anotherPlayersGetNewTile = true;
                                SkyCrabGlobalVariables.anotherPlayersGetNewTileCount = (uint)newTiles.letters.Count;
                            }
                            break;
                        }

                    case MessageId.EXCHANGE_TILES:
                        {
                            break;
                        }

                    case MessageId.PLAYER_PLACED_TILES:
                        {
                            WordOnBoard wordOnBoard = (WordOnBoard)messageInfo.message;

                            SkyCrabGlobalVariables.MessagesLog += "[ " + SkyCrabGlobalVariables.game.CurrentPlayer.Player.Nick + " ułożył słowo: " + wordOnBoard.word 
                                + " na pozycji "+ SkyCrabGlobalVariables.game.Board.getSquareID(wordOnBoard) + ". ]" + Environment.NewLine;
                          
                            break;
                        }

                    case MessageId.PLAYER_FAILED:
                        {
                            WordOnBoard wordOnBoard = (WordOnBoard)messageInfo.message;

                            SkyCrabGlobalVariables.MessagesLog += "[ " + SkyCrabGlobalVariables.game.CurrentPlayer.Player.Nick + " próbował ułożyć błędne słowo: " + wordOnBoard.word
                                + " na pozycji " + SkyCrabGlobalVariables.game.Board.getSquareID(wordOnBoard) + ". ]" + Environment.NewLine;

                            break;
                        }

                    case MessageId.PLAYER_EXCHAN_TILES:
                        {
                            byte countTiles = (byte)messageInfo.message;
                            string formsWord = "";
                            if(countTiles==1)
                            {
                                formsWord = "płytkę";
                            }
                            else if (countTiles >= 2 && countTiles <= 4)
                            {
                                formsWord = "płytki";
                            }
                            else
                                formsWord = "płytek";

                            SkyCrabGlobalVariables.MessagesLog += "[ " + SkyCrabGlobalVariables.game.CurrentPlayer.Player.Nick + " wymienił "+ countTiles.ToString() + " " + formsWord + ". ]" + Environment.NewLine;
                            break;
                        }

                    case MessageId.PLAYER_PASSED:
                        {
                            SkyCrabGlobalVariables.MessagesLog += "[ " + SkyCrabGlobalVariables.game.CurrentPlayer.Player.Nick + " spasował. ]" + Environment.NewLine;
                            break;
                        }

                    case MessageId.POINTS_CHANGED: // aktualizacja punktów graczy
                        {
                            SkyCrabGlobalVariables.gainPoints = true;
                            PlayerPoints playerPoints = (PlayerPoints)messageInfo.message;
                            SkyCrabGlobalVariables.game.GetPlayer(playerPoints.playerId).GainPoints(playerPoints.pointsDifference);
                            break;
                        }

                    case MessageId.PLACE_TILES:
                        {
                            SkyCrabGlobalVariables.TilesToPlaceByPlayers = (TilesToPlace)messageInfo.message;
                            //SkyCrabGlobalVariables.MessagesLog += "[ Gracz " + SkyCrabGlobalVariables.game.GetPlayer(SkyCrabGlobalVariables.TilesToPlaceByPlayers.playerId).Player.Nick + " położył " + SkyCrabGlobalVariables.TilesToPlaceByPlayers.tilesToPlace.Count + " płytki na planszy. ]" + Environment.NewLine;
                            if (SkyCrabGlobalVariables.TilesToPlaceByPlayers.playerId != SkyCrabGlobalVariables.player.Id)
                                 SkyCrabGlobalVariables.isPlacedTilesByPlayers = true;
                            break;
                        }

                    case MessageId.REORDER_RACK_TILES:
                         {
                            break;
                         }
                    

                    case MessageId.PASS:
                    {   
                            break;
                    }
                    case MessageId.LOSS_TILES:
                    {
                            LostLetters lostLetters = (LostLetters)messageInfo.message;

                            foreach (var lossTile in lostLetters.letters)
                                SkyCrabGlobalVariables.game.CurrentPlayer.Rack.TakeOff(lossTile.letter);

                            SkyCrabGlobalVariables.lostLetters = lostLetters;
                            break;
                    }
                    
                    default:
                        {
                            DisplayMessageBox("Otrzymano nieobsługiwany komunikat od serwera (" + messageInfo.messageId.ToString() + ")!");
                            throw new SkyCrabException("Unsuported message: " + messageInfo.messageId.ToString());
                        }
                }
            }
        }

        private void DisplayMessageBox(string message)
        {
            Task.Factory.StartNew(()=>System.Windows.MessageBox.Show(message));
        }

        protected override void DoDispose()
        {
            if (!disconectedOnItsOwn)
                DisplayMessageBox("Serwer zakończył pracę");
            base.DoDispose();
        }

    }
}
