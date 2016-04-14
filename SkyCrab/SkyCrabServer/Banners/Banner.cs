using System;

namespace SkyCrabServer.Banners
{
    static class Banner
    {

        public static void PrintBanner(object version)
        {
            Console.WriteLine(Properties.Resources.banner);
            PrintCenteredText("SkyCrab Server "+version);
            Console.WriteLine(new string('-', 80));
        }

        private static void PrintCenteredText(string text)
        {
            string leftSpace = new string(' ', (80 - text.Length) / 2);
            Console.Write(leftSpace);
            Console.WriteLine(text);
        }

    }
}
