using SkyCrab.Classes.Menu;
using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Chats;
using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Players;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Game;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
using SkyCrab.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SkyCrab.Classes.ScrabbleGameFolder
{
    /// <summary>
    /// Interaction logic for WindowGame.xaml
    /// </summary>
    public partial class WindowGame : UserControl
    {
        private List<ScrabblePlayers> ScrabblePlayers = null;

        private ScrabbleGame scrabbleGame = null;
        DispatcherTimer dispatcherTimerChat;


        public WindowGame()
        {
            InitializeComponent();
            scrabbleGame = new ScrabbleGame();
            InitBindingPlayers();
            DataContext = scrabbleGame;

            // co 3 sekundy następuje odświeżanie chatu
            dispatcherTimerChat = new DispatcherTimer();
            dispatcherTimerChat.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimerChat.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimerChat.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            ReadChat.Text = SkyCrabGlobalVariables.MessagesLog;

            if(SkyCrabGlobalVariables.isGetNewTile)
            {
                for(int i = 0; i < SkyCrabGlobalVariables.newTile.letters.Count;i++)
                {
                    TileOnRack temp = new TileOnRack(new Tile(SkyCrabGlobalVariables.newTile.letters[i]));
                    scrabbleGame.RackTiles.Add(new ScrabbleRackTiles(temp));
                } 

                SkyCrabGlobalVariables.isGetNewTile = false;
            }

            if(SkyCrabGlobalVariables.lostLetters.letters != null)
            {
                if(SkyCrabGlobalVariables.lostLetters.playerId != SkyCrabGlobalVariables.player.Id) // aktualizacja woreczka u graczy o podanym ID - woreczek gracza który dokonał wymiany jest aktualizowany od razu
                {
                    scrabbleGame.game.Puoches[0].InsertAnyTiles((uint)SkyCrabGlobalVariables.lostLetters.letters.Count);
                }

                SkyCrabGlobalVariables.lostLetters.letters = null;
            }

            if (SkyCrabGlobalVariables.isMyRound)
            {
                Play.IsEnabled = true;
                Exchange.IsEnabled = true;
                Pass.IsEnabled = true;
            }
            else
            {
                Play.IsEnabled = false;
                Exchange.IsEnabled = false;
                Pass.IsEnabled = false;
            }

            CommandManager.InvalidateRequerySuggested();
        }

  

        private void InitBindingPlayers()
        {

             PlayerInGame[] playerInGame = scrabbleGame.game.Players;
            ScrabblePlayers = new List<ScrabblePlayers>();

            foreach (var item in playerInGame)
            {
                ScrabblePlayers.Add(new ScrabblePlayers(item.Player.Nick, item.Points, "0:00", item.Rack.Tiles.Count));
            }

            Rack rack = new Rack(); // klasa piotra , do zrobienia
            ListPlayers.ItemsSource = ScrabblePlayers;
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            var joinToRoomMsgAnswer = LeaveRoomMsg.SyncPost(App.clientConn, 1000);

            if (!joinToRoomMsgAnswer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = joinToRoomMsgAnswer.Value;

            if (answerValue.messageId == MessageId.ERROR)
            {
                ErrorCode errorCode = (ErrorCode)answerValue.message;

                switch (errorCode)
                {
                    case ErrorCode.NOT_IN_ROOM:
                        {
                            MessageBox.Show("Nie ma Cię w pokoju!");
                            break;
                        }
                }

                return;
            }

            if (answerValue.messageId == MessageId.OK)
            {
                MessageBox.Show("Opuściłeś pokój!");
                SkyCrabGlobalVariables.room = null;
                SkyCrabGlobalVariables.MessagesLog = "";
                SkyCrabGlobalVariables.isGame = false;
                dispatcherTimerChat.Stop();
                if (SkyCrabGlobalVariables.player.Profile == null)
                {
                    Switcher.Switch(new MainMenu());
                }
                else
                {
                    Switcher.Switch(new MainMenuLoggedPlayer());
                }
            }
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

            // WALIDACJA ZAZNACZONYCH PŁYTEK NA PLANSZY I STOJAKU

            bool ifVerticalPositionTiles = true;
            bool ifHorizontalPositionTiles = true;

            for(int i = 0; i < scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard.Count; i++)
            {
                if(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].isValue)
                {
                    MessageBox.Show("W tym miejscu już istnieje płytka!");
                    return;
                }

                if(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[0].Column != scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Column)
                {
                    ifHorizontalPositionTiles = false;
                }
                if(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[0].Row != scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Row)
                {
                    ifVerticalPositionTiles = false;
                }

            }

            if (scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard.Count > 1)
            {

                if (!((ifVerticalPositionTiles && !ifHorizontalPositionTiles) || (!ifVerticalPositionTiles && ifHorizontalPositionTiles)))
                {
                    MessageBox.Show("Płytki są nieprawidłowo układane. Tylko poziomo lub pionowo");
                    return;
                }
            }
            if(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard.Count != scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count)
            {
                MessageBox.Show("Nie zaznaczono odpowiedniej liczby płytek lub pól");
                return;
            }

            // KONIEC WALIDACJI


            // ustawienie płytek na planszy

            for(int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
            {
                scrabbleGame.scrabbleBoard.SetScrabbleSquare(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].PositionInListBox, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Column , scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Row, scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Name, int.Parse(scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Value));
            }

            // usunięcie płytek ze stojaka

            for (int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
                 scrabbleGame.scrabbleRack.RemoveTile(scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Id);

            // uzupełnienie stojaka o ilość wyłożonych płytek. W przeciwnym przypadku jeżeli jest 
            //ich mniej w woreczku to wyciągnięcie pozostałych

            //MessageBox.Show("Zostało: " + scrabbleGame.pouch.Count + " płytek!");

            /*

            if (scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count > scrabbleGame.game.Puoches[0].Count)
            {
                for(int i=0; i < scrabbleGame.game.Puoches[0].Count; i++)
                {
                    TileOnRack temp = new TileOnRack(scrabbleGame.game.Puoches[0].DrawRandowmTile());
                    scrabbleGame.RackTiles.Add(new ScrabbleRackTiles(temp));
                }
            }

            else
            {
                for(int i=0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
                {
                    TileOnRack temp = new TileOnRack(scrabbleGame.game.Puoches[0].DrawRandowmTile());
                    scrabbleGame.RackTiles.Add(new ScrabbleRackTiles(temp));
                }
            }
            */

            // Aktualizacja ilości pozostałych płytek

            LeftTilesInPouch.Text = "Pozostało " + scrabbleGame.game.Puoches[0].Count + " płytek";

        }

        private void Exchange_Click(object sender, RoutedEventArgs e)
        {
            ScrabbleTilesSelectedFromRack scrabbleTilesSelectedFromRack = new ScrabbleTilesSelectedFromRack();
            
            // utworzenie listy do przetrzymywania płytek, które chce się wymienić
            List<LetterWithNumber> letterWithNumber = new List<LetterWithNumber>();

            // sprawdzanie czy ilość zaznaczonych płytek nie przekracza ilości pozostałych płytek w woreczku

            if (listViewRack.SelectedItems.Count > scrabbleGame.game.Puoches[0].Count)
            {
                MessageBox.Show("Nie ma aż tylu płytek w woreczku do wymiany!");
                return;
            }

            // weryfikacja czy w woreczku jest 7 lub mniej płytek dla reguły Wymiany

            if(scrabbleGame.game.Puoches[0].Count <= 7)
            {
                if(SkyCrabGlobalVariables.room.room.Rules.restrictedExchange.value)
                {
                    MessageBox.Show("Nie można już dokonać wymiany!");
                    return;
                }
            }

            // dodanie zaznaczonych płytek do tymczasowej listy

            foreach (var item in listViewRack.SelectedItems)
            {
                var scrabbleTileFromRack = scrabbleGame.scrabbleRack.SearchIdTile((ScrabbleRackTiles)item);
                scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Add(scrabbleTileFromRack);
                LetterWithNumber tempLetter = new LetterWithNumber();
                TileOnRack tileOnRackTemp = (TileOnRack)item.GetType().GetProperty("tile").GetValue(item);
                tempLetter.letter = tileOnRackTemp.Tile.Letter;
                letterWithNumber.Add(tempLetter);

            }

            // usunięcie płytek ze stojaka 

            for (int i =0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
            {
                scrabbleGame.scrabbleRack.RemoveTile(scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Id);
            }

            // komunikat do wymiany płytek

            var chatMsgAnswer = ExchangeTilesMsg.SyncPost(App.clientConn, letterWithNumber, 1000);

            if (!chatMsgAnswer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = chatMsgAnswer.Value;

            if (answerValue.messageId == MessageId.ERROR)
            {
                ErrorCode errorCode = (ErrorCode)answerValue.message;

                switch (errorCode)
                {
                    case ErrorCode.NOT_IN_GAME3:
                        {
                            MessageBox.Show("Nie ma Cię w grze!");
                            break;
                        }
                    case ErrorCode.NOT_YOUR_TURN2:
                        {
                            MessageBox.Show("To nie jest Twoja kolejka!");
                            break;
                        }
                    case ErrorCode.INCORRECT_MOVE2:
                        {
                            MessageBox.Show("Nieprawidłowy ruch!");
                            break;
                        }
                }
                return;
            }

            if(answerValue.messageId == MessageId.OK)
            {
               /* List<LetterWithNumber> getFromServerLetterWithNumber = new List<LetterWithNumber>();
                getFromServerLetterWithNumber = (List<LetterWithNumber>)answerValue.message;

                for (int i = 0; i < getFromServerLetterWithNumber.Count; i++)
                {
                    TileOnRack tileOnRackTemp = new TileOnRack(new Tile(getFromServerLetterWithNumber[i].letter));
                    scrabbleGame.RackTiles.Add(new ScrabbleRackTiles(tileOnRackTemp));
                }

            */
            }
            
            /*

            // losowanie płytek z woreczka

            for (int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
            {
                TileOnRack temp = new TileOnRack(scrabbleGame.game.Puoches[0].DrawRandowmTile());
                scrabbleGame.RackTiles.Add(new ScrabbleRackTiles(temp));
            }

            */
            
            /*
                        
            // włożenie starych płytek do woreczka
            for (int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
            {
                Letter temp = new Letter(scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].tile.Tile.Letter.character, scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].tile.Tile.Letter.points);
                scrabbleGame.pouch.InsertTile(temp);
            }
            
            */
        }

        private void WriteChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (WriteChat.Text.Length > LengthLimit.ChatMessage.Max)
            {
                MessageBox.Show("Wiadomość jest za długa!");
                return;
            }

            if (e.Key == Key.Enter && WriteChat.Text.Length > 0)
            {

                if (WriteChat.Text.Length < 1)
                    return;

                ChatMessage chatMessage = new ChatMessage();
                chatMessage.Message = WriteChat.Text;
                var chatMsgAnswer = ChatMsg.SyncPost(App.clientConn, chatMessage, 1000);

                if (!chatMsgAnswer.HasValue)
                {
                    MessageBox.Show("Brak odpowiedzi od serwera!");
                    return;
                }

                var answerValue = chatMsgAnswer.Value;

                if (answerValue.messageId == MessageId.ERROR)
                {
                    ErrorCode errorCode = (ErrorCode)answerValue.message;

                    switch (errorCode)
                    {
                        case ErrorCode.NOT_IN_ROOM4:
                            {
                                MessageBox.Show("Nie ma Cię w pokoju!");
                                break;
                            }
                    }
                    return;
                }

                WriteChat.Text = "";
            }
        }

        private void Pass_Click(object sender, RoutedEventArgs e)
        {

            var passMsgAnswer = PassMsg.SyncPost(App.clientConn,1000);


            if (!passMsgAnswer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = passMsgAnswer.Value;

            if (answerValue.messageId == MessageId.ERROR)
            {
                ErrorCode errorCode = (ErrorCode)answerValue.message;

                switch (errorCode)
                {
                    case ErrorCode.NOT_IN_GAME4:
                        {
                            MessageBox.Show("Nie ma Cię w grze!");
                            break;
                        }
                    case ErrorCode.NOT_YOUR_TURN3:
                        {
                            MessageBox.Show("Kolejka innego gracza!");
                            break;
                        }
                }
                return;
            }
            if(answerValue.messageId == MessageId.OK)
            {
                SkyCrabGlobalVariables.isMyRound = false;
            }
        }
    } 
}
