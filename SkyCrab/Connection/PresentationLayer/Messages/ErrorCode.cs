using System;

namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public enum ErrorCode : UInt16
    {
        //LOGIN_AS_GUEST
        SESSION_ALREADY_LOGGED = 0,
        //LOGIN
        SESSION_ALREADY_LOGGED2 = SESSION_ALREADY_LOGGED,
        WRONG_LOGIN_OR_PASSWORD = 1,
        USER_ALREADY_LOGGED     = 2,
        //LOGOUT
        NOT_LOGGED              = 3,
        //REGISTER
        SESSION_ALREADY_LOGGED3 = SESSION_ALREADY_LOGGED,
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
        NOT_LOGGED7             = NOT_LOGGED,
        ALREADY_IN_ROOM         = 11,
        INVALID_RULES           = 12,
        //JOIN_ROOM
        NOT_LOGGED8             = NOT_LOGGED,
        ALREADY_IN_ROOM2        = ALREADY_IN_ROOM,
        NO_SUCH_ROOM            = 13,
        GAME_IS_STARTED         = 14,
        ROOM_IS_FULL            = 15,
        //LEAVE_ROOM
        NOT_IN_ROOM             = 16,
        //PLAYER_READY
        NOT_IN_ROOM2            = NOT_IN_ROOM,
        IN_GAME                 = 17,
        //PLAYER_NOT_READY
        NOT_IN_ROOM3            = NOT_IN_ROOM,
        IN_GAME2                = IN_GAME,
        //CHAT
        NOT_IN_ROOM4            = NOT_IN_ROOM,

        //GET_GAME_LOG
        NO_SUCH_GAME            = 18,
        GAME_NOT_ENDED          = 19,

        //REORDER_RACK_TILES
        NOT_IN_GAME             = 20,
        INCORRECT_MOVE          = 21,
        //PLACE_TILES
        NOT_IN_GAME2            = NOT_IN_GAME,
        NOT_YOUR_TURN           = 22,
        INCORRECT_MOVE2         = INCORRECT_MOVE,
        TOO_LESS_TILES          = 23,
        FIVES_FIRST_VIOLATION   = 24,
        LETTERS_NOT_FROM_RACK   = 25,
        LETTERS_NOT_MATH        = 26,
        WORD_NOT_IN_LINE        = 27,
        NOT_IN_STARTING_SQUARE  = 28,
        NOT_ADJANCENT           = 29,
        NOT_CONTINUOUS          = 30,
        INCORECT_WORD           = 31,
        //EXCHANGE_TILES
        NOT_IN_GAME3            = NOT_IN_GAME,
        NOT_YOUR_TURN2          = NOT_YOUR_TURN,
        TOO_LESS_TILES2         = TOO_LESS_TILES,
        LETTERS_NOT_FROM_RACK2  = LETTERS_NOT_FROM_RACK,
        TOO_LESS_POUCH_LETTERS  = 32,
        RESTR_EXCH_VIOLATION    = 33,
        //PASS
        NOT_IN_GAME4            = NOT_IN_GAME,
        NOT_YOUR_TURN3          = NOT_YOUR_TURN
    }
}
