using SkyCrab.Common_classes.Players;
using SkyCrab.Common_classes.Rooms;

namespace SkyCrabServer.ServerClasses
{
    sealed class ServerPlayer
    {

        public readonly Player player;
        public Room room;

        public ServerPlayer(Player player)
        {
            this.player = player;
        }

    }
}
