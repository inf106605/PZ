namespace SkyCrab.Common_classes
{
    public class LengthLimit
    {

        public enum Result { OK, TOO_SHORT, TOO_LONG }


        public abstract class LenghtLimitException : SkyCrabException
        {
        }

        public class TextTooShortException : LenghtLimitException
        {
        }

        public class TextTooLongException : LenghtLimitException
        {
        }


        private static readonly LengthLimit login = new LengthLimit(3, 25);
        private static readonly LengthLimit password = new LengthLimit(5, 100);
        private static readonly LengthLimit nick = new LengthLimit(1, 50);
        private static readonly LengthLimit eMail = new LengthLimit(0, 100);
        private static readonly LengthLimit roomName = new LengthLimit(0, 50);
        private static readonly LengthLimit searchPhraze = new LengthLimit(0, 100);

        public static LengthLimit Login {
            get { return login; }
        }

        public static LengthLimit Password
        {
            get { return password; }
        }

        public static LengthLimit Nick
        {
            get { return nick; }
        }

        public static LengthLimit EMail
        {
            get { return eMail; }
        }

        public static LengthLimit RoomName
        {
            get { return roomName; }
        }

        public static LengthLimit SearchPhraze
        {
            get { return searchPhraze; }
        }


        private readonly int min;
        private readonly int max;

        public int Min
        {
            get { return min; }
        }

        public int Max
        {
            get { return max; }
        }


        private LengthLimit(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public Result check(string text)
        {
            int length = text.Length;
            return check(length);
        }

        public Result check(int length)
        {
            if (length < min)
                return Result.TOO_SHORT;
            else if (length > max)
                return Result.TOO_LONG;
            else
                return Result.OK;
        }

        public void checkAndThrow(string text)
        {
            if (text == null)
                return;
            int length = text.Length;
            checkAndThrow(length);
        }

        public void checkAndThrow(int length)
        {
            if (length < min)
                throw new TextTooShortException();
            if (length > max)
                throw new TextTooLongException();
        }

    }
}
