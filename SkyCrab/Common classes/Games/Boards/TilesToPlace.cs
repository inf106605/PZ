using SkyCrab.Common_classes.Games.Racks;
using System;
using System.Collections.Generic;

namespace SkyCrab.Common_classes.Games.Boards
{
    public struct TilesToPlace
    {

        public UInt32 playerId;
        public List<LetterWithNumber> lettersFromRack;
        public List<TileOnBoard> tilesToPlace;

        public override string ToString()
        {
            return typeof(TilesToPlace).Name + " (player: " + playerId + ", " + lettersFromRack.Count + ", " + tilesToPlace.Count + ")";
        }

    }
}
