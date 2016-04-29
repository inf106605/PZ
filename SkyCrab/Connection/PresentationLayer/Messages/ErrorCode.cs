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
        PASSWORD_TOO_SHORT2     = PASSWORD_TOO_SHORT,
        EMAIL_OCCUPIED2         = EMAIL_OCCUPIED,
        //GET_FRIENDS
        NOT_LOGGED2             = NOT_LOGGED,
        //ADD_FRIEND
        NOT_LOGGED3             = NOT_LOGGED,
        FRIEND_ALREADY_ADDED    = 8,
        FOREVER_ALONE           = 9,
        NO_SUCH_PLAYER          = 10,
        //REMOVE_FRIEND
        NOT_LOGGED4             = NOT_LOGGED,
        NO_SUCH_FRIEND          = 11,
        //GET_FRIEND_ROOMS
        NOT_LOGGED5             = NOT_LOGGED,
        //CREATE_ROOM
        ALREADY_IN_ROOM         = 12
    }
}
