using Common_classes.Game.Letters;

namespace Common_classes.Game.Tiles
{
    public struct LetterCount
    {
		
        public Letter letter;
        public uint count;

		
        public LetterCount(Letter letter, uint count)
        {
            this.letter = letter;
            this.count = count;
        }

        public LetterCount Clone()
        {
            return new LetterCount(letter, count);
        }

    }
}
