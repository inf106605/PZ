using Common_classes.Game.Letters;

namespace Common_classes.Game.Tiles
{
    public struct LetterInfo
    {
		
        Letter letter;
        uint count;
		
		
        public LetterInfo(Letter letter, uint count)
        {
            this.letter = letter;
            this.count = count;
        }
		
    }
}
