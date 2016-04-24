using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
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
            ScrabblePlayers.Add(new ScrabblePlayers("pleban325", 60, "0:06:14", 4));
            ScrabblePlayers.Add(new ScrabblePlayers("paterak195", 45, "0:05:32", 5));
            ScrabblePlayers.Add(new ScrabblePlayers("sebaalex", 120, "0:06:47", 7));
            ScrabblePlayers.Add(new ScrabblePlayers("ziomeczek", 15, "0:07:31", 2));
            Rack rack = new Rack(); // klasa piotra , do zrobienia
            ListPlayers.ItemsSource = ScrabblePlayers;
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            // symulacja wyłożenia płytek

            List<int> listTiles = new List<int>();

            foreach (var item in listViewRack.SelectedItems)
            {
                var x = item.GetType().GetProperty("Id").GetValue(item, null).ToString();
                listTiles.Add(int.Parse(x));
                MessageBox.Show(x);
            }

            // sortowanie listy z id elementów 

            listTiles.Sort();

            // wyświetlenie id usuwanych elementów

            string a = "";

            for (int i = 0; i < listTiles.Count; i++)
                a += listTiles[0] + " ";

            MessageBox.Show("Lista zaznaczonych: " + a);

            // usunięcie z kolekcji Rack zaznaczonych płytek 

            for (int i = 0; i < listTiles.Count; i++)
                ScrabbleRack.RackTiles.Remove(ScrabbleRack.RackTiles.Where(temp => temp.Id == listTiles[i]).Single());
            

        }
    } 
}
