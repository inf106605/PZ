using SkyCrab.Classes.Menu.LoggedPlayer;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
using SkyCrab.SkyCrabClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace SkyCrab.Classes.Menu
{
    /// <summary>
    /// Interaction logic for PlayAsLoggedPlayer.xaml
    /// </summary>
    public partial class PlayAsLoggedPlayer : UserControl
    {

        ManageRooms manageRooms = null;


        ObservableCollection<String> minTimeLimitLabels;
        ObservableCollection<String> maxTimeLimitLabels;
        ObservableCollection<String> minCountPlayersLabels;
        ObservableCollection<String> maxCountPlayersLabels;

        private int CounterForTrigger = 0;

        public PlayAsLoggedPlayer()
        {
            InitializeComponent();

            manageRooms = new ManageRooms();
            Room filterRoom = new Room();

            loadingItems();


            filterRoom.Name = "";
            filterRoom.Type = RoomType.PUBLIC;
            filterRoom.Rules.fivesFirst.indifferently = true;
           // filterRoom.Rules.fivesFirst.value = true;
           filterRoom.Rules.restrictedExchange.indifferently = true;
            //filterRoom.Rules.restrictedExchange.value = true;
            filterRoom.Rules.maxPlayerCount.indifferently = true;
            filterRoom.Rules.maxRoundTime.indifferently = true;

            var getListOfRooms = FindRoomsMsg.SyncPostFindRooms(App.clientConn, filterRoom, 1000);
 
            if (!getListOfRooms.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = getListOfRooms.Value;

            if (answerValue.messageId == MessageId.ROOM_LIST)
            {
                List<Room> rooms = (List<Room>)answerValue.message;
                foreach (Room room in rooms)
                {
                    manageRooms.ListOfRooms.Add(new SkyCrabRoom(room));
                }
            }

            DataContext = manageRooms;

            // co 5 sekund następuje odświeżanie listy pokoi graczy
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating Searching
            UpdateSearching();
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        private void UpdateSearching(object sender, RoutedEventArgs x)
        {
            CounterForTrigger++;

            if (CounterForTrigger < 5)
                return;

            textBoxSearchRoom.IsEnabled = true;
            minTimeLimit.IsEditable = true;
            maxTimeLimit.IsEditable = true;
            minCountPlayers.IsEditable = true;
            maxCountPlayers.IsEditable = true;
            fivesFirst.IsEnabled = true;
            restrictedExchange.IsEnabled = true;

            // czyszczenie starej zawartości listy pokoi
            manageRooms.ClearListRooms();

            Room filterRoom = new Room();

            filterRoom.Name = textBoxSearchRoom.Text;
            //publiczny
            if (publicRoomRadioButton.IsChecked.Value)
            {
                filterRoom.Type = RoomType.PUBLIC;
            }
            // znajomi
            else if (friendsRoomRadioButton.IsChecked.Value)
            {
                filterRoom.Type = RoomType.FRIENDS;
            }

            if (fivesFirst.IsChecked == true)
            {
                filterRoom.Rules.fivesFirst.value = true;
            }
            else if (fivesFirst.IsChecked == false)
            {
                filterRoom.Rules.fivesFirst.value = false;
            }

            if (restrictedExchange.IsChecked == true)
            {
                filterRoom.Rules.restrictedExchange.value = true;
            }
            else if (restrictedExchange.IsChecked == false)
            {
                filterRoom.Rules.restrictedExchange.value = false;
            }
            // stany pośrednie checkboxów - reguły gry
            if (fivesFirst.IsChecked == null)
            {
                filterRoom.Rules.fivesFirst.indifferently = true;
            }
            if (restrictedExchange.IsChecked == null)
            {
                filterRoom.Rules.restrictedExchange.indifferently = true;
            }
            // min i max czas gry

            if (minTimeLimit.SelectedValue != null)
            {
                if ((string)minTimeLimit.SelectedValue == "Brak limitu")
                    filterRoom.Rules.maxRoundTime.min = 0;
                else if (int.Parse((string)minTimeLimit.SelectedValue) > 0)
                    filterRoom.Rules.maxRoundTime.min = uint.Parse((string)minTimeLimit.SelectedValue);

            }


            if (maxTimeLimit.SelectedValue != null)
            {
                if ((string)maxTimeLimit.SelectedValue == "Brak limitu")
                    filterRoom.Rules.maxRoundTime.max = 0;

                else if (int.Parse((string)maxTimeLimit.SelectedValue) > 0)
                    filterRoom.Rules.maxRoundTime.max = uint.Parse((string)maxTimeLimit.SelectedValue);
            }

            // min i max liczba graczy

            if (minCountPlayers.SelectedValue != null)
            {
                if (int.Parse((string)minCountPlayers.SelectedValue) > 0)
                    filterRoom.Rules.maxPlayerCount.min = byte.Parse((string)minCountPlayers.SelectedValue);
            }

            if (maxCountPlayers.SelectedValue != null)
            {
                if (int.Parse((string)maxCountPlayers.SelectedValue) > 0)
                    filterRoom.Rules.maxPlayerCount.max = byte.Parse((string)maxCountPlayers.SelectedValue);
            }

            if (friendsRoomRadioButton.IsChecked.Value)
            {
                textBoxSearchRoom.IsEnabled = false;
                minTimeLimit.IsEditable = false;
                maxTimeLimit.IsEditable = false;
                minCountPlayers.IsEditable = false;
                maxCountPlayers.IsEditable = false;
                fivesFirst.IsEnabled = false;
                restrictedExchange.IsEnabled = false;

                var getListofFriendRooms = GetFriendRoomsMsg.SyncPostGetFriendRooms(App.clientConn, 2000);

                if (!getListofFriendRooms.HasValue)
                {
                    MessageBox.Show("Brak odpowiedzi od serwera!");
                    return;
                }

                var answerValue = getListofFriendRooms.Value;

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
                    List<Room> rooms = (List<Room>)answerValue.message;
                    foreach (Room room in rooms)
                    {
                        manageRooms.ListOfRooms.Add(new SkyCrabRoom(room));
                    }
                }


            }

            if (publicRoomRadioButton.IsChecked.Value)
            {

                var getListOfRooms = FindRoomsMsg.SyncPostFindRooms(App.clientConn, filterRoom, 1000);

                if (!getListOfRooms.HasValue)
                {
                    MessageBox.Show("Brak odpowiedzi od serwera!");
                    return;
                }

                var answerValue = getListOfRooms.Value;

                if (answerValue.messageId == MessageId.ROOM_LIST)
                {
                    List<Room> rooms = (List<Room>)answerValue.message;
                    foreach (Room room in rooms)
                    {
                        manageRooms.ListOfRooms.Add(new SkyCrabRoom(room));
                    }
                }

            }
        }

        private void UpdateSearching()
        {
            CounterForTrigger++;

            if (CounterForTrigger < 5)
                return;

            textBoxSearchRoom.IsEnabled = true;
            minTimeLimit.IsEditable = true;
            maxTimeLimit.IsEditable = true;
            minCountPlayers.IsEditable = true;
            maxCountPlayers.IsEditable = true;
            fivesFirst.IsEnabled = true;
            restrictedExchange.IsEnabled = true;

            // czyszczenie starej zawartości listy pokoi
            manageRooms.ClearListRooms();

            Room filterRoom = new Room();

            filterRoom.Name = textBoxSearchRoom.Text;
            //publiczny
            if(publicRoomRadioButton.IsChecked.Value)
            {
                filterRoom.Type = RoomType.PUBLIC;
            }
            // znajomi
            else if (friendsRoomRadioButton.IsChecked.Value)
            {
                filterRoom.Type = RoomType.FRIENDS;
            }

            if(fivesFirst.IsChecked == true)
            {
                filterRoom.Rules.fivesFirst.value = true;
            }
            else if(fivesFirst.IsChecked == false)
            {
                filterRoom.Rules.fivesFirst.value = false;
            }

            if(restrictedExchange.IsChecked == true)
            {
                filterRoom.Rules.restrictedExchange.value = true;
            }
            else if(restrictedExchange.IsChecked == false)
            {
                filterRoom.Rules.restrictedExchange.value = false;
            }
            // stany pośrednie checkboxów - reguły gry
            if(fivesFirst.IsChecked == null)
            {
                filterRoom.Rules.fivesFirst.indifferently = true;
            }
            if(restrictedExchange.IsChecked == null)
            {
                filterRoom.Rules.restrictedExchange.indifferently = true;
            }
            // min i max czas gry

            if (minTimeLimit.SelectedValue != null)
            {
                if ((string)minTimeLimit.SelectedValue == "Brak limitu")
                    filterRoom.Rules.maxRoundTime.min = 0;
                else if (int.Parse((string)minTimeLimit.SelectedValue) > 0)
                    filterRoom.Rules.maxRoundTime.min = uint.Parse((string)minTimeLimit.SelectedValue);

            }


            if (maxTimeLimit.SelectedValue != null)
            {
                if((string)maxTimeLimit.SelectedValue == "Brak limitu")
                    filterRoom.Rules.maxRoundTime.max = 0;

                else if (int.Parse((string)maxTimeLimit.SelectedValue) > 0)
                    filterRoom.Rules.maxRoundTime.max = uint.Parse((string)maxTimeLimit.SelectedValue);
            }

            // min i max liczba graczy
            
            if(minCountPlayers.SelectedValue != null)
            {
                if(int.Parse((string)minCountPlayers.SelectedValue) > 0)
                    filterRoom.Rules.maxPlayerCount.min = byte.Parse((string)minCountPlayers.SelectedValue);
            }

            if(maxCountPlayers.SelectedValue != null)
            {
                if(int.Parse((string)maxCountPlayers.SelectedValue) > 0)
                    filterRoom.Rules.maxPlayerCount.max = byte.Parse((string)maxCountPlayers.SelectedValue);
            }

            if (friendsRoomRadioButton.IsChecked.Value)
            {
                textBoxSearchRoom.IsEnabled = false;
                minTimeLimit.IsEditable = false;
                maxTimeLimit.IsEditable = false;
                minCountPlayers.IsEditable = false;
                maxCountPlayers.IsEditable = false;
                fivesFirst.IsEnabled = false;
                restrictedExchange.IsEnabled = false;

                var getListofFriendRooms = GetFriendRoomsMsg.SyncPostGetFriendRooms(App.clientConn, 2000);

                if (!getListofFriendRooms.HasValue)
                {
                    MessageBox.Show("Brak odpowiedzi od serwera!");
                    return;
                }

                var answerValue = getListofFriendRooms.Value;

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
                    List<Room> rooms = (List<Room>)answerValue.message;
                    foreach (Room room in rooms)
                    {
                        manageRooms.ListOfRooms.Add(new SkyCrabRoom(room));
                    }
                }


            }

            if (publicRoomRadioButton.IsChecked.Value)
            {

                var getListOfRooms = FindRoomsMsg.SyncPostFindRooms(App.clientConn, filterRoom, 1000);

                if (!getListOfRooms.HasValue)
                {
                    MessageBox.Show("Brak odpowiedzi od serwera!");
                    return;
                }

                var answerValue = getListOfRooms.Value;

                if (answerValue.messageId == MessageId.ROOM_LIST)
                {
                    List<Room> rooms = (List<Room>)answerValue.message;
                    foreach (Room room in rooms)
                    {
                        manageRooms.ListOfRooms.Add(new SkyCrabRoom(room));
                    }
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenuLoggedPlayer());
        }

        private void RoomCreateButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new CreateRoomForLoggedPlayers());
        }

        private void loadingItems()
        {
            minTimeLimitLabels = new ObservableCollection<String>();

            for (int i = 5; i <= 15; i += 5)
            {
                minTimeLimitLabels.Add(i.ToString());
            }
            for (int i = 30; i <= 60; i += 15)
            {
                minTimeLimitLabels.Add(i.ToString());
            }
            minTimeLimitLabels.Add("Brak limitu");

            minTimeLimit.ItemsSource = minTimeLimitLabels;
            minTimeLimit.SelectedIndex = 0;

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

            minCountPlayersLabels = new ObservableCollection<String>();
            for (int i = 1; i <= 4; i++)
            {
                minCountPlayersLabels.Add(i.ToString());
            }
            minCountPlayers.ItemsSource = minCountPlayersLabels;
            minCountPlayers.SelectedIndex = 0;

            maxCountPlayersLabels = new ObservableCollection<String>();
            for (int i = 1; i <= 4; i++)
            {
                maxCountPlayersLabels.Add(i.ToString());
            }
            maxCountPlayers.ItemsSource = maxCountPlayersLabels;
            maxCountPlayers.SelectedIndex = 3;
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

        private void SelectAndJoinToRoom_Click(object sender, RoutedEventArgs e)
        {

            lock (SkyCrabGlobalVariables.roomLock)
            {

                foreach (var item in ListRooms.SelectedItems)
                {
                    UInt32 roomId = ((Room)item.GetType().GetField("room").GetValue(item)).Id;
                    var joinToRoomMsgAnswer = JoinRoomMsg.SyncPostLogout(App.clientConn, roomId, 1000);

                    if (!joinToRoomMsgAnswer.HasValue)
                    {
                        MessageBox.Show("Brak odpowiedzi od serwera!");
                        return;
                    }

                    var answerValue = joinToRoomMsgAnswer.Value;

                    if (answerValue.messageId == MessageId.ERROR)
                    {
                        ErrorCode errorCode = (ErrorCode)answerValue.message;

                        switch (errorCode)
                        {
                            case ErrorCode.NO_SUCH_ROOM:
                                {
                                    MessageBox.Show("Nie ma takiego pokoju!");
                                    break;
                                }
                            case ErrorCode.ALREADY_IN_ROOM2:
                                {
                                    MessageBox.Show("Już jesteś w pokoju!");
                                    break;
                                }
                            case ErrorCode.ROOM_IS_FULL:
                                {
                                    MessageBox.Show("Wybrany pokój jest pełny!");
                                    break;
                                }
                        }

                        return;
                    }

                    if (answerValue.messageId == MessageId.ROOM)
                    {
                        Room answerRoom = (Room)answerValue.message;
                        SkyCrabGlobalVariables.room = new SkyCrabRoom(answerRoom);
                        Switcher.Switch(new LobbyGameForLoggedPlayer());
                    }

                }
            }
        }
    }
}
