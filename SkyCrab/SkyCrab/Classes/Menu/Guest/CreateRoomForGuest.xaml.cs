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
            Switcher.Switch(new LobbyGameForGuest());
        }
    }
}
