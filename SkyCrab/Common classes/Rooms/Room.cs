using Common_classes.Rooms.Players;
using Common_classes.Rooms.Rules;
using System.Collections.Generic;

namespace Common_classes.Rooms
{
    public class Room
    {

        //TODO when class 'Player' will be created: private Player owner;
        private uint id;
        private string name;
        private RoomType roomType;
        private RuleSet rules;
        private LinkedList<PlayerInRoom> players = new LinkedList<PlayerInRoom>();


        public uint Id
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public RoomType RoomType
        {
            get
            {
                return roomType;
            }
            set
            {
                roomType = value;
            }
        }

        public RuleSet Rules
        {
            get
            {
                return rules;
            }
        }

        public LinkedList<PlayerInRoom> Players
        {
            get
            {
                return players;
            }
        }


        public Room(uint id, string name, RoomType roomType, RuleSet rules)
        {
            this.id = id;
            this.name = name;
            this.roomType = roomType;
            this.rules = rules;
        }

        /*TODO when class 'Player' will be created
        public void AddPlayer(Player player)
        {
            //TODO
        }

        public void RemovePlayer(Player player)
        {
            //TODO
        }
        
        public SetPlayerReady(Player player, bool ready)
        {
            //TODO
        }*/

    }
}
