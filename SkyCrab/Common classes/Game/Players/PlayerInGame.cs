using Common_classes.Game.Racks;

namespace Common_classes.Game.Players
{
    class PlayerInGame
    {

        //TODO when it will be class 'Player': private Player player
        private Rack rack = new Rack();
        //TODO what else should be here?


        public Rack Rack
        {
            get
            {
                return rack;
            }
        }

    }
}
