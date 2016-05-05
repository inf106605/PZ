using SkyCrab.Common_classes.Rooms;
using SkyCrabServer.Consoles;
using SkyCrabServer.Databases;
using SkyCrabServer.ServerClasses;
using System;
using System.Collections.Concurrent;

namespace SkyCrabServer
{
    static class Globals
    {

        //TODO thread synchronization of everything (read and write)

        public static ServerConsole serverConsole;

        public static Database database;

        public static ConcurrentDictionary<UInt32, ServerPlayer> players = new ConcurrentDictionary<UInt32, ServerPlayer>();

        public static readonly Sequence roomIdSequence = new Sequence();
        public static ConcurrentDictionary<UInt32, Room> rooms = new ConcurrentDictionary<UInt32, Room>();

    }
}
