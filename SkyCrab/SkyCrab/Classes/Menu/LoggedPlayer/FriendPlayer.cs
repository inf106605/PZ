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
        public ObservableCollection<Player> ListOfFriends = null; // lista znajomych
        public List<Player> ListFriendFromServer = null;
        public ObservableCollection<Player> ListSearchingPlayers = null; // lista wszystkich graczy , których można dodać do znajomych
        public List<Player> listSearchngPlayers = null;

        public ObservableCollection<Player> ListPlayers // lista znajomych ( bindowanie )
        {
                get
                {
                     return ListOfFriends;
                }
        }

        public ObservableCollection<Player> ListBoxSearchingPlayers // lista wyszukiwanych graczy na podstawie wpisanego tekstu ( bindowanie ) 
        {
            get
            {
                return ListSearchingPlayers;
            }
        }

        public List<Player> ListPlayer // lista znajomych z serwera 
        {
            get
            {
                return ListFriendFromServer;
            }
        }



        public void GetPlayersFromServerToList(uint id, string nick)
        {
            ListSearchingPlayers.Add(new Player(id,false,nick));
        }

        public void AddPlayerToFriend(uint id,string nick)
        {
            ListOfFriends.Add(new Player(id,false,nick));
        }

        public void ClearListBoxSearchingPlayers()
        {
            ListSearchingPlayers.Clear();
        }

        public FriendPlayer()
        {
            ListSearchingPlayers = new ObservableCollection<Player>();

            ListFriendFromServer = new List<Player>();

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
                ListFriendFromServer = (List<Player>)answerValue.message; // lista znajomych 
                ListOfFriends = new ObservableCollection<Player>(); // nicki na liście znajomych

                for(int i = 0; i < ListFriendFromServer.Count; i++)
                {
                    ListOfFriends.Add(ListFriendFromServer[i]);
                }

                return;
            }
        }

     }
 }
