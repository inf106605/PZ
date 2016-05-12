using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.ScrabbleGameFolder
{
    class ScrabbleTilesSelectedFromBoard
    {
        ObservableCollection<ScrabbleSquare> scrabbleTilesBoardSelected = null;

        public ScrabbleTilesSelectedFromBoard()
        {
            scrabbleTilesBoardSelected = new ObservableCollection<ScrabbleSquare>();
        }

        public ObservableCollection<ScrabbleSquare> scrabbleTilesSelectedFromBoard
        {
            get
            {
                return scrabbleTilesBoardSelected;
            }
        }

        public void addToScrabbleTilesSelectedFromBoard(ScrabbleSquare scrabbleTileBoard)
        {
            scrabbleTilesBoardSelected.Add(scrabbleTileBoard);
        }
    }
}
