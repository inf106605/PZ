using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Game
{
    class ScrabbleRackTiles
    {
        static int counter;
        public int Id { get; set; }
        public TileOnRack tile { get; set; }

        public string Name
        {
            get
            {
                return "" + tile.Tile.Letter.character;
            }
        }

        public string Value
        {
            get
            {
                return  tile.Tile.Letter.points.ToString();
            }
        }

        public ScrabbleRackTiles()
        {
        }

        public ScrabbleRackTiles(TileOnRack tile)
        {
            this.Id = counter++;
            this.tile = tile;
        }

    }
}
