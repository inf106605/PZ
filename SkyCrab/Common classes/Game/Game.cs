using Common_classes.Game.Boards;
using Common_classes.Game.Players;
using Common_classes.Game.Pouches;

namespace Common_classes.Game
{
    //This is practically a dummy at the moment.
    class Game
    {

        private Board board;
        private PlayerInGame[] players;
        private uint currentPlayerNumber;
        private Pouch[] pouches;


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


        public Game(/*TODO Rules rules*/ /*TODO Player[] players*/)
        {
            this.board = new StandardBoard(); //TODO from rules
            this.players = new PlayerInGame[0]; //TODO
            this.currentPlayerNumber = 0; //TODO randomize
            this.pouches = new Pouch[0]; //TODO
        }

    }
}
