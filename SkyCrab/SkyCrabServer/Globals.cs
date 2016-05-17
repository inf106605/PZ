using SkyCrab.Dictionaries;
using SkyCrabServer.Consoles;
using SkyCrabServer.Databases;
using SkyCrabServer.ServerClasses;
using SkyCrabServer.ServerLogics;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SkyCrabServer
{
    static class Globals
    {

        //TODO thread synchronization of everything (read and write)

        public static ServerConsole serverConsole;

        public static Database database;

        public static readonly ReaderWriterLock dataLock = new ReaderWriterLock();

        public static readonly ConcurrentDictionary<UInt32, ServerPlayer> players = new ConcurrentDictionary<UInt32, ServerPlayer>();

        public static readonly Sequence roomIdSequence = new Sequence();
        public static readonly ConcurrentDictionary<UInt32, ServerRoom> rooms = new ConcurrentDictionary<UInt32, ServerRoom>();

        public static Dictionary dictionary;

    }
}
