namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public enum MessageId : byte
    {
        //Common
        DISCONNECT          = 0,
        OK_DISCONNECT       = 1,
        PING                = 2,
        PONG                = 3,
        NO_PONG             = 4,
        //Menu
        OK                  = 5,
        ERROR               = 6,
        LOGIN               = 7,
        LOGIN_OK            = 8,
        LOGOUT              = 9,
        REGISTER            = 10,
        EDIT_PROFILE        = 11,
        GET_FRIENDS         = 12,
        FIND_PLAYERS        = 13,
        PLAYER_LIST         = 14,
        ADD_FRIEND          = 15,
        REMOVE_FRIEND       = 16,
        GET_FRIEND_ROOMS    = 17,
        FIND_ROOMS          = 18,
        ROOM_LIST           = 19,
        CREATE_ROOM         = 20,
        ROOM                = 21/*,
        JOIN_ROOM           = 22,
        LEAVE_ROOM          = 23,
        PLAYER_JOINED       = 24,
        PLAYER_LEVED        = 25,
        CHAT                = 26*/
    }
}
