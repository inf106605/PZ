using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Players;

namespace SkyCrab.Common_classes.Games.Players
{
    class PlayerInGame
    {

        private Player player;
        private Rack rack = new Rack();


        public Player Player
        {
            get
            {
                return player;
            }
        }

        public Rack Rack
        {
            get
            {
                return rack;
            }
        }


        public PlayerInGame(Player player)
        {
            this.player = player;
        }

    }
}
