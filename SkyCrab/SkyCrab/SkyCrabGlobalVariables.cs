﻿using SkyCrab.Common_classes.Chats;
using SkyCrab.Common_classes.Players;
using SkyCrab.SkyCrabClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab
{
    static class SkyCrabGlobalVariables
    {
       public static Player player;
       public static SkyCrabRoom room; // pokój z uzupełnionymi danymi od serwera , wykorzystywany m.in. przez lobby
        public static ChatMessage chatMessages;
        public static String MessagesLog = "";
        public static bool isGame = false;
        public static readonly object roomLock = new object();
    }
}
