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

namespace SkyCrab.Classes.Menu.Guest
{
    /// <summary>
    /// Interaction logic for CreateRoomForGuest.xaml
    /// </summary>
    public partial class CreateRoomForGuest : UserControl
    {
        ObservableCollection<String> labels;

        public CreateRoomForGuest()
        {
            InitializeComponent();
        }

        private void PlayAsGuestReturn_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new PlayAsGuest());
        }

        private void maxCountPlayersComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            labels = new ObservableCollection<String>();
            for (int i = 1; i <= 4; i++)
            {
                labels.Add(i.ToString());
            }

            maxCountPlayersComboBox.ItemsSource = labels;
            maxCountPlayersComboBox.SelectedIndex = 0;
        }

        private void GameAreaButton_Click(object sender, RoutedEventArgs e)
        {

            // validation data - room name , time limit , max count players

            var ifRoomNameCorrect = nameRommTextbox.Text;
          //  var ifTimeLimitCorrect = Regex.IsMatch("^[0-9]+$", TimeLimit.Tag.ToString());
            var ifMaxCountPlayersCorrect = maxCountPlayersComboBox.Text;

            if (ifRoomNameCorrect.Length > 15)
            {
                MessageBox.Show("Nazwa pokoju za długa!");
                nameRommTextbox.Text = "";
            }
           /* 
            if(!ifTimeLimitCorrect)
            {
                MessageBox.Show("Nieprawidłowa wartość");
            }
            */

               Switcher.Switch(new LobbyGameForGuest());
        }

        private void nameRommTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
