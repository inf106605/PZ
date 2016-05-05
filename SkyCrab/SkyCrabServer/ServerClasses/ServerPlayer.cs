using SkyCrab.Common_classes.Players;
using SkyCrab.Common_classes.Rooms;
using SkyCrabServer.Connactions;

namespace SkyCrabServer.ServerClasses
{
    sealed class ServerPlayer
    {

        public readonly ServerConnection connection;
        public readonly Player player;
        public Room room;

        public ServerPlayer(ServerConnection connection, Player player)
        {
            this.connection = connection;
            this.player = player;
        }

    }
}
