using SkyCrab.Dictionaries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DictionariesTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary polishDict = Dictionary.GetPolish();
            Console.WriteLine(polishDict.WordsCount);

            List<string> wordsToTest = new List<string>();
            using (StreamReader inputFile = new StreamReader("./test_data.txt"))
            {
                string wordToTest;
                while ((wordToTest = inputFile.ReadLine()) != null)
                    wordsToTest.Add(wordToTest);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i != 100; ++i)
                foreach (string wordToTest in wordsToTest)
                    polishDict.IsWordWalid(wordToTest);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }
}
