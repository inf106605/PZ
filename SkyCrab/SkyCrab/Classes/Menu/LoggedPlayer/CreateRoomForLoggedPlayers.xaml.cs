﻿using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Friends;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
using SkyCrab.SkyCrabClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CreateRoomForLoggedPlayers.xaml
    /// </summary>
    public partial class CreateRoomForLoggedPlayers : UserControl
    {
        ObservableCollection<String> labels; //  values of selectbox ( count players )
        ObservableCollection<String> TimeLimitMinorSecondList;

        public CreateRoomForLoggedPlayers()
        {
            InitializeComponent();
            TimeLimitMinorSecondList = new ObservableCollection<String>();
            TimeLimitMinorSecondList.Add("s");
            TimeLimitMinorSecondList.Add("min");
            TimeLimitMinorSecond.ItemsSource = TimeLimitMinorSecondList;
            TimeLimitMinorSecond.SelectedIndex = 0;
        }

        private void PlayAsLoggedPlayerReturn_Click(object sender, RoutedEventArgs e)
        {
            SkyCrabGlobalVariables.room = null;
            Switcher.Switch(new PlayAsLoggedPlayer());
        }

        private void maxCountPlayersComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            labels = new ObservableCollection<String>();
            for (int i = 1; i <= 4; i++)
            {
                labels.Add(i.ToString());
            }

            maxCountPlayersComboBox.ItemsSource = labels;
            maxCountPlayersComboBox.SelectedIndex = 1;
        }

        private void GameAreaButton_Click(object sender, RoutedEventArgs e)
        {
            Room room = new Room();

            // walidacja nazwy pokoju

            if (LengthLimit.RoomName.Max < nameRommTextbox.Text.Length)
            {
                MessageBox.Show("Nazwa pokoju jest za długa!");
                return;
            }

            if (LengthLimit.RoomName.Min > nameRommTextbox.Text.Length)
            {
                MessageBox.Show("Nazwa pokoju jest za krótka!");
                return;
            }

            room.Name = nameRommTextbox.Text;

            // walidacja rodzaju pokoju

            if(!(publicRoomRadioButton.IsChecked.Value || friendsRoomRadioButton.IsChecked.Value || privateRoomRadioButton.IsChecked.Value))
            {
                MessageBox.Show("Wybierz rodzaj pokoju!");
                return;
            }

            if (publicRoomRadioButton.IsChecked == true)
            {
                room.Type = RoomType.PUBLIC;
            }

            if(friendsRoomRadioButton.IsChecked == true)
            {
                room.Type = RoomType.FRIENDS;
            }

            if(privateRoomRadioButton.IsChecked == true)
            {
                room.Type = RoomType.PRIVATE;
            }

            // walidacja czasu gry

            if (TimeLimit.SelectedValue != null || TimeLimit.Text != null)
            {
                Regex regex = new Regex(@"^\d+$");
                Match match = regex.Match(TimeLimit.Text);

                if ((string)TimeLimit.Text == "Brak limitu")
                    room.Rules.maxTurnTime.value = 0;
                else if (!match.Success)
                {
                    MessageBox.Show("Nieprawidłowa wartość czasu gry!");
                    return;
                }
                else if (int.Parse((string)TimeLimit.Text) > 0)
                    if (TimeLimitMinorSecond.Text == "s")
                        room.Rules.maxTurnTime.value = uint.Parse((string)TimeLimit.Text);
                    else
                        room.Rules.maxTurnTime.value = uint.Parse((string)TimeLimit.Text) * 60;
            }

            // przypisanie maksymalnej liczby graczy

            if (byte.Parse(maxCountPlayersComboBox.Text) < 1 || byte.Parse(maxCountPlayersComboBox.Text) > 4)
            {
                return;
            }

            room.Rules.maxPlayerCount.value = byte.Parse(maxCountPlayersComboBox.Text);

            if(RulesFive.IsChecked ?? true)
            {
                room.Rules.fivesFirst.value = true;
            }
            else
            {
                room.Rules.fivesFirst.value = false;
            }

            if(RulesExchange.IsChecked ?? true)
            {
                room.Rules.restrictedExchange.value = true;
            }
            else
            {
                room.Rules.restrictedExchange.value = false;
            }


            var createRoomMsgAnswer = CreateRoomMsg.SyncPost(App.clientConn, room, 1000);
            

            if (!createRoomMsgAnswer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = createRoomMsgAnswer.Value;

            if (answerValue.messageId == MessageId.ERROR)
            {
                ErrorCode errorCode = (ErrorCode)answerValue.message;

                switch (errorCode)
                {
                    case ErrorCode.ALREADY_IN_ROOM:
                        {
                            MessageBox.Show("Juz jesteś w pokoju!");
                            break;
                        }
                    case ErrorCode.INVALID_RULES:
                        {
                            MessageBox.Show("Nieznane reguły gry!");
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
