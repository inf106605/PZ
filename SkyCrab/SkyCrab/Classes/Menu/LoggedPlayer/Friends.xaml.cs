using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu;
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

namespace SkyCrab.Classes.Menu.LoggedPlayer
{
    /// <summary>
    /// Interaction logic for Friends.xaml
    /// </summary>
    public partial class Friends : UserControl
    {
        Timer timer;
        FriendPlayer friendPlayer = null;
        // timer = new Timer(nazwa_funkcji,parametry,po jakim czasie ma sie wywolac, ile razy ) 
        public Friends()
        {
            InitializeComponent();
            friendPlayer = new FriendPlayer();
            DataContext = friendPlayer;
        }

        private void ReturnMenuLogged_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenuLoggedPlayer());
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void addFriendButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteFriendButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void beginChatWithFriendButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            friendPlayer.ClearListBoxSearchingPlayers();

            var getFriendMsgAnswer = FindPlayerMsg.SyncPostGetFriends(App.clientConn, searchTextBox.Text, 1000);

            if (!getFriendMsgAnswer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = getFriendMsgAnswer.Value;

            if (answerValue.messageId == MessageId.PLAYER_LIST)
            {
                List<Player> listPlayerTemp = (List<Player>)answerValue.message;

                for (int i = 0; i < listPlayerTemp.Count; i++)
                {
                    friendPlayer.GetPlayersFromServerToList(listPlayerTemp[i].Nick);
                }

                return;
            }
        }
    }
 }
