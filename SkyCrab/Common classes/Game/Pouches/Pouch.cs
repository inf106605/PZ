using Common_classes.Game.Letters;
using Common_classes.Game.Tiles;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Common_classes.Game.Pouches
{
    class Pouch
    {

        private readonly LetterCount[] tiles;
        private uint count;
        private RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();  //TODO Maybe this should be static. It would need thread synchronization but would be even more random. However, it would be bottleneck of the server. Anyway, that strong randomness is not needed. It could be standard random function, but prezentation says something else, so...


        public uint Count
        {
            get
            {
                return count;
            }
        }


        public Pouch(IList<LetterCount> letters)
        {
            tiles = new LetterCount[letters.Count];
            count = 0;
            for (uint i = 0; i != letters.Count; ++i)
            {
                LetterCount letterCount = letters[(int)i];
                tiles[i] = letterCount.Clone();
                count += letterCount.count;
            }
        }

        public Pouch(uint letterCount)
        {
            tiles = new LetterCount[1] { new LetterCount(LetterSet.BLANK, letterCount) };
            count = letterCount;
        }

        public bool hasLetter(Letter letter)
        {
            foreach (LetterCount letterCount in tiles)
                if (ReferenceEquals(letterCount.letter, letter))
                    return letterCount.count != 0;
            return false;
        }

        public Tile DrawTileWithLetter(Letter letter)
        {
            for (uint i = 0; i != tiles.Length; ++i)
            {
                LetterCount letterCount = tiles[i];
                if (ReferenceEquals(letterCount.letter, letter))
                {
                    Tile tile = new Tile(letterCount.letter);
                    --letterCount.count;
                    --count;
                    return tile;
                }
            }
            return null;
        }

        public Tile DrawRandowmTile()
        {
            uint tileNumber = getRandomUInt32() % count;
            return DrawNthTile(tileNumber);
        }

        private UInt32 getRandomUInt32()
        {
            byte[] bytes = new byte[4];
            random.GetBytes(bytes);
            UInt32 randomNumber = (UInt32)(bytes[0] << 24 |
                    bytes[1] << 16 |
                    bytes[2] << 8 |
                    bytes[3] << 0);
            return randomNumber;
        }

        public Tile DrawFirstTile()
        {
            return DrawNthTile(1);
        }

        public Tile DrawNthTile(uint number)
        {
            for (uint i = 0; i != tiles.Length; ++i)
            {
                LetterCount letterCount = tiles[i];
                if (number <= letterCount.count)
                {
                    Tile tile = new Tile(letterCount.letter);
                    --letterCount.count;
                    --count;
                    return tile;
                }
                else
                {
                    number -= letterCount.count;
                }
            }
            return null;
        }

    }
}
