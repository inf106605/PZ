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
        private ObservableCollection<ScrabbleSquare> squares;

        public ObservableCollection<ScrabbleSquare> Squares
        {
            get
            {
                return squares;
            }
        }

        public void SetScrabbleSquare(int PositionListBox, int Columns, int Rows, string NameTiles, int ValuesTiles)
        {
            squares[PositionListBox] = new ScrabbleSquare(Columns, Rows, NameTiles, ValuesTiles);
        }

        public ScrabbleBoard()
        {
            squares = new ObservableCollection<ScrabbleSquare>();

            StandardBoard standardBoard = new StandardBoard();

            foreach(var square in standardBoard.Squares)
            {
                Squares.Add(new ScrabbleSquare(square.x, square.y));
            }
        }
    }
}
