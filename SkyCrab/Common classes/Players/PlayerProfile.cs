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
                LengthLimit.Login.CheckAndThrow(value);
                login = value;
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                LengthLimit.Password.CheckAndThrow(value);
                password = value;
            }
        }

        public string Nick
        {
            get { return nick; }
            set
            {
                LengthLimit.Nick.CheckAndThrow(value);
                nick = value;
            }
        }

        public string EMail
        {
            get { return eMail; }
            set
            {
                LengthLimit.EMail.CheckAndThrow(value);
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
