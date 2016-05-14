namespace SkyCrab.Common_classes.Games.Letters
{
    public struct Letter
    {

        public readonly char character;
        public readonly uint points;


        public Letter(char character, uint points)
        {
            this.character = character;
            this.points = points;
        }

        public static bool operator ==(Letter letter1, Letter letter2)
        {
            return letter1.character == letter2.character;
            //TODO compare points?
        }

        public static bool operator !=(Letter letter1, Letter letter2)
        {
            return !(letter1 == letter2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(Letter))
                return false;
            return this == (Letter)obj;
        }

        public override int GetHashCode()
        {
            return (int)character * 111 + (int)points;
        }

    }

}
