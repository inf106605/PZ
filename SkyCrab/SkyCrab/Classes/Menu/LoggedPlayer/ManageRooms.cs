using SkyCrab.Common_classes.Rooms;
using SkyCrab.SkyCrabClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.Classes.Menu.LoggedPlayer
{
    class ManageRooms
    {
        public ObservableCollection<SkyCrabRoom> ListOfRooms = null; // lista pokoi znajomych

        public ObservableCollection<SkyCrabRoom> ListRooms // lista pokoi znajomych ( bindowanie )
        {
            get
            {
                return ListOfRooms;
            }
        }

        public ManageRooms()
        {
            ListOfRooms = new ObservableCollection<SkyCrabRoom>();
        }

        public void ClearListRooms()
        {
            ListOfRooms.Clear();
        }

    }
}
