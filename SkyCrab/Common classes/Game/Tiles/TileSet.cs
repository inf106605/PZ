using System;

namespace Common_classes.Game.Tiles
{
    public abstract class TileSet
    {

        private LetterInfo[] letters;


        public abstract LetterInfo[] Vowels { get; }

        public abstract LetterInfo[] Consonants { get; }

        public abstract LetterInfo[] Blanks { get; }

        public LetterInfo[] Letters
        {
            get
            {
                if (letters == null)
                {
                    letters = new LetterInfo[Vowels.Length + Consonants.Length + Blanks.Length];
                    Array.Copy(Vowels, 0, letters, 0, Vowels.Length);
                    Array.Copy(Consonants, 0, letters, Vowels.Length, Consonants.Length);
                    Array.Copy(Blanks, 0, letters, Vowels.Length + Consonants.Length, Blanks.Length);
                }
                return letters;
            }
        }

    }
}
