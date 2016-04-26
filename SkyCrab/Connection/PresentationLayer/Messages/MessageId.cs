namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public enum MessageId : byte
    {
        DISCONNECT          = 0,
        PING                = 1,
        PONG                = 2,
        NO_PONG             = 3,
        OK                  = 4,
        ERROR               = 5,
        LOGIN               = 6,
        LOGIN_OK            = 7,
        LOGOUT              = 8,
        REGISTER            = 9,
        EDIT_PROFILE        = 10/*,
        GET_FRIENDS         = 11,
        FIND_PLAYER         = 12,
        PLAYER_LIST         = 13,
        ADD_FRIEND          = 14,
        REMOVE_FRIEND       = 15,
        GET_FRIEND_ROOMS    = 16,
        FIND_ROOM           = 17,
        NEW_ROOM            = 18,
        ROOM_LIST           = 19,
        JOIN_ROOM           = 20,
        LEAVE_ROOM          = 21,
        PLAYER_JOINED       = 22,
        PLAYER_LEVED        = 23,
        CHAT                = 24*/
    }
}
