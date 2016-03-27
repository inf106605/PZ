using System.Collections.Generic;

namespace Common_classes.Game
{
    public struct Letter
    {

        char character;

        uint points;

        public Letter(char character, uint points)
        {
            this.character = character;
            this.points = points;
        }

    }

    public abstract class LetterSet
    {

        public static readonly Letter BLANK = new Letter('\0', 0);


        public abstract IList<Letter> Letters { get; }

    }

}
