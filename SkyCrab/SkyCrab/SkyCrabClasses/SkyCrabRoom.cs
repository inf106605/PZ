using SkyCrab.Common_classes.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.SkyCrabClasses
{
    class SkyCrabRoom
    {

        public Room room;


        public SkyCrabRoom(Room room)
        {
            this.room = room;
        }

        public string Name
        {
            get { return room.Name; }
        }

        public string MaxTimeLimit
        {
            get { return TextConverter.TimeintToString(room.Rules.maxRoundTime.value); }
        }

        public string MaxPlayersLimit
        {
            get { return room.Rules.maxPlayerCount.value.ToString(); }
        }

        public string IsRulesFive
        {
            get { return TextConverter.BoolToString(room.Rules.fivesFirst.value); }
        }

        public string IsRulesExchange
        {
            get { return TextConverter.BoolToString(room.Rules.restrictedExchange.value); }
        }

    }
}
