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

        private ScrabbleGame scrabbleGame = null;

        public WindowGame()
        {
            InitializeComponent();
            InitBinding();
            scrabbleGame = new ScrabbleGame();
            DataContext = scrabbleGame;
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

            Dictionary<int,string> idAndLetter = new Dictionary<int, string>();

            foreach (var item in listViewRack.SelectedItems)
            {
                var key = int.Parse(item.GetType().GetProperty("Id").GetValue(item, null).ToString());
                var value = item.GetType().GetProperty("Name").GetValue(item, null).ToString();
                idAndLetter.Add(key, value);
                MessageBox.Show(key + " " + value);
            }

            // sortowanie słownika po kluczu - id elementu 

            var list_keys = idAndLetter.Keys.ToList();
            // list_keys.Sort();
            List<string> list_tiles = new List<string>();
            //MessageBox.Show("Lista zaznaczonych na stojaku");

            foreach (var key in list_keys)
            {
                //MessageBox.Show(key + " : " + idAndLetter[key]);
                list_tiles.Add(idAndLetter[key]);
            }

            //MessageBox.Show("Koniec zaznaczonych na stojaku");
            
            List<int> Columns = new List<int>();
            List<int> Rows = new List<int>();
            List<int> PositionsListBox = new List<int>();

            foreach (var item in scrabbleBoard.SelectedItems)
            {
                var Column = int.Parse(item.GetType().GetProperty("Row").GetValue(item, null).ToString()); // pobieranie współrzędnej x
                var Row = int.Parse(item.GetType().GetProperty("Column").GetValue(item, null).ToString()); // pobieranie współrzędnej y
                var listBoxPosition = int.Parse(item.GetType().GetProperty("PositionInListBox").GetValue(item, null).ToString()); // współrzędna pozycji w listbox'ie

                Columns.Add(Column);
                Rows.Add(Row);
                PositionsListBox.Add(listBoxPosition);
                MessageBox.Show(Column + " " + Row); // wyświetlenie pozycji x , y
            }

            // sortowanie

            Columns.Sort();
            Rows.Sort();
            PositionsListBox.Sort();

            for(int i = 0; i < list_tiles.Count; i++)
            {
                scrabbleGame.scrabbleBoard.SetScrabbleSquare(PositionsListBox[i], Columns[i], Rows[i], list_tiles[i], 1);
            }

            //ScrabbleBoard.Squares[112] = new ScrabbleSquare(int.Parse("7"), int.Parse("7"), "A", 1); // wpisywanie współrzędnych na planszy oraz literki i wartości którą chcemy wyświetlić                                         
            //usunięcie z kolekcji Rack zaznaczonych płytek 

            for (int i = 0; i < list_keys.Count; i++)
                 scrabbleGame.scrabbleRack.RemoveTile(list_keys[i]);

        }
    } 
}
