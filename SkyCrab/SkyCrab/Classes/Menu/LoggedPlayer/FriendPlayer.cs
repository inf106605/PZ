using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SkyCrab.Classes.Menu.LoggedPlayer
{
    class FriendPlayer
    {
        ObservableCollection<string> ListOfPlayers = null;
        ObservableCollection<string> ListSearchingPlayers = null;

        public ObservableCollection<string> ListPlayers
        {
                get
                {
                     return ListOfPlayers;
                }
        }

        public ObservableCollection<string> ListBoxSearchingPlayers
        {
            get
            {
                return ListSearchingPlayers;
            }
        }

        public void GetPlayersFromServerToList(string nick)
        {
            ListSearchingPlayers.Add(nick);
        }

        public void ClearListBoxSearchingPlayers()
        {
            ListSearchingPlayers.Clear();
        }

        public FriendPlayer()
        {
            ListSearchingPlayers = new ObservableCollection<string>();

            var getFriendMsgAnswer = GetFriendsMsg.SyncPostGetFriends(App.clientConn, 1000);

            if (!getFriendMsgAnswer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = getFriendMsgAnswer.Value;

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
                List<Player> listPlayerTemp = (List<Player>)answerValue.message;
                ListOfPlayers = new ObservableCollection<string>();

                for(int i = 0; i < listPlayerTemp.Count; i++)
                {
                    ListOfPlayers.Add(listPlayerTemp[i].Nick);
                }

                return;
            }
        }

     }
 }
