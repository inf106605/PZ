using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Players;
using System;

namespace SkyCrab.Common_classes.Games.Players
{
    public sealed class NegativePointsException : SkyCrabException
    {
        public NegativePointsException(UInt16 currentPoints, Int16 newPoints) :
            base("" + currentPoints + newPoints + "is a negative number!")
        {
        }
    }

    public sealed class CannotUnwalkoverPlayerException : SkyCrabException
    {
    }

    class PlayerInGame
    {

        private Player player;
        private Rack rack = new Rack();
        private UInt16 points = 0;
        private bool walkover = false;


        public Player Player
        {
            get { return player; }
        }

        public Rack Rack
        {
            get { return rack;}
        }

        public UInt16 Points
        {
            get { return points; }
        }

        public bool Walkover
        {
            get { return walkover; }
            set
            {
                if (!value && walkover)
                    throw new CannotUnwalkoverPlayerException();
                walkover = value;
            }
        }


        public PlayerInGame(Player player)
        {
            this.player = player;
        }

        public void GainPoints(Int16 newPoints)
        {
            if (newPoints < 0 && -newPoints > points)
                throw new NegativePointsException(points, newPoints);
            points = (UInt16)((Int16)points + newPoints);
        }

    }
}
