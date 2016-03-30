using System;

namespace SkyCrab.Common_classes.Games.Tiles
{
    public abstract class TileSet
    {

        private LetterCount[] letters;


        public abstract LetterCount[] Vowels { get; }

        public abstract LetterCount[] Consonants { get; }

        public abstract LetterCount[] Blanks { get; }

        public LetterCount[] Letters
        {
            get
            {
                if (letters == null)
                {
                    letters = new LetterCount[Vowels.Length + Consonants.Length + Blanks.Length];
                    Array.Copy(Vowels, 0, letters, 0, Vowels.Length);
                    Array.Copy(Consonants, 0, letters, Vowels.Length, Consonants.Length);
                    Array.Copy(Blanks, 0, letters, Vowels.Length + Consonants.Length, Blanks.Length);
                }
                return letters;
            }
        }

    }
}
