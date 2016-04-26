using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Game
{
    class ScrabbleBoardTiles
    {

        public ObservableCollection<ScrabbleBoardTiles> TilesInBoard { get; set; }

        public int Position { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public ScrabbleBoardTiles(){}

        public ScrabbleBoardTiles(int id, string name, int value)
        {
            this.Position = id;
            this.Name = name;
            this.Value = value;
        }

    }
}
