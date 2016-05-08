using SkyCrab.Common_classes.Players;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Common_classes.Rooms.Players;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Menu.LoggedPlayer
{
    class PlayersInLobby
    {

        public ObservableCollection<PlayerInRoom> listOfPlayers = null; // lista graczy w pokoju
        public ObservableCollection<string> rulesName = null; 

        public ObservableCollection<PlayerInRoom> ListOfPlayers // lista pokoi znajomych ( bindowanie )
        {
            get
            {
                return listOfPlayers;
            }
        }
        
        public ObservableCollection<string> RulesName
        {
            get
            {
                return rulesName;
            }
        }

        public PlayersInLobby()
        {
            if (SkyCrabGlobalVariables.room != null)
            {
                listOfPlayers = new ObservableCollection<PlayerInRoom>(SkyCrabGlobalVariables.room.Players);

                rulesName = new ObservableCollection<string>();

                if (SkyCrabGlobalVariables.room.Rules.fivesFirst.value == true)
                    rulesName.Add("Pierwsze piątki");
                if (SkyCrabGlobalVariables.room.Rules.restrictedExchange.value == true)
                    rulesName.Add("Wymiany");
            }
           // if (SkyCrabGlobalVariables.room.Owner.Id != SkyCrabGlobalVariables.player.Id) return;


        }

        public string NameRoom
        {
            get
            {
                if(SkyCrabGlobalVariables.room != null)
                    return SkyCrabGlobalVariables.room.Name;
                return "";
            }
        }

        public string OwnerRoom
        {
            get
            {
                if(SkyCrabGlobalVariables.room != null)
                 return SkyCrabGlobalVariables.room.Owner.Player.Nick;
                return "";
            }
        }

        public string TypeOfRoom
        {
            get
            {
                if (SkyCrabGlobalVariables.room != null)
                {
                    if (SkyCrabGlobalVariables.room.RoomType == RoomType.PUBLIC)
                        return "publiczny";
                    else if (SkyCrabGlobalVariables.room.RoomType == RoomType.PRIVATE)
                        return "prywatny";
                    else if (SkyCrabGlobalVariables.room.RoomType == RoomType.FRIENDS)
                        return "znajomi";
                }
                return "";
            }
        }


        public string MaxLimitTime
        {
            get
            {
                if(SkyCrabGlobalVariables.room !=null)
                 return SkyCrabGlobalVariables.room.MaxTimeLimit;
                return "";
            }
        }

        public string MaxCountPlayers
        {
            get
            {
                if(SkyCrabGlobalVariables.room != null)
                  return SkyCrabGlobalVariables.room.MaxPlayersLimit;
                return "";
            }
        }
    }
}
