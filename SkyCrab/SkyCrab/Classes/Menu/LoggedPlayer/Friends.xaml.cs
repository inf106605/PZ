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


            foreach ( var item in FriendSearchListBox.SelectedItems)
            {
                MessageBox.Show(item.GetType().GetProperty("Nick").GetValue(item,null).ToString());
                if(!friendPlayer.ListPlayers.Contains(item))
                {
                    var addFriendMsgAnswer = AddFriendMsg.SyncPostAddFriend(App.clientConn, uint.Parse(item.GetType().GetProperty("Id").GetValue(item, null).ToString()), 1000);

                    if (!addFriendMsgAnswer.HasValue)
                    {
                        MessageBox.Show("Brak odpowiedzi od serwera!");
                        return;
                    }

                    var answerValue = addFriendMsgAnswer.Value;

                    if (answerValue.messageId == MessageId.ERROR)
                    {
                        ErrorCode errorCode = (ErrorCode)answerValue.message;

                        switch (errorCode)
                        {
                            case ErrorCode.NOT_LOGGED3:
                                {
                                    MessageBox.Show("Nie jesteś zalogowany!");
                                    break;
                                }
                            case ErrorCode.FRIEND_ALREADY_ADDED:
                                {
                                    MessageBox.Show("Już dodałeś tego znajomego!");
                                    break;
                                }
                            case ErrorCode.FOREVER_ALONE:
                                {
                                    MessageBox.Show("Próbujesz dodać do znajomych samego siebie!");
                                    break;
                                }
                        }

                        return;
                    }

                    if (answerValue.messageId == MessageId.OK)
                    {
                        friendPlayer.AddPlayerToFriend(uint.Parse(item.GetType().GetProperty("Id").GetValue(item, null).ToString()), item.GetType().GetProperty("Nick").GetValue(item,null).ToString());

                        var getFriendMsgAnswer = GetFriendsMsg.SyncPostGetFriends(App.clientConn, 1000);
                        if (!getFriendMsgAnswer.HasValue)
                        {
                            MessageBox.Show("Brak odpowiedzi od serwera!");
                            return;
                        }
                        var answerValue1 = getFriendMsgAnswer.Value;

                        if (answerValue.messageId == MessageId.ERROR)
                        {
                            ErrorCode errorCode = (ErrorCode)answerValue.message;

                            switch (errorCode)
                            {
                                case ErrorCode.NOT_LOGGED2:
                                    {
                                        MessageBox.Show("Nie jesteś zalogowany!");
                                        break;
                                    }
                            }

                            return;
                        }

                        if (answerValue.messageId == MessageId.PLAYER_LIST)
                        {
                            friendPlayer.ListFriendFromServer = (List<Player>)answerValue.message; // lista znajomych 
                            friendPlayer.ListOfFriends = new ObservableCollection<Player>(); // nicki na liście znajomych

                            for (int i = 0; i < friendPlayer.ListFriendFromServer.Count; i++)
                            {
                                friendPlayer.ListOfFriends.Add(friendPlayer.ListFriendFromServer[i]);
                            }

                            return;
                        }


                        MessageBox.Show("Dodano nowego znajomego");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Masz już takiego znajomego");
                    return;
                }
            }
        }

        private void deleteFriendButton_Click(object sender, RoutedEventArgs e)
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
                friendPlayer.listSearchngPlayers = (List<Player>)answerValue.message;
                for (int i = 0; i < friendPlayer.listSearchngPlayers.Count; i++)
                {
                    friendPlayer.GetPlayersFromServerToList(uint.Parse(friendPlayer.listSearchngPlayers[i].GetType().GetProperty("Id").GetValue(friendPlayer.listSearchngPlayers[i], null).ToString()), friendPlayer.listSearchngPlayers[i].GetType().GetProperty("Nick").GetValue(friendPlayer.listSearchngPlayers[i], null).ToString());
                }

                return;
            }
        }
    }
 }
