using SkyCrab.Common_classes.Players;
using SkyCrabServer.Consoles;
using SkyCrabServer.Databases;
using System;
using System.Collections.Concurrent;

namespace SkyCrabServer
{
    static class Globals
    {

        public static ServerConsole serverConsole;

        public static Database database;

        public static ConcurrentDictionary<UInt32, Player> players = new ConcurrentDictionary<UInt32, Player>();

    }
}
