using System;

namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public enum ErrorCode : UInt16
    {
        //LOGIN
        WRONG_LOGIN_OR_PASSWORD = 0,
        USER_ALREADY_LOGGED     = 1,
        //LOGOUT
        NOT_LOGGED              = 2,
        //REGISTER
        LOGIN_OCCUPIED          = 3,
        PASSWORD_TOO_SHORT      = 4,
        EMAIL_OCCUPIED          = 5,
        //EDIT_PROFILE
        NICK_IS_TOO_SHITTY      = 6,
        PASSWORD_TOO_SHORT2     = 4,
        EMAIL_OCCUPIED2         = 5
    }
}
