using System.Collections.Generic;

namespace Common_classes.Game
{
    public struct Letter
    {

        char character;
        uint points;


        public Letter(char character, uint points)
        {
            this.character = character;
            this.points = points;
        }

    }

}
