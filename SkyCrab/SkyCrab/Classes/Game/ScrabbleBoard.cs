using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Common_classes.Games.Racks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SkyCrab.Classes.Game
{
    public class ScrabbleBoard
    {
        public List<ScrabbleSquare> Squares { get; private set; }

        public ScrabbleBoard()
        {
            Squares = new List<ScrabbleSquare>();

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Squares.Add(new ScrabbleSquare(i,j));
                }
            }
        }
    }
}
