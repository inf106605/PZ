using System;

namespace Common_classes.Game
{
    public class TileOnRackPositionException : SkyCrabException
    {

        public TileOnRackPositionException() :
            base()
        {
        }

    }

    public class TileOnRack
    {

        public const float Size = 1.0f;

        private Tile tile;
        private float position;


        public Tile Tile
        {
            get
            {
                return tile;
            }
        }

        public float LeftPosition
        {
            get
            {
                return position;
            }
            set
            {
                if (value < 0.0f || LeftToRight(value) > Rack.Size)
                    throw new TileOnRackPositionException();
                position = value;
            }
        }

        public float RightPosition
        {
            get
            {
                return LeftToRight(position);
            }
            set
            {
                if (RightToLeft(value) < 0.0f || value > Rack.Size)
                    throw new TileOnRackPositionException();
                position = RightToLeft(value);
            }
        }

        public float CenterPosition
        {
            get
            {
                return LeftToCenter(position);
            }
            set
            {
                if (CenterToLeft(value) < 0.0f || CenterToRight(value) > Rack.Size)
                    throw new TileOnRackPositionException();
                position = CenterToLeft(value);
            }
        }


        public static float LeftToCenter(float leftPosition)
        {
            return leftPosition + Size / 2.0f;
        }

        public static float LeftToRight(float leftPosition)
        {
            return leftPosition + Size;
        }

        public static float CenterToLeft(float centerPosition)
        {
            return centerPosition - Size / 2.0f;
        }

        public static float CenterToRight(float centerPosition)
        {
            return centerPosition + Size / 2.0f;
        }

        public static float RightToCenter(float rightPosition)
        {
            return rightPosition - Size / 2.0f;
        }

        public static float RightToLeft(float rightPosition)
        {
            return rightPosition - Size;
        }


        public TileOnRack(Tile tile, float position = 0.0f)
        {
            this.tile = tile;
            this.position = position;
        }

        public bool IsOnPosition(float position)
        {
            return (LeftPosition <= position) && (RightPosition >= position);
        }

        public void Move(float shift)
        {
            if (LeftPosition + shift < 0.0f || RightPosition + shift > Rack.Size)
                throw new TileOnRackPositionException();
            position += shift;
        }

        public bool IsMoreOnLeft(float position)
        {
            return CenterPosition > position;
        }

    }
}
