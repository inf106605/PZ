using SkyCrab.Classes.ScrabbleGameFolder;
using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Pouch;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyCrab.Common_classes.Games;

namespace SkyCrab.Classes.ScrabbleGameFolder
{
    class ScrabbleGame 
    {
        public Game game;
        public ScrabbleBoard scrabbleBoard;
        public ScrabbleRack scrabbleRack;

        public ScrabbleGame()
        {
            game = new Game(SkyCrabGlobalVariables.GameId, SkyCrabGlobalVariables.room.room, true);
            scrabbleBoard = new ScrabbleBoard(); 
            scrabbleRack = new ScrabbleRack();
        }

       public ObservableCollection<ScrabbleSquare> Squares {
            get
            {
                return scrabbleBoard.Squares;   
            }
        }

        public ObservableCollection<ScrabbleRackTiles> RackTiles
        {
            get
            {
                return scrabbleRack.RackTiles;
            }
        }

        public string OwnerRoom
        {
            get
            {
                if (SkyCrabGlobalVariables.room.room.Owner != null)
                    return "Właściciel pokoju: " + SkyCrabGlobalVariables.room.room.Owner.Player.Nick;
                else
                    return "";
            }
        }

        public string PouchesLeftTiles
        {
            get
            {
                return "Pozostało " + game.Puoches[0].Count + " płytki";
            }
        }

        public string RoomName
        {
            get
            {
                return "Pokój: " + SkyCrabGlobalVariables.room.Name;
            }
        }

    }
}
