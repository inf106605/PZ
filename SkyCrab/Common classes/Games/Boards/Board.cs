using SkyCrab.Common_classes.Games.Tiles;
using System.Collections.Generic;
using System;

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

    public abstract class Board : ICloneable
    {

        public abstract bool Rectangle { get; }

        public abstract PositionOnBoard LeftTop { get; }
        public abstract PositionOnBoard RightBottom { get; }

        public abstract IList<PositionOnBoard> Squares { get; }

        public abstract PositionOnBoard StartSquare { get; }

        public abstract uint Count { get; }

        public abstract bool IsEmpty { get; }


        public abstract bool IsSquare(PositionOnBoard position);

        public abstract SquareType GetSquareType(PositionOnBoard position);

        public void PutTile(TileOnBoard tileOnBoard)
        {
            PutTile(tileOnBoard.tile, tileOnBoard.position);
        }

        public abstract void PutTile(Tile tile, PositionOnBoard position);

        public TileOnBoard GetTileOnBoard(PositionOnBoard position)
        {
            TileOnBoard tileOnBoard = new TileOnBoard();
            tileOnBoard.tile = GetTile(position);
            if (tileOnBoard.tile == null)
                return null;
            tileOnBoard.position = position;
            return tileOnBoard;
        }

        public abstract Tile GetTile(PositionOnBoard position);

        public string getSquareID(WordOnBoard wordOnBoard)
        {
            return getSquareID(wordOnBoard.position, wordOnBoard.horizonatal);
        }

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

        public abstract object Clone();

    }
}
