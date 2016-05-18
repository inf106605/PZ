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
                                SkyCrabGlobalVariables.room.room.AddPlayer((Player)messageInfo.message);
                            }
                            break;
                        }

                    case MessageId.PLAYER_LEAVED:
                        {
                            lock(SkyCrabGlobalVariables.roomLock)
                                SkyCrabGlobalVariables.room.room.RemovePlayer((uint)messageInfo.message);
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
                                SkyCrabGlobalVariables.room.room.OwnerId = (UInt32)messageInfo.message;
                            break;
                        }
                    case MessageId.CHAT:
                        {
                            SkyCrabGlobalVariables.chatMessages = new ChatMessage();
                            SkyCrabGlobalVariables.chatMessages = (ChatMessage)messageInfo.message;

                            PlayerInRoom tempPlayer = SkyCrabGlobalVariables.room.room.GetPlayer(SkyCrabGlobalVariables.chatMessages.PlayerId);
                            SkyCrabGlobalVariables.MessagesLog += tempPlayer.Player.Nick + ": " + SkyCrabGlobalVariables.chatMessages.Message + Environment.NewLine;

                            SkyCrabGlobalVariables.chatMessages = null;
                            break;
                        }

                    case MessageId.GAME_STARTED:
                        {
                            SkyCrabGlobalVariables.game = new Game((uint)messageInfo.message, SkyCrabGlobalVariables.room.room, true);
                            SkyCrabGlobalVariables.isGame = true;
                            SkyCrabGlobalVariables.GameId = (uint)messageInfo.message;
                            break;
                        }

                    case MessageId.GAME_ENDED:
                        {
                            SkyCrabGlobalVariables.isGame = false;
                            SkyCrabGlobalVariables.game = null;
                            break;
                        }
                    
                    case MessageId.NEXT_TURN:
                        {
                            SkyCrabGlobalVariables.game.CurrentPlayerId = (uint)messageInfo.message;

                            if(SkyCrabGlobalVariables.player.Id == (uint)messageInfo.message)
                            {
                                SkyCrabGlobalVariables.isMyRound = true;
                            }
                            else
                            {
                                SkyCrabGlobalVariables.isMyRound = false;
                            }

                            break;
                        }
                    case MessageId.TURN_TIMEOUT:
                        {
                            SkyCrabGlobalVariables.isMyRound = false;
                            break;
                        }
                 
                    case MessageId.NEW_TILES:
                        {
                            DrawedLetters newTiles = (DrawedLetters)messageInfo.message;

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

                    case MessageId.GAIN_POINTS: // aktualizacja punktów graczy
                        {
                            SkyCrabGlobalVariables.gainPoints = true;
                            PlayerPoints playerPoints = (PlayerPoints)messageInfo.message;
                            SkyCrabGlobalVariables.game.GetPlayer(playerPoints.playerId).GainPoints(playerPoints.points);
                            break;
                        }

                    case MessageId.PLACE_TILES:
                        {
                            SkyCrabGlobalVariables.TilesToPlaceByPlayers = (TilesToPlace)messageInfo.message;
                            if(SkyCrabGlobalVariables.TilesToPlaceByPlayers.playerId != SkyCrabGlobalVariables.player.Id)
                                 SkyCrabGlobalVariables.isPlacedTilesByPlayers = true;
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
                                SkyCrabGlobalVariables.game.CurrentPlayer.Rack.TakeOff(new TileOnRack(new Tile(lossTile.letter)));

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
