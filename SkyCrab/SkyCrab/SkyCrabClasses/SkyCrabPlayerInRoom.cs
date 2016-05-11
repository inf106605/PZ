using SkyCrab.Common_classes.Rooms.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.SkyCrabClasses
{
    class SkyCrabPlayerInRoom
    {
        public PlayerInRoom playerInRoom;

        public SkyCrabPlayerInRoom(PlayerInRoom playerInRoom)
        {
            this.playerInRoom = playerInRoom;
        }

        public string Nick
        {
            get
            {
                return playerInRoom.Player.Nick;
            }
        }

        public string isReadyStatus
        {
            get
            {
                if (playerInRoom.IsReady)
                    return "GOTOWY";
                else
                    return "OCZEKUJĄCY";
            }
        }
    }
}
