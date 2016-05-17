namespace SkyCrab.Common_classes.Games.Boards
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

        public static bool operator ==(PositionOnBoard position1, PositionOnBoard position2)
        {
            return position1.x == position2.x &&
                position1.y == position2.y;
        }

        public static bool operator !=(PositionOnBoard position1, PositionOnBoard position2)
        {
            return !(position1 == position2);
        }

        public bool Equals(PositionOnBoard position)
        {
            return this == position;
        }

        public override int GetHashCode()
        {
            return x * 111 + y;
        }

    }
	
}
