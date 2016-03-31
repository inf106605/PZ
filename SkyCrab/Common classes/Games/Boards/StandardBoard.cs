﻿using SkyCrab.Common_classes.Games.Tiles;
using System.Collections.Generic;

namespace SkyCrab.Common_classes.Games.Boards
{
    class StandardBoard : Board
    {

        private static readonly List<PositionOnBoard> squares;

        private static readonly SquareType[][] squareTypes = new SquareType[8][]
                {
					new SquareType[8] { SquareType.word3,   SquareType.normal,  SquareType.normal,  SquareType.letter2, SquareType.normal,  SquareType.normal,  SquareType.normal,  SquareType.word3 },
					new SquareType[8] { SquareType.normal,  SquareType.word2,   SquareType.normal,  SquareType.normal,  SquareType.normal,  SquareType.letter3, SquareType.normal,  SquareType.normal },
					new SquareType[8] { SquareType.normal,  SquareType.normal,  SquareType.word2,   SquareType.normal,  SquareType.normal,  SquareType.normal,  SquareType.letter2, SquareType.normal },
					new SquareType[8] { SquareType.letter2, SquareType.normal,  SquareType.normal,  SquareType.word2,   SquareType.normal,  SquareType.normal,  SquareType.normal,  SquareType.letter2 },
					new SquareType[8] { SquareType.normal,  SquareType.normal,  SquareType.normal,  SquareType.normal,  SquareType.word2,   SquareType.normal,  SquareType.normal,  SquareType.normal },
					new SquareType[8] { SquareType.normal,  SquareType.letter3, SquareType.normal,  SquareType.normal,  SquareType.normal,  SquareType.letter3, SquareType.normal,  SquareType.normal },
					new SquareType[8] { SquareType.normal,  SquareType.normal,  SquareType.letter2, SquareType.normal,  SquareType.normal,  SquareType.normal,  SquareType.letter2, SquareType.normal },
					new SquareType[8] { SquareType.word3,   SquareType.normal,  SquareType.normal,  SquareType.letter2, SquareType.normal,  SquareType.normal,  SquareType.normal,  SquareType.start }
				};

        private Tile[][] tiles;


        static StandardBoard()
        {
            squares = new List<PositionOnBoard>(15 * 15);
            for (int i = 0; i != 15; ++i)
                for (int j = 0; j != 15; ++j)
                    squares.Add(new PositionOnBoard(i, j));
        }


        public override bool Rectangle
        {
            get
            {
                return true;
            }
        }

        public override PositionOnBoard LeftTop
        {
            get
            {
                return new PositionOnBoard(0, 0);
            }
        }
        public override PositionOnBoard RightBottom
        {
            get
            {
                return new PositionOnBoard(14, 14);
            }
        }

        public override IList<PositionOnBoard> Squares
        {
            get
            {
                return squares;
            }
        }

        public override PositionOnBoard StartSquare
        {
            get
            {
                return new PositionOnBoard(7, 7);
            }
        }
        

        public StandardBoard()
        {
            tiles = new Tile[15][];
            for (uint i = 0; i != 15; ++i)
                tiles[i] = new Tile[15];
        }

        public override bool IsSquare(PositionOnBoard position)
        {
            return position.x >= 0 && position.x < 15 &&
                    position.y >= 0 && position.y < 15;
        }

        public override SquareType GetQuareType(PositionOnBoard position)
        {
            if (!IsSquare(position))
                throw new NoSuchSquareOnBoardException();
            int x = (position.x <= 7) ? position.x : (14 - position.x);
            int y = (position.y <= 7) ? position.y : (14 - position.y);
            SquareType squareType = squareTypes[x][y];
            return squareType;
        }

        public override void PutTile(Tile tile, PositionOnBoard position)
        {
            if (GetTile(position) != null)
                throw new SquareOnBoardIsOccupiedException();
            tiles[position.x][position.y] = tile;
        }

        public override Tile GetTile(PositionOnBoard position)
        {
            if (!IsSquare(position))
                throw new NoSuchSquareOnBoardException();
            Tile tile = tiles[position.x][position.y];
            return tile;
        }

        public override string getSquareID(PositionOnBoard position, bool horizontal)
        {
            return getSquareStandardID(position, horizontal);
        }

    }
}
