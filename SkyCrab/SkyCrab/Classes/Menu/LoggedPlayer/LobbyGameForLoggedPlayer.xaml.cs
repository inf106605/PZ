using SkyCrab.Classes.Game;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace SkyCrab.Classes.Menu.LoggedPlayer
{
    /// <summary>
    /// Interaction logic for LobbyGameForLoggedPlayer.xaml
    /// </summary>
    public partial class LobbyGameForLoggedPlayer : UserControl
    {
        PlayersInLobby playersInLobby = null;


        public LobbyGameForLoggedPlayer()
        {
            InitializeComponent();

            playersInLobby = new PlayersInLobby();

            DataContext = playersInLobby;
            // co 3 sekundy następuje odświeżanie listy graczy w lobby
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            playersInLobby = new PlayersInLobby();
            DataContext = playersInLobby;

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        private void ReturnCreateRoomForLoggedPlayer_Click(object sender, RoutedEventArgs e)
        {
            var joinToRoomMsgAnswer = LeaveRoomMsg.SyncPostLeaveRoom(App.clientConn, 1000);

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
                    case ErrorCode.NOT_IN_ROOM:
                        {
                            MessageBox.Show("Nie ma Cię w pokoju!");
                            break;
                        }
                }

                return;
            }

            if (answerValue.messageId == MessageId.OK)
            {
                MessageBox.Show("Opuściłeś pokój!");
                SkyCrabGlobalVariables.room = null;
                playersInLobby = null;
                Switcher.Switch(new PlayAsLoggedPlayer());
            }

        }

        private void GameAreaButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new WindowGame());
        }

        private void RefreshPlayerList_Click(object sender, RoutedEventArgs e)
        {
            playersInLobby = new PlayersInLobby();
            DataContext = playersInLobby;
        }

        private void ChangeStatusGame_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new WindowGame());
        }
    }
}
