using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Menu.Guest
{
    static class RulesClass
    {
        public static string PrintText()
        {
            return Properties.Resources.RulesText;
        }

        private static void PrintCenteredText(string text)
        {
            string leftSpace = new string(' ', (80 - text.Length) / 2);
            Console.Write(leftSpace);
            Console.WriteLine(text);
        }

    }
}
