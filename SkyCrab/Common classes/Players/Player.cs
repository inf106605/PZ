namespace SkyCrab.Common_classes.Players
{
    public class Player
    {

        private uint id;
        private bool isGuest;
        private string nick;
        private PlayerProfile? profile;


        public uint Id
        {
            get { return id; }
        }

        public bool IsGuest
        {
            get { return isGuest; }
        }

        public string Nick
        {
            get { return nick; }
            set {
                nick = value;
                if (profile.HasValue)
                {
                    PlayerProfile playerProfile = profile.Value;
                    playerProfile.nick = nick;
                    profile = playerProfile;
                }
            }
        }

        public PlayerProfile? Profile
        {
            get { return profile; }
            set {
                profile = value;
                if (profile.HasValue)
                    nick = profile.Value.nick;
            }
        }


        public Player(uint id, bool isGuest, string nick)
        {
            this.id = id;
            this.isGuest = isGuest;
            this.nick = nick;
            this.profile = null;
        }

        public Player(uint id, PlayerProfile profile)
        {
            this.id = id;
            this.isGuest = false;
            this.nick = profile.nick;
            this.profile = profile;
        }

    }
}
