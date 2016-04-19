using SkyCrab.Menu;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace SkyCrab.Classes.Game
{
    /// <summary>
    /// Interaction logic for WindowGame.xaml
    /// </summary>
    public partial class WindowGame : UserControl
    {
        private List<ScrabblePlayers> ScrabblePlayers = null;

        public WindowGame()
        {
            InitializeComponent();
            InitBinding();
            DataContext = new ScrabbleGame();
        }

        private void InitBinding()
        {
            ScrabblePlayers = new List<ScrabblePlayers>();
            ScrabblePlayers.Add(new ScrabblePlayers("( Ty ) pleban325", 60, "0:05:59", 4));
            ScrabblePlayers.Add(new ScrabblePlayers("paterak195", 45, "0:05:59", 5));
            ScrabblePlayers.Add(new ScrabblePlayers("sebaalex", 120, "0:05:59", 7));
            ScrabblePlayers.Add(new ScrabblePlayers("ziomeczek", 15, "0:05:59", 2));
            ListPlayers.ItemsSource = ScrabblePlayers;
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }
    } 
}
