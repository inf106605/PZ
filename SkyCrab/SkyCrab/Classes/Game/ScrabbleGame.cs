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
        
        public ScrabbleGame()
        {
            scrabbleBoard = new ScrabbleBoard(); 
            scrabbleRack = new ScrabbleRack();
        }

        public List<ScrabbleSquare> Squares {
            get
            {
                return scrabbleBoard.Squares;   
            }
        }

        public ObservableCollection<ScrabbleRackTiles> RackTiles
        {
            get
            {
                return ScrabbleRack.RackTiles;
            }
        }

    }
}
