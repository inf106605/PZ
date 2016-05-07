using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Common_classes.Games.Pouch;
using SkyCrab.Common_classes.Games.Tiles;
using System;

namespace SkyCrab.Common_classes.Rooms.Rules
{
	//Options that are sure to exist for now.
    public class RuleSet
    {
  
        public RangeRule<UInt32> maxRoundTime = new RangeRule<UInt32>();
        public RangeRule<byte> maxPlayerCount = new RangeRule<byte>();
        public Rule<bool> fivesFirst = new Rule<bool>();
        public Rule<bool> restrictedExchange = new Rule<bool>();


        public Board CreateBoard()
        {
            Board board = new StandardBoard();
            return board;
        }

        public Pouch[] CreatePouches(bool dummy)
        {
            return CreateSinglePouch(dummy);
        }

        private Pouch[] CreateSinglePouch(bool dummy)
        {
            Pouch[] pouches = new Pouch[1];
            if (dummy)
            {
                pouches[0] = new Pouch(0, (uint)PolishTileSet.instance.Letters.Length);
            }
            else
            {
                pouches[0] = new Pouch(0, PolishTileSet.instance.Letters);
            }
            return pouches;
        }

        public bool Math(RuleSet filter)
        {
            if (!maxRoundTime.Math(filter.maxRoundTime))
                return false;
            if (!maxPlayerCount.Math(filter.maxPlayerCount))
                return false;
            if (!fivesFirst.Math(filter.fivesFirst))
                return false;
            if (!restrictedExchange.Math(filter.restrictedExchange))
                return false;
            return true;
        }

    }
}
