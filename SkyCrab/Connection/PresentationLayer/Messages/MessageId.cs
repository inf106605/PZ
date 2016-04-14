namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public enum MessageId : byte
    {
        OK                  = 0,
        ERROR               = 1,
        LOGIN               = 2,
        LOGIN_OK            = 3,
        LOGOUT              = 4,
        REGISTER            = 5,
        EDIT_PROFILE        = 6/*,
        GET_FRIENDS         = 7,
        FIND_PLAYER         = 8,
        PLAYER_LIST         = 9,
        ADD_FRIEND          = 10,
        REMOVE_FRIEND       = 11,
        GET_FRIEND_ROOMS    = 12,
        FIND_ROOM           = 13,
        NEW_ROOM            = 14,
        ROOM_LIST           = 15,
        JOIN_ROOM           = 16,
        LEAVE_ROOM          = 17,
        PLAYER_JOINED       = 18,
        PLAYER_LEVED        = 19,
        CHAT                = 20*/
    }
}
