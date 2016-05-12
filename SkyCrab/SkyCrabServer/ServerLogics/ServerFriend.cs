using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Friends;
using SkyCrabServer.Databases;
using System;
using System.Collections.Generic;

namespace SkyCrabServer.ServerLogics
{
    class ServerFriend
    {
        
        private readonly ServerPlayer serverPlayer;


        public ServerFriend(ServerPlayer serverPlayer)
        {
            this.serverPlayer = serverPlayer;
        }

        public void GetFriends(Int16 id)
        {
            if (!serverPlayer.LoggedNormally)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_LOGGED3);
                return;
            }
            List<UInt32> friendIds = FriendTable.GetByPlayerId(serverPlayer.player.Id);
            List<Player> friends = new List<Player>();
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                foreach (UInt32 friendId in friendIds) //TODO Do it using database
                    friends.Add(PlayerProfileTable.GetShortById(friendId));
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
            PlayerListMsg.AsyncPost(id, serverPlayer.connection, friends);
        }

        public void FindPlayers(Int16 id, string searchPhraze)
        {
            List<Player> players;
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                players = PlayerProfileTable.FindShort(searchPhraze);
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
            PlayerListMsg.AsyncPost(id, serverPlayer.connection, players);
        }

        public void AddFriend(Int16 id, UInt32 friendId)
        {
            if (!serverPlayer.LoggedNormally)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_LOGGED4);
                return;
            }
            if (serverPlayer.player.Id == friendId)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.FOREVER_ALONE);
                return;
            }
            if (!PlayerProfileTable.IdExists(friendId))
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NO_SUCH_PLAYER);
                return;
            }
            if (FriendTable.Exists(serverPlayer.player.Id, friendId))
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.FRIEND_ALREADY_ADDED);
                return;
            }

            FriendTable.Create(serverPlayer.player.Id, friendId);
            OkMsg.AsyncPost(id, serverPlayer.connection);
        }

        public void RemoveFriend(Int16 id, UInt32 friendId)
        {
            if (!serverPlayer.LoggedNormally)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_LOGGED5);
                return;
            }
            if (!FriendTable.Exists(serverPlayer.player.Id, friendId))
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NO_SUCH_FRIEND);
                return;
            }

            FriendTable.Delete(serverPlayer.player.Id, friendId);
            OkMsg.AsyncPost(id, serverPlayer.connection);
        }

    }
}
