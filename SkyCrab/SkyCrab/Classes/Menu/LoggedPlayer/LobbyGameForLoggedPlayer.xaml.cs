using SkyCrab.Classes.ScrabbleGameFolder;
using SkyCrab.Common_classes.Chats;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
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
        DispatcherTimer dispatcherTimer;

        public LobbyGameForLoggedPlayer()
        {
            InitializeComponent();

            playersInLobby = new PlayersInLobby();

            DataContext = playersInLobby;
            // co 3 sekundy następuje odświeżanie listy graczy w lobby
            dispatcherTimer = new DispatcherTimer();
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

            ReadChat.Text = SkyCrabGlobalVariables.MessagesLog;

            if (SkyCrabGlobalVariables.isGame)
            {
                Switcher.Switch(new WindowGame());
                dispatcherTimer.Stop();
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private void ReturnCreateRoomForLoggedPlayer_Click(object sender, RoutedEventArgs e)
        {
            var joinToRoomMsgAnswer = LeaveRoomMsg.SyncPost(App.clientConn, 1000);

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
                SkyCrabGlobalVariables.MessagesLog = "";
                playersInLobby = null;
                dispatcherTimer.Stop();
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
            if (SkyCrabGlobalVariables.room != null)
            {
                PlayerInRoom myPlayer = SkyCrabGlobalVariables.room.room.GetPlayer(SkyCrabGlobalVariables.player.Id);

                MessageInfo? playerReadyMsgAnswer;

                if (!myPlayer.IsReady)
                {
                    playerReadyMsgAnswer = PlayerReadyMsg.SyncPost(App.clientConn, myPlayer.Player.Id, 1000);
                }
                else
                {
                    playerReadyMsgAnswer = PlayerNotReadyMsg.SyncPost(App.clientConn, myPlayer.Player.Id, 1000);
                }

                if (!playerReadyMsgAnswer.HasValue)
                {
                    MessageBox.Show("Brak odpowiedzi od serwera!");
                    return;
                }

                var answerValue = playerReadyMsgAnswer.Value;

                if (answerValue.messageId == MessageId.ERROR)
                {
                    ErrorCode errorCode = (ErrorCode)answerValue.message;

                    switch (errorCode)
                    {
                        case ErrorCode.NOT_IN_ROOM2:
                            {
                                MessageBox.Show("Nie ma Cię w pokoju!");
                                break;
                            }
                    }

                    return;
                }
            }

            else
                return;
        }
        
        private void SendChatMessage_Click(object sender, RoutedEventArgs e)
        {
            if (WriteChat.Text.Length < 1)
                return;

            ChatMessage chatMessage = new ChatMessage();
            chatMessage.Message =  WriteChat.Text;
            var chatMsgAnswer = ChatMsg.SyncPost(App.clientConn, chatMessage, 1000);

            if (!chatMsgAnswer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = chatMsgAnswer.Value;

            if (answerValue.messageId == MessageId.ERROR)
            {
                ErrorCode errorCode = (ErrorCode)answerValue.message;

                switch (errorCode)
                {
                    case ErrorCode.NOT_IN_ROOM4:
                        {
                            MessageBox.Show("Nie ma Cię w pokoju!");
                            break;
                        }
                }
                return;
            }

            WriteChat.Text = "";

        }

        private void WriteChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && WriteChat.Text.Length > 0)
            {
                SendChatMessage_Click((object)sender, (KeyEventArgs)e);
            }
        }
    }
}
