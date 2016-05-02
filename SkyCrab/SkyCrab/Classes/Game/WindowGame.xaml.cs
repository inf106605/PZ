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
            InitBindingPlayers();
            scrabbleGame = new ScrabbleGame();
            DataContext = scrabbleGame;
        }

        private void InitBindingPlayers()
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

            ScrabbleTilesSelectedFromRack scrabbleTilesSelectedFromRack = new ScrabbleTilesSelectedFromRack();
            ScrabbleTilesSelectedFromBoard scrabbleTilesSelectedFromBoard = new ScrabbleTilesSelectedFromBoard();

            foreach (var item in listViewRack.SelectedItems)
            {
                var scrabbleTileFromRack = scrabbleGame.scrabbleRack.SearchIdTile((ScrabbleRackTiles)item);
                scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Add(scrabbleTileFromRack);
            }

            /*
            for(int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
            {
                MessageBox.Show( scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].id + " " + scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Name + " " + scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Value);

            }
            */

            foreach (var item in scrabbleBoard.SelectedItems)
            {
                scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard.Add((ScrabbleSquare)item);
            }

            for(int i = 0; i < scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard.Count; i++)
            {
                MessageBox.Show(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Column + " " + scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Row);
            }
            
            if(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard.Count != scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count)
            {
                MessageBox.Show("Nie zaznaczono odpowiedniej liczby płytek lub pól");
                return;
            }

            for(int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
            {
                scrabbleGame.scrabbleBoard.SetScrabbleSquare(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].PositionInListBox, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Column , scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Row, scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Name, int.Parse(scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Value));
            }

            //ScrabbleBoard.Squares[112] = new ScrabbleSquare(int.Parse("7"), int.Parse("7"), "A", 1); // wpisywanie współrzędnych na planszy oraz literki i wartości którą chcemy wyświetlić                                         
            //usunięcie z kolekcji Rack zaznaczonych płytek 

            for (int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
                 scrabbleGame.scrabbleRack.RemoveTile(scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Id);
                 
        }
    } 
}
