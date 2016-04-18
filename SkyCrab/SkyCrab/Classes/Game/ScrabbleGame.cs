using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Game
{
    class ScrabbleGame
    {
        public ScrabbleBoard scrabbleBoard;

        public Command<ScrabbleSquare> SquareClickCommand { get; private set; }

        public ScrabbleGame()
        {
            scrabbleBoard = new ScrabbleBoard(); 
            SquareClickCommand = new Command<ScrabbleSquare>(OnSquareClick);
        }

        public List<ScrabbleSquare> Squares {
            get
            {
                return scrabbleBoard.Squares;   
            }
        }

        private void OnSquareClick(ScrabbleSquare square)
        {
            System.Windows.MessageBox.Show("You clicked on Row: " + square.Row + " - Column: " + square.Column);

        }
    }
}
