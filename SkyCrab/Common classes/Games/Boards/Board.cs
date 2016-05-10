using SkyCrab.Common_classes.Games.Tiles;
using System.Collections.Generic;

namespace SkyCrab.Common_classes.Games.Boards
{
    public class NoSuchSquareOnBoardException : SkyCrabException
    {

        public NoSuchSquareOnBoardException() :
            base()
        {
        }

    }

    public class SquareOnBoardIsOccupiedException : SkyCrabException
    {

        public SquareOnBoardIsOccupiedException() :
            base()
        {
        }

    }

    public abstract class Board
    {

        public abstract bool Rectangle { get; }

        public abstract PositionOnBoard LeftTop { get; }
        public abstract PositionOnBoard RightBottom { get; }

        public abstract IList<PositionOnBoard> Squares { get; }

        public abstract PositionOnBoard StartSquare { get; }


        public abstract bool IsSquare(PositionOnBoard position);

        public abstract SquareType GetSquareType(PositionOnBoard position);

        public void PutTile(TileOnBoard tileOnBoard)
        {
            PutTile(tileOnBoard.tile, tileOnBoard.position);
        }

        public abstract void PutTile(Tile tile, PositionOnBoard position);

        public abstract Tile GetTile(PositionOnBoard position);

        public abstract string getSquareID(PositionOnBoard position, bool horizontal);

        protected static string getSquareStandardID(PositionOnBoard position, bool horizontal)
        {
            string number = (position.x + 1) + "";
            char letter = (char) (((int) 'A') + position.y);
            if (horizontal)
                return letter + number;
            else
                return number + letter;
        }

    }
}
