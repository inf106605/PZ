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
        LOGIN_AS_GUEST      = 7,
        LOGIN               = 8,
        LOGIN_OK            = 9,
        LOGOUT              = 10,
        REGISTER            = 11,
        EDIT_PROFILE        = 12,
        //Friends
        GET_FRIENDS         = 13,
        FIND_PLAYERS        = 14,
        PLAYER_LIST         = 15,
        ADD_FRIEND          = 16,
        REMOVE_FRIEND       = 17,
        //Rooms
        GET_FRIEND_ROOMS    = 18,
        FIND_ROOMS          = 19,
        ROOM_LIST           = 20,
        CREATE_ROOM         = 21,
        ROOM                = 22,
        JOIN_ROOM           = 23,
        LEAVE_ROOM          = 24,
        PLAYER_JOINED       = 25,
        PLAYER_LEAVED       = 26,
        PLAYER_READY        = 27,
        PLAYER_NOT_READY    = 28,
        NEW_ROOM_OWNER      = 29,
        CHAT                = 30,

        //--- Game ---
        //Commands
        PLACE_TILES         = 31,
        EXCHANGE_TILES      = 32,
        PASS                = 33,
        //Informations
        GAME_STARTED        = 34,
        GAME_ENDED          = 35,
        NEXT_TURN           = 36,
        NEW_TILES           = 37,
        LOSS_TILES          = 38,
        POINTS_CHANGED      = 39,
        REORDER_RACK_TILES  = 40,
        PLAYER_FAILED       = 41,
        PLAYER_PLACED_TILES = 42,
        PLAYER_EXCHAN_TILES = 43,
        PLAYER_PASSED       = 44,
        TIMEOUT_OCCURRED    = 45
    }
}
