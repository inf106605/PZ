using SkyCrab.Common_classes.Games.Tiles;
using System.Collections.Generic;
using System.Linq;

namespace SkyCrab.Common_classes.Games.Racks
{
    public class NoSuchTileOnRackException : SkyCrabException
    {

        public NoSuchTileOnRackException() :
            base()
        {
        }

    }

    public class ToManyTileOnRackException : SkyCrabException
    {

        public ToManyTileOnRackException() :
            base()
        {
        }

    }

    public class TileAlreadyOnRackException : SkyCrabException
    {

        public TileAlreadyOnRackException() :
            base()
        {
        }

    }

    public class Rack
    {

        public const int IntendedTilesCount = 7;
        public const int MaxTilesCount = IntendedTilesCount + 1;
        public const float Size = MaxTilesCount * TileOnRack.Size;

        private LinkedList<TileOnRack> tiles = new LinkedList<TileOnRack>();

        
        public LinkedList<TileOnRack> Tiles
        {
            get { return tiles; }
        }


        public Rack()
        {
        }

        public TileOnRack GetTileOnPosition(float position)
        {
            foreach (TileOnRack tile in tiles)
                if (tile.IsOnPosition(position))
                    return tile;
            return null;
        }

        public int GetNumberOfTile(TileOnRack tileToFind)
        {
            int i = 0;
            foreach (TileOnRack tile in tiles)
            {
                if (ReferenceEquals(tile, tileToFind))
                    return i;
                ++i;
            }
            throw new NoSuchTileOnRackException();
        }

        public TileOnRack PutTile(Tile tile, float position = float.PositiveInfinity)
        {
            return PutTile(new Tile[] { tile }, position).First.Value;
        }

        public LinkedList<TileOnRack> PutTile(IList<Tile> tiles, float position = Size)
        {
            if (tiles.Count == 0)
                return new LinkedList<TileOnRack>();
            if (this.tiles.Count + tiles.Count > IntendedTilesCount)
                throw new ToManyTileOnRackException();
            foreach (Tile tileToPut in tiles)
                foreach (TileOnRack tile in this.tiles)
                    if (ReferenceEquals(tileToPut, tile.Tile))
                        throw new TileAlreadyOnRackException();

            if (TileOnRack.CenterToLeft(position) < 0.0f)
                position = TileOnRack.LeftToCenter(0.0f);
            else if (TileOnRack.CenterToRight(position) > Size)
                position = TileOnRack.RightToCenter(Size);
            int tilesOnLeft = 0, tilesOnRight = 0;
            foreach (TileOnRack tile in this.tiles)
                if (tile.IsMoreOnLeft(position))
                    ++tilesOnLeft;
                else
                    ++tilesOnRight;
            float left = position - (tiles.Count * TileOnRack.Size) / 2.0f;
            float right = position + (tiles.Count * TileOnRack.Size) / 2.0f;
            float leftBound = tilesOnLeft * TileOnRack.Size;
            float rightBound = Size - tilesOnRight * TileOnRack.Size;
            if (left < leftBound)
            {
                right -= leftBound - left;
                left = leftBound;
            }
            else if (right > rightBound)
            {
                left -= right - rightBound;
                right = rightBound;
            }

            if (tilesOnLeft != 0)
            {
                int rightestLeftIndex = tilesOnLeft - 1;
                TileOnRack rightestLeftTile = this.tiles.ElementAt(rightestLeftIndex);
                if (rightestLeftTile.RightPosition > left)
                    MoveTile(rightestLeftIndex, left - rightestLeftTile.RightPosition);
            }
            if (tilesOnRight != 0)
            {
                int leftestRightIndex = this.tiles.Count - tilesOnRight;
                TileOnRack leftestRightTile = this.tiles.ElementAt(leftestRightIndex);
                if (leftestRightTile.LeftPosition < right)
                    MoveTile(leftestRightIndex, right - leftestRightTile.LeftPosition);
            }

            var tileAfter = this.Tiles.First;
            for (int i = 0; i != tilesOnLeft; ++i)
                tileAfter = tileAfter.Next;
            float currentLeft = left;
            LinkedList<TileOnRack> tilesOnRack = new LinkedList<TileOnRack>();
            foreach (Tile tile in tiles)
            {
                TileOnRack tileOnRack = new TileOnRack(tile, currentLeft);
                if (tileAfter == null)
                    this.tiles.AddLast(tileOnRack);
                else
                    this.tiles.AddBefore(tileAfter, tileOnRack);
                currentLeft += TileOnRack.Size;
            }
            return tilesOnRack;
        }

        public void MoveTile(TileOnRack tile, float shift)
        {
            MoveTile(new TileOnRack[] { tile }, shift);
        }

        public void MoveTile(IList<TileOnRack> tiles, float shift)
        {
            if (tiles.Count == 0)
                return;

            int firstTileIndex = int.MaxValue;
            int lastTileIndex = int.MinValue;
            foreach (TileOnRack tileToMove in tiles)
            {
                int i = 0;
                foreach (TileOnRack tile in this.tiles)
                {
                    if (ReferenceEquals(tileToMove, tile))
                    {
                        if (i < firstTileIndex)
                            firstTileIndex = i;
                        if (i > lastTileIndex)
                            lastTileIndex = i;
                        goto next_tile;
                    }
                    ++i;
                }
                throw new NoSuchTileOnRackException();
                next_tile:;
            }

            MoveTile(firstTileIndex, lastTileIndex, shift);
        }

        public void MoveTile(int tileIndex, float shift)
        {
            MoveTile(tileIndex, tileIndex, shift);
        }

        public void MoveTile(int firstTileIndex, int lastTileIndex, float shift)
        {
            if (firstTileIndex > lastTileIndex)
            {
                int temp = firstTileIndex;
                firstTileIndex = lastTileIndex;
                lastTileIndex = temp;
            }
            if (firstTileIndex < 0)
                throw new NoSuchTileOnRackException();
            if (lastTileIndex >= tiles.Count)
                throw new NoSuchTileOnRackException();
            
            if (shift < 0.0f)
            {
                int tilesOnLeft = firstTileIndex;
                float leftBound = tilesOnLeft * TileOnRack.Size;
                TileOnRack firstTile = tiles.ElementAt(firstTileIndex);
                if (leftBound > firstTile.LeftPosition + shift)
                    shift = leftBound - firstTile.LeftPosition;
                
                var i = tiles.First;
                for (int j = 0; j != tilesOnLeft; ++j)
                    i = i.Next;
                float maxRight = i.Value.LeftPosition + shift;
                i = i.Previous;
                while (i != null)
                {
                    if (i.Value.RightPosition > maxRight)
                    {
                        i.Value.RightPosition = maxRight;
                        maxRight -= TileOnRack.Size;
                        i = i.Previous;
                    } else {
                        break;
                    }
                }
            } else
            {
                int tilesOnRight = tiles.Count - 1 - lastTileIndex;
                float rightBound = Size - tilesOnRight * TileOnRack.Size;
                TileOnRack lastTile = tiles.ElementAt(lastTileIndex);
                if (rightBound < lastTile.RightPosition + shift)
                    shift = rightBound - lastTile.RightPosition;

                var i = tiles.Last;
                for (int j = 0; j != tilesOnRight; ++j)
                    i = i.Previous;
                float minLeft = i.Value.RightPosition + shift;
                i = i.Next;
                while (i != null)
                {
                    if (i.Value.LeftPosition < minLeft)
                    {
                        i.Value.LeftPosition = minLeft;
                        minLeft += TileOnRack.Size;
                        i = i.Next;
                    }
                    else {
                        break;
                    }
                }
            }

            {
                var i = tiles.First;
                for (int j = 0; j != firstTileIndex; ++j)
                    i = i.Next;
                for (int j = firstTileIndex - 1; j != lastTileIndex; ++j)
                {
                    i.Value.Move(shift);
                    i = i.Next;
                }
            }
        }

        public void TakeOff(TileOnRack tile)
        {
            TakeOff(new TileOnRack[] { tile });
        }

        public void TakeOff(IEnumerable<TileOnRack> tiles)
        {
            foreach (TileOnRack tileToTakeOff in tiles)
            {
                var i = this.tiles.First;
                while (i != null)
                {
                    if (ReferenceEquals(i.Value, tileToTakeOff))
                    {
                        this.tiles.Remove(i);
                        goto next_tile;
                    }
                    i = i.Next;
                }
                throw new NoSuchTileOnRackException();
                next_tile:;
            }
        }

    }
}
