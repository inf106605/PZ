using SkyCrab.Classes.Menu.LoggedPlayer;
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

namespace SkyCrab.Classes.Menu
{
    /// <summary>
    /// Interaction logic for PlayAsLoggedPlayer.xaml
    /// </summary>
    public partial class PlayAsLoggedPlayer : UserControl
    {
        public PlayAsLoggedPlayer()
        {
            InitializeComponent();
        } 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenuLoggedPlayer());
        }

        private void RoomCreateButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new CreateRoomForLoggedPlayers());
        }
    }
}
