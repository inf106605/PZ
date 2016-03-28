﻿using Common_classes.Game.Letters;

namespace Common_classes.Game.Tiles
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


        /// <summary> Letter or ampty blank. </summary>
        public Tile(Letter letter)
        {
            this.blank = ReferenceEquals(letter, LetterSet.BLANK);
            this.letter = letter;
        }

        /// <summary> Letter or blank. </summary>
        public Tile(bool blank, Letter letter)
        {
            this.blank = blank;
            this.letter = letter;
        }

        public bool IsBlankEmpty()
        {
            return ReferenceEquals(letter, LetterSet.BLANK);
        }

    }
}
