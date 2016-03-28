using Common_classes.Game.Letters;

namespace Common_classes.Game.Tiles
{
    class PolishTileSet : TileSet
    {

        private static readonly LetterInfo[] vovels = new LetterInfo[9]
                {
                    new LetterInfo(PolishLetterSet.A,        9),
                    new LetterInfo(PolishLetterSet.A_OGONEK, 1),
                    new LetterInfo(PolishLetterSet.E,        7),
                    new LetterInfo(PolishLetterSet.E_OGONEK, 1),
                    new LetterInfo(PolishLetterSet.I,        8),
                    new LetterInfo(PolishLetterSet.O,        6),
                    new LetterInfo(PolishLetterSet.O_ACUTE,  1),
                    new LetterInfo(PolishLetterSet.U,        2),
                    new LetterInfo(PolishLetterSet.Y,        4)
                };
        private static readonly LetterInfo[] consonants = new LetterInfo[23]
                {
                    new LetterInfo(PolishLetterSet.B,        2),
                    new LetterInfo(PolishLetterSet.C,        3),
                    new LetterInfo(PolishLetterSet.C_ACUTE,  1),
                    new LetterInfo(PolishLetterSet.D,        3),
                    new LetterInfo(PolishLetterSet.F,        1),
                    new LetterInfo(PolishLetterSet.G,        2),
                    new LetterInfo(PolishLetterSet.H,        2),
                    new LetterInfo(PolishLetterSet.J,        2),
                    new LetterInfo(PolishLetterSet.K,        3),
                    new LetterInfo(PolishLetterSet.L,        3),
                    new LetterInfo(PolishLetterSet.L_STROKE, 2),
                    new LetterInfo(PolishLetterSet.M,        3),
                    new LetterInfo(PolishLetterSet.N,        5),
                    new LetterInfo(PolishLetterSet.N_ACUTE,  1),
                    new LetterInfo(PolishLetterSet.P,        3),
                    new LetterInfo(PolishLetterSet.R,        4),
                    new LetterInfo(PolishLetterSet.S,        4),
                    new LetterInfo(PolishLetterSet.S_ACUTE,  1),
                    new LetterInfo(PolishLetterSet.T,        3),
                    new LetterInfo(PolishLetterSet.W,        4),
                    new LetterInfo(PolishLetterSet.Z,        5),
                    new LetterInfo(PolishLetterSet.Z_ACUTE,  1),
                    new LetterInfo(PolishLetterSet.Z_DOT,    1)
                };
        private static readonly LetterInfo[] blanks = new LetterInfo[1]
                {
                    new LetterInfo(PolishLetterSet.BLANK,    2)
                };
        

        public override LetterInfo[] Vowels
        {
            get
            {
                return vovels;
            }
        }

        public override LetterInfo[] Consonants
        {
            get
            {
                return consonants;
            }
        }

        public override LetterInfo[] Blanks
        {
            get
            {
                return blanks;
            }
        }

    }
}
