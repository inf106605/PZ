using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Common_classes.Games.Racks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SkyCrab.Classes.Game
{
    public class ScrabbleBoard
    {
        static public ObservableCollection<ScrabbleSquare> Squares { get; private set; }

        public ScrabbleBoard()
        {
            Squares = new ObservableCollection<ScrabbleSquare>();

            StandardBoard standardBoard = new StandardBoard();

            foreach(var square in standardBoard.Squares)
            {
                Squares.Add(new ScrabbleSquare(square.x, square.y));
            }
        }
    }
}
