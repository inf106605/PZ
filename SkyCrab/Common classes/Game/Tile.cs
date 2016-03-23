namespace Common_classes.Game
{
    class Tile
    {

        private bool blank;
        private char letter;


        public bool Blank
        {
            get
            {
                return blank;
            }
        }

        public char Letter
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
        public Tile(bool blank, char letter)
        {
            this.blank = blank;
            this.letter = letter;
        }

        /// <summary> Blanks only. </summary>
        public Tile()
        {
            this.blank = true;
            this.letter = '\0';
        }

        /// <summary> Letters only. </summary>
        public Tile(char letter)
        {
            this.blank = false;
            this.letter = letter;
        }

        public bool isLetterEmpty()
        {
            return letter == '\0';
        }

    }
}
