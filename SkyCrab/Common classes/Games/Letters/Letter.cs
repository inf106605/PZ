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

    }

}
