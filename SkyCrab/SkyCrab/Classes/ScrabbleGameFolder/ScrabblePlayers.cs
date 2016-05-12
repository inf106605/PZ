using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.ScrabbleGameFolder
{
    class ScrabblePlayers
    {
        public string PlayerName { get; set; }
        public int PlayerPoints { get; set; }
        public string PlayerTime { get; set; }
        public int PlayerTiles { get; set; }

        public ScrabblePlayers(string PlayerName, int PlayerPoints, string PlayerTime, int PlayerTiles )
        {
            this.PlayerName = PlayerName;
            this.PlayerPoints = PlayerPoints;
            this.PlayerTime = PlayerTime;
            this.PlayerTiles = PlayerTiles;
        }
    }
}
