using Common_classes.Players;

namespace Common_classes.Rooms.Players
{
    public class PlayerInRoom
    {

        private Player player;
        private bool isReady = false;


        public Player Player
        {
            get
            {
                return player;
            }
        }

        public bool IsReady
        {
            get
            {
                return isReady;
            }
            set
            {
                isReady = value;
            }
        }


        public PlayerInRoom(Player player)
        {
            this.player = player;
        }

    }
}
