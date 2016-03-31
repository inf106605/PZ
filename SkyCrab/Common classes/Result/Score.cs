using SkyCrab.Common_classes.Players;

namespace SkyCrab.Common_classes.Result
{
    public class Score
    {

        private Player player;
        private bool walkover;
        private uint place;
        private uint points;

        
        public Player Player
        {
            get
            {
                return player;
            }
        }

        public bool Walkover
        {
            get
            {
                return walkover;
            }
        }

        public uint Place
        {
            get
            {
                return place;
            }
        }

        public uint Points
        {
            get
            {
                return points;
            }
        }


        public Score(Player player, bool walkover, uint place, uint points)
        {
            this.player = player;
            this.walkover = walkover;
            this.place = place;
            this.points = points;
        }

    }
}
