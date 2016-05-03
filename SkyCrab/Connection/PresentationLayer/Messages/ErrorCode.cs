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
        EMAIL_OCCUPIED          = 5,
        //EDIT_PROFILE
        NOT_LOGGED2             = NOT_LOGGED,
        NICK_IS_TOO_SHITTY      = 6,
        EMAIL_OCCUPIED2         = EMAIL_OCCUPIED,
        //GET_FRIENDS
        NOT_LOGGED3             = NOT_LOGGED,
        //ADD_FRIEND
        NOT_LOGGED4             = NOT_LOGGED,
        FRIEND_ALREADY_ADDED    = 7,
        FOREVER_ALONE           = 8,
        NO_SUCH_PLAYER          = 9,
        //REMOVE_FRIEND
        NOT_LOGGED5             = NOT_LOGGED,
        NO_SUCH_FRIEND          = 10,
        //GET_FRIEND_ROOMS
        NOT_LOGGED6             = NOT_LOGGED,
        //CREATE_ROOM
        ALREADY_IN_ROOM         = 11,
        //JOIN_ROOM
        NO_SUCH_ROOM            = 12,
        ALREADY_IN_ROOM2        = ALREADY_IN_ROOM,
        ROOM_IS_FULL            = 13,
        //LEAVE_ROOM
        NOT_IN_ROOM             = 14
    }
}
