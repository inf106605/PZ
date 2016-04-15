using SkyCrab.Common_classes.Games.Letters;

namespace SkyCrab.Common_classes.Games.Tiles
{
    class PolishTileSet : TileSet
    {

        private static readonly LetterCount[] vovels = new LetterCount[9]
                {
                    new LetterCount(PolishLetterSet.A,        9),
                    new LetterCount(PolishLetterSet.A_OGONEK, 1),
                    new LetterCount(PolishLetterSet.E,        7),
                    new LetterCount(PolishLetterSet.E_OGONEK, 1),
                    new LetterCount(PolishLetterSet.I,        8),
                    new LetterCount(PolishLetterSet.O,        6),
                    new LetterCount(PolishLetterSet.O_ACUTE,  1),
                    new LetterCount(PolishLetterSet.U,        2),
                    new LetterCount(PolishLetterSet.Y,        4)
                };
        private static readonly LetterCount[] consonants = new LetterCount[23]
                {
                    new LetterCount(PolishLetterSet.B,        2),
                    new LetterCount(PolishLetterSet.C,        3),
                    new LetterCount(PolishLetterSet.C_ACUTE,  1),
                    new LetterCount(PolishLetterSet.D,        3),
                    new LetterCount(PolishLetterSet.F,        1),
                    new LetterCount(PolishLetterSet.G,        2),
                    new LetterCount(PolishLetterSet.H,        2),
                    new LetterCount(PolishLetterSet.J,        2),
                    new LetterCount(PolishLetterSet.K,        3),
                    new LetterCount(PolishLetterSet.L,        3),
                    new LetterCount(PolishLetterSet.L_STROKE, 2),
                    new LetterCount(PolishLetterSet.M,        3),
                    new LetterCount(PolishLetterSet.N,        5),
                    new LetterCount(PolishLetterSet.N_ACUTE,  1),
                    new LetterCount(PolishLetterSet.P,        3),
                    new LetterCount(PolishLetterSet.R,        4),
                    new LetterCount(PolishLetterSet.S,        4),
                    new LetterCount(PolishLetterSet.S_ACUTE,  1),
                    new LetterCount(PolishLetterSet.T,        3),
                    new LetterCount(PolishLetterSet.W,        4),
                    new LetterCount(PolishLetterSet.Z,        5),
                    new LetterCount(PolishLetterSet.Z_ACUTE,  1),
                    new LetterCount(PolishLetterSet.Z_DOT,    1)
                };
        private static readonly LetterCount[] blanks = new LetterCount[1]
                {
                    new LetterCount(PolishLetterSet.BLANK,    2)
                };

        public static readonly PolishTileSet instance = new PolishTileSet();


        public override LetterCount[] Vowels
        {
            get { return vovels; }
        }

        public override LetterCount[] Consonants
        {
            get { return consonants; }
        }

        public override LetterCount[] Blanks
        {
            get { return blanks; }
        }


        private PolishTileSet()
        {
        }

    }
}
