using System;

namespace Common_classes.Game
{
    class AlterNonBlankTileException : SkyCrabException
    {

        public AlterNonBlankTileException() :
            base()
        {
        }
        
        public AlterNonBlankTileException(String message) :
            base(message)
        {
        }
        
        public AlterNonBlankTileException(String message, Exception innerException) :
            base(message, innerException)
        {
        }

    }
}
