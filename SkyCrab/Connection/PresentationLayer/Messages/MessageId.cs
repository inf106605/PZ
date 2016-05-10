namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public enum MessageId : byte
    {
        //--- Common ---
        //Connections
        DISCONNECT          = 0,
        SHUTDOWN            = 1,
        //Pings
        PING                = 2,
        PONG                = 3,
        NO_PONG             = 4,
        //Errors
        OK                  = 5,
        ERROR               = 6,

        //--- Menu ---
        //Accounts
        LOGIN               = 7,
        LOGIN_OK            = 8,
        LOGOUT              = 9,
        REGISTER            = 10,
        EDIT_PROFILE        = 11,
        //Friends
        GET_FRIENDS         = 12,
        FIND_PLAYERS        = 13,
        PLAYER_LIST         = 14,
        ADD_FRIEND          = 15,
        REMOVE_FRIEND       = 16,
        //Rooms
        GET_FRIEND_ROOMS    = 17,
        FIND_ROOMS          = 18,
        ROOM_LIST           = 19,
        CREATE_ROOM         = 20,
        ROOM                = 21,
        //InRooms
        JOIN_ROOM           = 22,
        LEAVE_ROOM          = 23,
        PLAYER_JOINED       = 24,
        PLAYER_LEAVED       = 25,
        PLAYER_READY        = 26,
        PLAYER_NOT_READY    = 27,
        NEW_ROOM_OWNER      = 28,
        CHAT                = 29,
        
        //--- Game ---
        GAME_STARTED        = 30,
        NEXT_TURN           = 31,
        NEW_TILES           = 32,
        LOSS_TILES          = 33,
        GAIN_POINTS         = 34,
        REORDER_RACK_TILES  = 35,
        TURN_TIMEOUT        = 36,
        PLACE_TILES         = 37,
        EXCHANGE_TILES      = 38,
        PASS                = 39,
        GAME_ENDED          = 40
    }
}
