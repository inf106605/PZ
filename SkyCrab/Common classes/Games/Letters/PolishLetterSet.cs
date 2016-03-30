using System.Collections.Generic;

namespace SkyCrab.Common_classes.Games.Letters
{
    class PolishLetterSet : LetterSet
    {

        public static readonly Letter A        = new Letter('A', 1);
        public static readonly Letter A_OGONEK = new Letter('Ą', 5);
        public static readonly Letter B        = new Letter('B', 3);
        public static readonly Letter C        = new Letter('C', 2);
        public static readonly Letter C_ACUTE  = new Letter('Ć', 6);
        public static readonly Letter D        = new Letter('D', 2);
        public static readonly Letter E        = new Letter('E', 1);
        public static readonly Letter E_OGONEK = new Letter('Ę', 5);
        public static readonly Letter F        = new Letter('F', 5);
        public static readonly Letter G        = new Letter('G', 3);
        public static readonly Letter H        = new Letter('H', 3);
        public static readonly Letter I        = new Letter('I', 1);
        public static readonly Letter J        = new Letter('J', 3);
        public static readonly Letter K        = new Letter('K', 2);
        public static readonly Letter L        = new Letter('L', 2);
        public static readonly Letter L_STROKE = new Letter('Ł', 3);
        public static readonly Letter M        = new Letter('M', 2);
        public static readonly Letter N        = new Letter('N', 1);
        public static readonly Letter N_ACUTE  = new Letter('Ń', 7);
        public static readonly Letter O        = new Letter('O', 1);
        public static readonly Letter O_ACUTE  = new Letter('Ó', 5);
        public static readonly Letter P        = new Letter('P', 2);
        public static readonly Letter R        = new Letter('R', 1);
        public static readonly Letter S        = new Letter('S', 1);
        public static readonly Letter S_ACUTE  = new Letter('Ś', 5);
        public static readonly Letter T        = new Letter('T', 2);
        public static readonly Letter U        = new Letter('U', 3);
        public static readonly Letter W        = new Letter('W', 1);
        public static readonly Letter Y        = new Letter('Y', 2);
        public static readonly Letter Z        = new Letter('Z', 1);
        public static readonly Letter Z_ACUTE  = new Letter('Ż', 9);
        public static readonly Letter Z_DOT    = new Letter('Ż', 5);

        private static readonly Letter[] letters = new Letter[33] { A, A_OGONEK, B, C, C_ACUTE, D, E, E_OGONEK, F, G, H, I, J, K, L, L_STROKE, M, N, N_ACUTE, O, O_ACUTE, P, R, S, S_ACUTE, T, U, W, Y, Z, Z_ACUTE, Z_DOT, BLANK };

		
        public override IList<Letter> Letters
        {
            get
            {
                return letters;
            }
        }

    }
}
