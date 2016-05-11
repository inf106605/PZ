using SkyCrab.Common_classes.Games.Racks;
using System.Collections.Generic;

namespace SkyCrab.Common_classes.Games.Boards
{
    public struct TilesToPlace
    {
        public List<TileWithNumber> tilesFromRack;
        public List<TileOnBoard> tilesToPlace;
    }
}
