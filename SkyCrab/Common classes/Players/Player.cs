using System;

namespace SkyCrab.Common_classes.Players
{
    public class Player
    {

        private UInt32 id;
        private bool isGuest;
        private string nick;
        private PlayerProfile profile;


        public UInt32 Id
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
                LengthLimit.Nick.checkAndThrow(value);
                nick = value;
                if (profile != null)
                    profile.Nick = nick;
            }
        }

        public PlayerProfile Profile
        {
            get { return profile; }
            set {
                profile = value;
                if (profile != null)
                    nick = profile.Nick;
            }
        }


        public Player(UInt32 id, bool isGuest, string nick)
        {
            LengthLimit.Nick.checkAndThrow(nick);
            this.id = id;
            this.isGuest = isGuest;
            this.nick = nick;
            this.profile = null;
        }

        public Player(UInt32 id, PlayerProfile profile)
        {
            this.id = id;
            this.isGuest = false;
            this.nick = profile.Nick;
            this.profile = profile;
        }

    }
}
