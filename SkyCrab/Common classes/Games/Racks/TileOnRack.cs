using SkyCrab.Common_classes.Games.Tiles;

namespace SkyCrab.Common_classes.Games.Racks
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
        private float leftPosition;


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
                return leftPosition;
            }
            set
            {
                if (value < 0.0f || LeftToRight(value) > Rack.Size)
                    throw new TileOnRackPositionException();
                leftPosition = value;
            }
        }

        public float RightPosition
        {
            get
            {
                return LeftToRight(leftPosition);
            }
            set
            {
                if (RightToLeft(value) < 0.0f || value > Rack.Size)
                    throw new TileOnRackPositionException();
                leftPosition = RightToLeft(value);
            }
        }

        public float CenterPosition
        {
            get
            {
                return LeftToCenter(leftPosition);
            }
            set
            {
                if (CenterToLeft(value) < 0.0f || CenterToRight(value) > Rack.Size)
                    throw new TileOnRackPositionException();
                leftPosition = CenterToLeft(value);
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


        public TileOnRack(Tile tile, float leftPosition = 0.0f)
        {
            this.tile = tile;
            this.leftPosition = leftPosition;
        }

        public bool IsOnPosition(float leftPosition)
        {
            return (LeftPosition <= leftPosition) && (RightPosition >= leftPosition);
        }

        public void Move(float shift)
        {
            if (LeftPosition + shift < 0.0f || RightPosition + shift > Rack.Size)
                throw new TileOnRackPositionException();
            leftPosition += shift;
        }

        public bool IsMoreOnLeft(float position)
        {
            return CenterPosition < position;
        }

    }
}
