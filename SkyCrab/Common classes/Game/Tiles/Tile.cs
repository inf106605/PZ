namespace Common_classes.Game
{
    public class AlterNonBlankTileException : SkyCrabException
    {

        public AlterNonBlankTileException() :
            base()
        {
        }

    }

    public class Tile
    {

        private bool blank;
        private Letter letter;


        public bool Blank
        {
            get
            {
                return blank;
            }
        }

        public Letter Letter
        {
            get
            {
                return letter;
            }
            set
            {
                if (blank == false)
                    throw new AlterNonBlankTileException();
                letter = value;
            }
        }
        

        /// <summary> Blanks and letters. </summary>
        public Tile(bool blank, Letter letter)
        {
            this.blank = blank;
            this.letter = letter;
        }

        /// <summary> Blanks only. </summary>
        public Tile()
        {
            this.blank = true;
            this.letter = LetterSet.BLANK;
        }

        /// <summary> Letters only. </summary>
        public Tile(Letter letter)
        {
            this.blank = false;
            this.letter = letter;
        }

        public bool IsBlankEmpty()
        {
            return ReferenceEquals(letter, LetterSet.BLANK);
        }

    }
}
