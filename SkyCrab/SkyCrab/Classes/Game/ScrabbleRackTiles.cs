using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Game
{
    class ScrabbleRackTiles
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public ScrabbleRackTiles()
        {

        }

        public ScrabbleRackTiles(int id, string name, int value)
        {
            this.Id = id;
            this.Name = name;
            this.Value = value;
        }

    }
}
