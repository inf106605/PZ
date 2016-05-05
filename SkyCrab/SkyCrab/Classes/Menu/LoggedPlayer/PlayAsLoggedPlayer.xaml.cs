using SkyCrab.Classes.Menu.LoggedPlayer;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkyCrab.Classes.Menu
{
    /// <summary>
    /// Interaction logic for PlayAsLoggedPlayer.xaml
    /// </summary>
    public partial class PlayAsLoggedPlayer : UserControl
    {

        ManageRooms manageRooms = null;

        ObservableCollection<String> typeOfRoomLabels;
        ObservableCollection<String> minTimeLimitLabels;
        ObservableCollection<String> maxTimeLimitLabels;
        ObservableCollection<String> minCountPlayersLabels;
        ObservableCollection<String> maxCountPlayersLabels;

        public PlayAsLoggedPlayer()
        {
            InitializeComponent();
            manageRooms = new ManageRooms();
            
            

          //  var getListOfRooms = FindRoomsMsg.SyncPostFindRooms(App.clientConn,
 
            if (!getListOfRooms.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = getListOfRooms.Value;

            if (answerValue.messageId == MessageId.ERROR)
            {
                ErrorCode errorCode = (ErrorCode)answerValue.message;

                switch (errorCode)
                {
                    case ErrorCode.NOT_LOGGED6:
                        {
                            MessageBox.Show("Nie jesteś zalogowany!");
                            break;
                        }

                }

                return;
            }

            if (answerValue.messageId == MessageId.ROOM_LIST)
            {
                manageRooms.ListRoomsFromServer = (List<Room>)answerValue.message;
                for (int i = 0; i < manageRooms.ListRoomsFromServer.Count; i++)
                {
                    manageRooms.ListOfRooms.Add(manageRooms.ListRoomsFromServer[i]);
                }
            }

            DataContext = manageRooms;

    
        } 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenuLoggedPlayer());
        }

        private void RoomCreateButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new CreateRoomForLoggedPlayers());
        }

        private void textBoxSearchRoom_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void typeOfRoom_Loaded(object sender, RoutedEventArgs e)
        {
            typeOfRoomLabels = new ObservableCollection<String>();

            typeOfRoomLabels.Add("publiczny");
            typeOfRoomLabels.Add("znajomi");

            typeOfRoom.ItemsSource = typeOfRoomLabels;
            typeOfRoom.SelectedIndex = 0;
        }

        private void minTimeLimit_Loaded(object sender, RoutedEventArgs e)
        {
            minTimeLimitLabels = new ObservableCollection<String>();

            for(int i=5; i <=15; i+=5)
            {
                minTimeLimitLabels.Add(i.ToString());
            }
            for(int i=30; i <=60; i+=15)
            {
                minTimeLimitLabels.Add(i.ToString());
            }
            minTimeLimitLabels.Add("Brak limitu");

            minTimeLimit.ItemsSource = minTimeLimitLabels;
            minTimeLimit.SelectedIndex = 0;
        }

        private void maxTimeLimit_Loaded(object sender, RoutedEventArgs e)
        {
            maxTimeLimitLabels = new ObservableCollection<String>();

            for (int i = 5; i <= 15; i += 5)
            {
                maxTimeLimitLabels.Add(i.ToString());
            }
            for (int i = 30; i <= 60; i += 15)
            {
                maxTimeLimitLabels.Add(i.ToString());
            }
            maxTimeLimitLabels.Add("Brak limitu");
            maxTimeLimit.ItemsSource = minTimeLimitLabels;
            maxTimeLimit.SelectedIndex = 6;

        }

        private void minCountPlayers_Loaded(object sender, RoutedEventArgs e)
        {
            minCountPlayersLabels = new ObservableCollection<String>();
            for(int i=1; i <=4; i++)
            {
                minCountPlayersLabels.Add(i.ToString());
            }
            minCountPlayers.ItemsSource = minCountPlayersLabels;
            minCountPlayers.SelectedIndex = 0;
        }

        private void maxCountPlayers_Loaded(object sender, RoutedEventArgs e)
        {
            maxCountPlayersLabels = new ObservableCollection<String>();
            for (int i = 1; i <= 4; i++)
            {
                maxCountPlayersLabels.Add(i.ToString());
            }
            maxCountPlayers.ItemsSource = maxCountPlayersLabels;
            maxCountPlayers.SelectedIndex = 3;
        }
    }
}
