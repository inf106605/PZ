using SkyCrab.Common_classes.Chats;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Players;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrabServer.ServerLogics;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SkyCrabServer.Connactions
{
    sealed class UnsuportedMessageException : SkyCrabServerException
    {
    }

    class ServerConnection : AbstractServerConnection
    {
        
        private readonly ServerPlayer serverPlayer;


        public ServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
            serverPlayer = new ServerPlayer(this);
        }

        //TODO do something smart if exception occured
        protected override void ProcessMessages()
        {
            foreach (MessageInfo messageInfo in messages.GetConsumingEnumerable())
            {
                processingMessagesOk = true;

                Int16 id = messageInfo.id;
                switch (messageInfo.messageId)
                {
                    //COMMON

                    case MessageId.PING:
                        AnswerPing(id, messageInfo.message);
                        break;

                    case MessageId.NO_PONG:
                        Globals.serverConsole.WriteLine("No answer to PING! (" + ClientAuthority + ")");
                        AsyncDispose();
                        break;

                    //MENU

                    //accounts

                    case MessageId.LOGIN_AS_GUEST:
                        serverPlayer.LoginAsGuest(id);
                        break;

                    case MessageId.LOGIN:
                        serverPlayer.Login(id, (PlayerProfile)messageInfo.message);
                        break;

                    case MessageId.LOGOUT:
                        serverPlayer.Logout(id);
                        break;

                    case MessageId.REGISTER:
                        serverPlayer.Register(id, (PlayerProfile)messageInfo.message);
                        break;

                    case MessageId.EDIT_PROFILE:
                        serverPlayer.EditProfile(id, (PlayerProfile)messageInfo.message);
                        break;

                    //friends

                    case MessageId.GET_FRIENDS:
                        serverPlayer.serverFriend.GetFriends(id);
                        break;

                    case MessageId.FIND_PLAYERS:
                        serverPlayer.serverFriend.FindPlayers(id, (string) messageInfo.message);
                        break;

                    case MessageId.ADD_FRIEND:
                        serverPlayer.serverFriend.AddFriend(id, (UInt32) messageInfo.message);
                        break;

                    case MessageId.REMOVE_FRIEND:
                        serverPlayer.serverFriend.RemoveFriend(id, (UInt32)messageInfo.message);
                        break;

                    //room

                    case MessageId.CREATE_ROOM:
                        serverPlayer.serverRoom.CreateRoom(id, (Room)messageInfo.message);
                        break;

                    case MessageId.FIND_ROOMS:
                        serverPlayer.serverRoom.FindRooms(id, (Room)messageInfo.message);
                        break;

                    case MessageId.GET_FRIEND_ROOMS:
                        serverPlayer.serverRoom.GetFriendRooms(id);
                        break;

                    //in rooms

                    case MessageId.JOIN_ROOM:
                        serverPlayer.serverRoom.JoinRoom(id, (UInt32)messageInfo.message);
                        break;

                    case MessageId.LEAVE_ROOM:
                        serverPlayer.serverRoom.LeaveRoom(id);
                        break;

                    case MessageId.PLAYER_READY:
                        serverPlayer.serverRoom.PlayerReady(id);
                        break;

                    case MessageId.PLAYER_NOT_READY:
                        serverPlayer.serverRoom.PlayerNotReady(id);
                        break;

                    case MessageId.CHAT:
                        serverPlayer.serverRoom.Chat(id, (ChatMessage)messageInfo.message);
                        break;

                    //GAME
						
                    case MessageId.GAIN_POINTS:
						//TODO
						break;
						
                    case MessageId.REORDER_RACK_TILES:
						//TODO
						break;
						
                    case MessageId.TURN_TIMEOUT:
						//TODO
						break;
						
                    case MessageId.PLACE_TILES:
						//TODO
						break;
						
                    case MessageId.EXCHANGE_TILES:
                        serverPlayer.serverGame.ExchangeTiles(id, (List<LetterWithNumber>)messageInfo.message);
						break;
						
                    case MessageId.PASS:
                        serverPlayer.serverGame.Pass(id);
						break;
						
                    case MessageId.GAME_ENDED:
						//TODO
						break;
						
                    //Unknown

                    default:
                        throw new UnsuportedMessageException();
                }
            }
        }

        protected override void DoDispose()
        {
            serverPlayer.OnLogout();
            base.DoDispose();
        }

    }
}
