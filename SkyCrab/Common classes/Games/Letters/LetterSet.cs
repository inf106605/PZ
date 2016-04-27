using System.Collections.Generic;

namespace SkyCrab.Common_classes.Games.Letters
{
    public abstract class LetterSet
    {

        public static readonly Letter BLANK = new Letter(' ', 0);


        public abstract IList<Letter> Letters { get; }

    }

}
