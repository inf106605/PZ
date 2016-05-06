using SkyCrab.Common_classes.Rooms;
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
        public ObservableCollection<Room> ListOfRooms = null; // lista pokoi znajomych
        public List<Room> ListRoomsFromServer = null; // tymczasowa lista przypisanych danych z serwera - lista pokoi

        public ObservableCollection<Room> ListRooms // lista pokoi znajomych ( bindowanie )
        {
            get
            {
                return ListOfRooms;
            }
        }

        public ManageRooms()
        {
            ListOfRooms = new ObservableCollection<Room>();
            ListRoomsFromServer = new List<Room>();
        }

        public void GetRoomsFromServerToList(Room room)
        {
            ListOfRooms.Add(room);
        }

        public void ClearListRooms()
        {
            ListOfRooms.Clear();
        }

    }
}
