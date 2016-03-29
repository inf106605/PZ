namespace Common_classes.Players
{
    public class Player
    {

        private uint id;
        private string nick;
        private PlayerProfile profile;


        public uint Id
        {
            get
            {
                return id;
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


        public Player(uint id, string nick)
        {
            this.id = id;
            this.nick = nick;
        }

    }
}
