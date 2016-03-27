using System;
using System.Collections.Generic;

namespace Common_classes.Game
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
