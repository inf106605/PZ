using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Pouch;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Game
{
    class ScrabbleGame 
    {
        public ScrabbleBoard scrabbleBoard;
        public ScrabbleRack scrabbleRack;
        public Pouch pouch;

        public ScrabbleGame()
        {
            scrabbleBoard = new ScrabbleBoard(); 
            scrabbleRack = new ScrabbleRack();
            scrabbleRack = new ScrabbleRack();
            pouch = new Pouch(0,PolishTileSet.instance.Letters);
            for(int i=0; i < 7;i++)
            {
                TileOnRack temp = new TileOnRack(pouch.DrawRandowmTile());
                RackTiles.Add(new ScrabbleRackTiles(temp));
            }
        }

       public ObservableCollection<ScrabbleSquare> Squares {
            get
            {
                return scrabbleBoard.Squares;   
            }
        }

        public ObservableCollection<ScrabbleRackTiles> RackTiles
        {
            get
            {
                return scrabbleRack.RackTiles;
            }
        }

    }
}
