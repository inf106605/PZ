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

namespace SkyCrab.Classes.Menu.LoggedPlayer
{
    /// <summary>
    /// Interaction logic for RankLoggedPlayer.xaml
    /// </summary>
    public partial class RankLoggedPlayer : UserControl
    {
        public RankLoggedPlayer()
        {
            InitializeComponent();
        }

        private void ReturnMenuLoggedPlayer_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenuLoggedPlayer());
        }
    }
}
