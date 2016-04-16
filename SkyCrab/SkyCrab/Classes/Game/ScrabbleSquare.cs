using SkyCrab.Common_classes.Games.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Game
{
    public class ScrabbleSquare
    {

        static StandardBoard board = new StandardBoard();
        private SquareType squareType;

        public ScrabbleSquare(int Row, int Column)
        {
            this.Row = Row;
            this.Column = Column;
            this.squareType = board.GetSquareType(new PositionOnBoard(Row, Column));
        }


        public int Row { get; set; }

        public int Column { get; set; }

        public bool IsDarkBlue
        {
            get
            {
                return squareType == SquareType.letter3;
            }
        }

        public bool IsRed
        {
            get
            {
                return squareType == SquareType.word3;
            }
        }

        public bool IsOrange
        {
            get
            {
                return squareType == SquareType.word2 || squareType == SquareType.start;
            }
        }

        public bool IsBlue
        {
            get
            {

                return squareType == SquareType.letter2;

            }
        }

        public bool IsStart
        {
            get
            {

                return squareType == SquareType.word3;

            }
        }

        public bool IsGreen
        {
            get
            {

                return squareType == SquareType.normal;
            }
        }

    }
}
