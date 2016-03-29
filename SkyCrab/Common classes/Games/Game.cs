using Common_classes.Games.Boards;
using Common_classes.Games.Players;
using Common_classes.Games.Pouches;
using Common_classes.Players;
using Common_classes.Rooms.Rules;
using System.Collections.Generic;

namespace Common_classes.Games
{
    class Game
    {

        private RuleSet rules;
        private Board board;
        private PlayerInGame[] players;
        private uint currentPlayerNumber = 0;
        private Pouch[] pouches;
        private bool isDummy;


        public RuleSet Rules
        {
            get
            {
                return rules;
            }
        }

        public Board Board
        {
            get
            {
                return board;
            }
        }

        public PlayerInGame[] Players
        {
            get
            {
                return players;
            }
        }

        public uint CurrentPlayerNumber
        {
            get
            {
                return currentPlayerNumber;
            }
            set
            {
                currentPlayerNumber = value;
            }
        }

        public PlayerInGame CurrentPlayer
        {
            get
            {
                return players[currentPlayerNumber];
            }
        }

        public Pouch[] Puoches
        {
            get
            {
                return pouches;
            }
        }

        public bool IsDummy
        {
            get
            {
                return isDummy;
            }
        }


        public Game(RuleSet rules, IList<Player> players, bool isDummy)
        {
            this.rules = rules;
            this.board = rules.CreateBoard();
            this.players = new PlayerInGame[players.Count];
            uint i = 0;
            foreach (Player player in players)
                this.players[i++] = new PlayerInGame(player);
            this.pouches = rules.CreatePouches(isDummy);
            this.isDummy = isDummy;
        }

    }
}
