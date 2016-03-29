namespace Common_classes.Rooms.Players
{
    public class PlayerInRoom
    {

        //TODO when class 'Player' will be created: private Player player;
        private bool isReady = false;


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

    }
}
