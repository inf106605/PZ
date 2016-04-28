namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public enum MessageId : byte
    {
        DISCONNECT          = 0,
        OK_DISCONNECT       = 1,
        PING                = 2,
        PONG                = 3,
        NO_PONG             = 4,
        OK                  = 5,
        ERROR               = 6,
        LOGIN               = 7,
        LOGIN_OK            = 8,
        LOGOUT              = 9,
        REGISTER            = 10,
        EDIT_PROFILE        = 11,
        GET_FRIENDS         = 12,
        FIND_PLAYER         = 13,
        PLAYER_LIST         = 14,
        ADD_FRIEND          = 15,
        REMOVE_FRIEND       = 16/*,
        GET_FRIEND_ROOMS    = 17,
        FIND_ROOM           = 18,
        NEW_ROOM            = 19,
        ROOM_LIST           = 20,
        JOIN_ROOM           = 21,
        LEAVE_ROOM          = 22,
        PLAYER_JOINED       = 23,
        PLAYER_LEVED        = 24,
        CHAT                = 25*/
    }
}
