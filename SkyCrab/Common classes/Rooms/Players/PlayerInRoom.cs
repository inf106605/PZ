using SkyCrab.Common_classes.Players;

namespace SkyCrab.Common_classes.Rooms.Players
{
    public class PlayerInRoom
    {

        private Player player;
        private bool isReady = false;

        public string isReadyStatus
        {
            get
            {
                if (isReady)
                    return "GOTOWY";
                return "OCZEKUJĄCY";
            }
        }

        public Player Player
        {
            get { return player; }
        }

        public bool IsReady
        {
            get { return isReady; }
            set { isReady = value; }
        }

        public PlayerInRoom(Player player)
        {
            this.player = player;
        }

    }
}
