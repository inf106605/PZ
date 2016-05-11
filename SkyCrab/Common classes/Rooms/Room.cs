using SkyCrab.Common_classes.Players;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Common_classes.Rooms.Rules;
using System;
using System.Collections.Generic;

namespace SkyCrab.Common_classes.Rooms
{
    public class TooManyPlayersInRoomException : SkyCrabException
    {
        public TooManyPlayersInRoomException(byte maxPlayers) :
            base("There can be at most " + maxPlayers + " in this room!")
        {
        }
    }

    public class PlayerAlreadyInRoomException : SkyCrabException
    {
        public PlayerAlreadyInRoomException(UInt32 playerId) :
            base("It is already player with ID " + playerId + " in this room!")
        {
        }
    }

    public class NoSuchPlayerInRoomException : SkyCrabException
    {
        public NoSuchPlayerInRoomException(UInt32 playerId) :
            base("There is not player with ID " + playerId + " in this room!")
        {
        }
    }

    public class Room
    {

        public uint MAX_PLAYERS = 4;

        private uint id;
        private UInt32 ownerId;
        private string name;
        private RoomType type;
        private readonly RuleSet rules = new RuleSet();
        private LinkedList<PlayerInRoom> players = new LinkedList<PlayerInRoom>();


        public uint Id
        {
            get { return id; }
            set { id = value; }
        }

        public UInt32 OwnerId
        {
            get { return ownerId; }
            set
            {
                ownerId = value;
                if (Owner == null)
                    ownerId = 0;
            }
        }

        public PlayerInRoom Owner
        {
            get
            {
                if (ownerId == 0)
                    return null;
                return GetPlayer(ownerId);
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                LengthLimit.RoomName.CheckAndThrow(value);
                name = value;
            }
        }

        public RoomType Type
        {
            get { return type; }
            set { type = value; }
        }

        public RuleSet Rules
        {
            get { return rules; }
        }


        public LinkedList<PlayerInRoom> Players
        {
            get { return players; }
        }

        public bool AllPlayersReady
        {
            get
            {
                foreach (PlayerInRoom player in players)
                    if (!player.IsReady)
                        return false;
                return true;
            }
        }


        public Room()
        {
        }

        public Room(uint id, string name, RoomType type, RuleSet rules)
        {
            this.id = id;
            this.ownerId = 0;
            LengthLimit.RoomName.CheckAndThrow(name);
            this.name = name;
            this.type = type;
            this.rules = rules;
        }

        public void AddPlayer(Player player)
        {
            if (players.Count >= MAX_PLAYERS)
                throw new TooManyPlayersInRoomException(rules.maxPlayerCount.value);
            if (hasPlayer(player.Id))
                throw new PlayerAlreadyInRoomException(player.Id);
            PlayerInRoom playerInRoom = new PlayerInRoom(player);
            players.AddLast(playerInRoom);
        }

        public PlayerInRoom GetPlayer(UInt32 playerId)
        {
            foreach (PlayerInRoom playerInRoom in players)
                if (playerInRoom.Player.Id == playerId)
                    return playerInRoom;
            throw new NoSuchPlayerInRoomException(playerId);
        }

        public void RemovePlayer(uint playerId)
        {
            for (var i = players.First; i != null; i = i.Next)
                if (i.Value.Player.Id == playerId)
                {
                    players.Remove(i);
                    if (playerId == ownerId)
                        ownerId = 0;
                    return;
                }
            throw new NoSuchPlayerInRoomException(playerId);
        }

        public bool hasPlayer(uint playerId)
        {
            foreach (PlayerInRoom playerInRoom in players)
                if (playerInRoom.Player.Id == playerId)
                    return true;
            return false;
        }
        
        public void SetPlayerReady(uint playerId, bool ready)
        {
            foreach (PlayerInRoom playerInRoom in players)
                if (playerInRoom.Player.Id == playerId)
                {
                    playerInRoom.IsReady = ready;
                    return;
                }
        }

    }
}
