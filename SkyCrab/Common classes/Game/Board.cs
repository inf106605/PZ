using System.Collections.Generic;

namespace Common_classes.Game
{
    public struct PositionOnBoard
    {
        public int x;
        public int y;

        public PositionOnBoard(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public enum SquareType
    {
        normal = 0,
        start = 1,
        letter2 = 2,
        letter3 = 3,
        word2 = 4,
        word3 = 5
    }

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

        public abstract SquareType GetQuareType(PositionOnBoard position);

        public abstract void PutTile(Tile tile, PositionOnBoard position);

        public abstract Tile GetTile(PositionOnBoard position);

        public abstract string getSquareID(PositionOnBoard position, bool horizontal);

        protected static string getStandardSquareID(PositionOnBoard position, bool horizontal)
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
