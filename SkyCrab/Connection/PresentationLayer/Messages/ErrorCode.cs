using System;

namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public enum ErrorCode : UInt16
    {
        //LOGIN
        WRONG_LOGIN_OR_PASSWORD = 0,
        USER_ALREADY_LOGGED     = 1,
        SESSION_ALREADY_LOGGED  = 2,
        //LOGOUT
        NOT_LOGGED              = 3,
        //REGISTER
        LOGIN_OCCUPIED          = 4,
        PASSWORD_TOO_SHORT      = 5,
        EMAIL_OCCUPIED          = 6,
        //EDIT_PROFILE
        NICK_IS_TOO_SHITTY      = 7,
        PASSWORD_TOO_SHORT2     = 5,
        EMAIL_OCCUPIED2         = 6
    }
}
