using System;

namespace SkyCrab.Common_classes.Players
{
    public class PlayerProfile
    {

        private string login;
        private string password;
        private string nick;
        private string eMail;
        private DateTime registration;
        private DateTime lastActivity;


        public string Login
        {
            get { return login; }
            set
            {
                LengthLimit.Login.checkAndThrow(value);
                login = value;
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                LengthLimit.Password.checkAndThrow(value);
                password = value;
            }
        }

        public string Nick
        {
            get { return nick; }
            set
            {
                LengthLimit.Nick.checkAndThrow(value);
                nick = value;
            }
        }

        public string EMail
        {
            get { return eMail; }
            set
            {
                LengthLimit.EMail.checkAndThrow(value);
                eMail = value;
            }
        }

        public DateTime Registration
        {
            get { return registration; } 
            set { registration = value; }
        }

        public DateTime LastActivity
        {
            get { return lastActivity; }
            set { lastActivity = value; }
        }

    }
}
