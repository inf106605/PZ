namespace Common_classes.Players
{
    public class Player
    {

        private uint id;
        private bool isGuest;
        private string nick;
        private PlayerProfile profile;


        public uint Id
        {
            get
            {
                return id;
            }
        }

        public bool IsGuest
        {
            get
            {
                return isGuest;
            }
        }

        public string Nick
        {
            get
            {
                return nick;
            }
            set
            {
                nick = value;
            }
        }

        public PlayerProfile Profile
        {
            get
            {
                return profile;
            }
            set
            {
                profile = value;
            }
        }


        public Player(uint id, bool isGuest, string nick)
        {
            this.id = id;
            this.isGuest = isGuest;
            this.nick = nick;
        }

    }
}
