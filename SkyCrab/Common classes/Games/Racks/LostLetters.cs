using System;
using System.Collections.Generic;

namespace SkyCrab.Common_classes.Games.Racks
{
    public struct LostLetters
    {
        public UInt32 playerId;
        public List<TileWithNumber> letters;
    }
}
