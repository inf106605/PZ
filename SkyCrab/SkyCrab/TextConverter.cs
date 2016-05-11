using System;

namespace SkyCrab
{
    static class TextConverter
    {
        public static string BoolToString(bool value)
        {
            if (value == true)
                return "✓";
            else
                return "-";
        }

        public static string TimeintToString(UInt32 value)
        {
            if (value == 0)
                return "Brak limitu";
            else
                return value.ToString();
        }

        public static string playerStatusToString(bool ready)
        {
            if (ready)
                return "GOTOWY";
            else
                return "OCZEKUJĄCY";
        }
    }
}
