using SkyCrab.Common_classes.Games.Racks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Game
{
    class ScrabbleTilesSelectedFromRack
    {

        ObservableCollection<ScrabbleRackTiles> ScrabbleTilesSelectedFromRacks = null;

        public ObservableCollection<ScrabbleRackTiles> scrabbleTilesSelectedFromRack
        {
            get
            {
                return ScrabbleTilesSelectedFromRacks;
            }
        }

        void addToScrabbleTilesSelectedFromRack(ScrabbleRackTiles scrabbleRackTile)
        {
            ScrabbleTilesSelectedFromRacks.Add(scrabbleRackTile);
        }

        public ScrabbleTilesSelectedFromRack()
        {
            ScrabbleTilesSelectedFromRacks = new ObservableCollection<ScrabbleRackTiles>();
        }
    }
}
