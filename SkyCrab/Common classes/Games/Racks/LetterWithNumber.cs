using SkyCrab.Common_classes.Games.Letters;

namespace SkyCrab.Common_classes.Games.Racks
{
    public class LetterWithNumber
    {

        public Letter letter;
        public byte number;

        public override string ToString()
        {
            return letter + "; " + number;
        }

    }
}
