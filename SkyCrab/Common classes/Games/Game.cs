using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Common_classes.Games.Players;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Common_classes.Rooms.Players;
using System;

namespace SkyCrab.Common_classes.Games
{
    class Game
    {

        private UInt32 id;
        private Room room;
        private Board board;
        private PlayerInGame[] players;
        private uint currentPlayerNumber = 0;
        private Pouch.Pouch[] pouches;
        private bool isDummy;


        public UInt32 Id
        {
            get { return id; }
        }

        public Room Room
        {
            get { return room; }
        }

        public Board Board
        {
            get { return board; }
        }

        public PlayerInGame[] Players
        {
            get { return players; }
        }

        public uint CurrentPlayerNumber
        {
            get { return currentPlayerNumber; }
            set { currentPlayerNumber = value; }
        }

        public PlayerInGame CurrentPlayer
        {
            get { return players[currentPlayerNumber]; }
        }

        public Pouch.Pouch[] Puoches
        {
            get { return pouches; }
        }

        public bool IsDummy
        {
            get { return isDummy; }
        }


        public Game(UInt32 id, Room room, bool isDummy)
        {
            this.id = id;
            this.room = room;
            this.board = room.Rules.CreateBoard();
            this.players = new PlayerInGame[room.Players.Count];
            uint i = 0;
            foreach (PlayerInRoom playerInRoom in room.Players)
                this.players[i++] = new PlayerInGame(playerInRoom.Player);
            this.pouches = room.Rules.CreatePouches(isDummy);
            this.isDummy = isDummy;
        }

    }
}
