using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Tiles;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SkyCrab.Common_classes.Games.Pouch
{

    public class NoSuchLetterEntryInPouchException : SkyCrabException
    {

        public NoSuchLetterEntryInPouchException() :
            base()
        {
        }

    }

    public class NoSuchTileInPouchException : SkyCrabException
    {

        public NoSuchTileInPouchException() :
            base()
        {
        }

    }

    public class PouchHasToNotBeDummyToUseThisMethodException : SkyCrabException
    {

        public PouchHasToNotBeDummyToUseThisMethodException() :
            base()
        {
        }

    }

    public class PouchHasToBeDummyToUseThisMethodException : SkyCrabException
    {

        public PouchHasToBeDummyToUseThisMethodException() :
            base()
        {
        }

    }


    public class Pouch
    {

        private byte id;
        private bool dummy;
        private readonly LetterCount[] tiles;
        private uint count;
        private RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();  //Maybe this should be static. It would need thread synchronization but would be even more random. However, it would be bottleneck of the server. Anyway, that strong randomness is not needed. It could be standard random function, but prezentation says something else, so...


        public byte Id
        {
            get { return id; }
        }

        public bool Dummy
        {
            get { return dummy; }
        }

        public uint Count
        {
            get { return count; }
        }


        public Pouch(byte id, IList<LetterCount> letters)
        {
            this.id = id;
            this.dummy = false;
            this.tiles = new LetterCount[letters.Count];
            this.count = 0;
            for (uint i = 0; i != letters.Count; ++i)
            {
                LetterCount letterCount = letters[(int)i];
                this.tiles[i] = letterCount.Clone();
                this.count += letterCount.count;
            }
        }

        public Pouch(byte id, uint letterCount)
        {
            this.id = id;
            this.dummy = true;
            this.tiles = new LetterCount[1] { new LetterCount(LetterSet.BLANK, letterCount) };
            this.count = letterCount;
        }

        public bool hasLetter(Letter letter)
        {
            if (dummy)
                throw new PouchHasToNotBeDummyToUseThisMethodException();
            foreach (LetterCount letterCount in tiles)
                if (ReferenceEquals(letterCount.letter, letter))
                    return letterCount.count != 0;
            return false;
        }

        public void InsertTile(Letter letter)
        {
            if (dummy)
            {
                InsertAnyTile();
                return;
            }
            for (uint i = 0; i != tiles.Length; ++i)
            {
                LetterCount letterCount = tiles[i];
                if (ReferenceEquals(letterCount.letter, letter))
                {
                    ++letterCount.count;
                    ++count;
                }
            }
            throw new NoSuchLetterEntryInPouchException();
        }

        public void InsertAnyTile()
        {
            if (!dummy)
                throw new PouchHasToBeDummyToUseThisMethodException();
            ++tiles[0].count;
            ++count;
        }

        public Tile DrawTileWithLetter(Letter letter)
        {
            for (uint i = 0; i != tiles.Length; ++i)
            {
                LetterCount letterCount = tiles[i];
                if (ReferenceEquals(letterCount.letter, letter))
                {
                    if (letterCount.count == 0)
                        throw new NoSuchTileInPouchException();
                    Tile tile = new Tile(letterCount.letter);
                    --letterCount.count;
                    --count;
                    return tile;
                }
            }
            throw new NoSuchLetterEntryInPouchException();
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
                if (number < letterCount.count)
                {
                    --letterCount.count;
                    --count;
                    return new Tile(letterCount.letter);
                }
                else
                {
                    number -= letterCount.count;
                }
            }
            throw new NoSuchTileInPouchException();
        }

    }
}
