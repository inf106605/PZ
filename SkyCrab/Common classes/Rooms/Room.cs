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
        private RoomType roomType;
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
                foreach (PlayerInRoom playerInRoom in players)
                    if (playerInRoom.Player.Id == ownerId)
                        return playerInRoom;
                throw new NoSuchPlayerInRoomException(ownerId);
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


        public RoomType RoomType
        {
            get { return roomType; }
            set { roomType = value; }
        }

        public RuleSet Rules
        {
            get { return rules; }
        }



        public string MaxPlayersLimit
        {
            get
            {
                return rules.maxPlayerCount.value.ToString();
            }
        }

        public string MaxTimeLimit
        {
            get
            {
                if(rules.maxRoundTime.value == 0)
                {
                    return "Brak limitu";
                }
                else
                    return rules.maxRoundTime.value.ToString();
            }
        }

        public string IsRulesFive
        {
            get
            {
                if(rules.fivesFirst.value == true)
                {
                    return "✓";
                }
                else if(rules.fivesFirst.value == false)
                {
                    return "-";
                }
                return "-";
            }
        }

        public string IsRulesExchange
        {
            get
            {
                if (rules.restrictedExchange.value == true)
                {
                    return "✓";
                }
                else if (rules.restrictedExchange.value == false)
                {
                    return "-";
                }
                return "-";
            }
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

        public Room(uint id, string name, RoomType roomType, RuleSet rules)
        {
            this.id = id;
            this.ownerId = 0;
            LengthLimit.RoomName.CheckAndThrow(name);
            this.name = name;
            this.roomType = roomType;
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
