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
        //Games
        GET_GAME_LOG        = 31,
        GAME_LOG            = 32,

        //--- Game ---
        //Commands
        PLACE_TILES         = 33,
        EXCHANGE_TILES      = 34,
        PASS                = 35,
        //Informations
        GAME_STARTED        = 36,
        GAME_ENDED          = 37,
        NEXT_TURN           = 38,
        NEW_TILES           = 39,
        LOSS_TILES          = 40,
        POINTS_CHANGED      = 41,
        REORDER_RACK_TILES  = 42,
        PLAYER_FAILED       = 43,
        PLAYER_PLACED_TILES = 44,
        PLAYER_EXCHAN_TILES = 45,
        PLAYER_PASSED       = 46,
        TIMEOUT_OCCURRED    = 47
    }
}
