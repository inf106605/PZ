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

        public override bool Equals(object position)
        {
            if (position == null)
                return false;
            if (position.GetType() != typeof(PositionOnBoard))
                return false;
            return this == (PositionOnBoard)position;
        }

        public override int GetHashCode()
        {
            return x * 111 + y;
        }

    }
	
}
