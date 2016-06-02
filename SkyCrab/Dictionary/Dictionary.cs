using System;
using System.Collections.Generic;
using System.IO;

namespace SkyCrab.Dictionaries
{
    public sealed class Dictionary
    {

        public enum Language
        {
            POLISH
        }


        private static readonly Dictionary<Language, string> languageNames = new Dictionary<Language, string>();

        private readonly HashSet<string> words = new HashSet<string>();


        static Dictionary()
        {
            languageNames.Add(Language.POLISH, "polish");
        }

        public uint WordsCount
        {
            get { return (uint)words.Count; }
        }


        public Dictionary(Language language)
        {
            const string DIRECTORY = "./dictionaries/";
            const string EXTENSION = ".dict";
            string languageName;
            languageNames.TryGetValue(language, out languageName);
            string filePath = DIRECTORY + languageName + EXTENSION;
            LoadWords(filePath);
        }

        private void LoadWords(string filePath)
        {
            using (StreamReader file = new StreamReader(filePath))
            {
                string line;
                string previousWord = "";
                while ((line = file.ReadLine()) != null)
                {
                    if (line == "")
                        continue;
                    string word;
                    int repeatedLetters = GetNumberFromBegining(line, out word);
                    string decompresedWord = previousWord.Substring(0, repeatedLetters) + word;
                    AddWord(decompresedWord);
                    previousWord = decompresedWord;
                }
            }
        }

        private static int GetNumberFromBegining(string line, out string word)
        {
            int result = 0;
            int i;
            for (i = 0; line[i] >= '0' && line[i] <= '9'; ++i)
            {
                result *= 10;
                result += line[i] - '0';
            }
            word = line.Substring(i);
            return result;
        }

        private void AddWord(string word)
        {
            word = word.ToUpper();
            words.Add(word);
        }

        public static void Compress(string inputFilePath, string outputFilePath)
        {
            using (StreamReader inputFile = new StreamReader(inputFilePath))
            using (StreamWriter outputFile = new StreamWriter(outputFilePath))
            {
                string previousWord = " ";
                string word;
                while ((word = inputFile.ReadLine()) != null)
                {
                    if (word == "")
                        continue;
                    string cuttedWord;
                    int repeatedLetters = commonStart(word, out cuttedWord, previousWord);
                    string compesedWord = (repeatedLetters == 0 ? "" : "" + repeatedLetters) + cuttedWord;
                    outputFile.WriteLine(compesedWord);
                    previousWord = word;
                }
            }
        }

        private static int commonStart(string wholeWord, out string cuttedWord, string previousWord)
        {
	        int max = Math.Min(wholeWord.Length, previousWord.Length);
            int i = 0;
	        for (;; ++i)
	        {
		        if (i == max)
			        break;
		        if (wholeWord[i] != previousWord[i])
			        break;
	        }
            cuttedWord = wholeWord.Substring(i);
	        return i;
        }

        public bool IsWordWalid(string word)
        {
            word = word.ToUpper();
            return words.Contains(word);
        }

    }
}
