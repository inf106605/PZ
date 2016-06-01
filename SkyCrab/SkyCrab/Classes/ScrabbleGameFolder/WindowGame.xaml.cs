using SkyCrab.Classes.Menu;
using SkyCrab.Classes.Menu.Guest;
using SkyCrab.Classes.Menu.LoggedPlayer;
using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Chats;
using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Players;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Game;
using SkyCrab.Connection.PresentationLayer.Messages.Game.Commands;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
using SkyCrab.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
        string defineBlankValue = "";
        string defineBlankTwoValue = "";
        int isExistBlankCounter = 0;
        int isExistBlankTwoCounter = 0;
        int countBlank;

        public WindowGame()
        {
            InitializeComponent();
            scrabbleGame = new ScrabbleGame();
            //InitBindingPlayers();
            DataContext = scrabbleGame;

            // co 3 sekundy następuje odświeżanie chatu
            dispatcherTimerChat = new DispatcherTimer();
            dispatcherTimerChat.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimerChat.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimerChat.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(!SkyCrabGlobalVariables.isGame || SkyCrabGlobalVariables.game == null)
            {
                dispatcherTimerChat.Stop();
                if (SkyCrabGlobalVariables.player.IsGuest)
                    Switcher.Switch(new LobbyGameForGuest());
                else
                    Switcher.Switch(new LobbyGameForLoggedPlayer());
                return;
            }

            // Updating the Label which displays the current second

            if (ReadChat.Text != SkyCrabGlobalVariables.MessagesLog)
            {
                ReadChat.Text = SkyCrabGlobalVariables.MessagesLog;
                ReadChat.SelectionStart = ReadChat.Text.Length; // przewijanie scrollbara automatycznie w dół - 1
                ReadChat.ScrollToEnd(); //  // przewijanie scrollbara automatycznie w dół - 2
                System.Media.SystemSounds.Asterisk.Play();
            }

            // aktualizowanie listy graczy 
            InitBindingPlayers();

            LeftTilesInPouch.Text = "Pozostało płytek: " + scrabbleGame.game.Puoches[0].Count;

            if (SkyCrabGlobalVariables.isGetNewTile)
            {
                for(int i = 0; i < SkyCrabGlobalVariables.newTile.letters.Count;i++)
                {
                    TileOnRack temp = new TileOnRack(new Tile(SkyCrabGlobalVariables.newTile.letters[i]));
                    scrabbleGame.RackTiles.Add(new ScrabbleRackTiles(temp));
                }
                scrabbleGame.game.Puoches[0].RemoveAnyTiles((uint)SkyCrabGlobalVariables.newTile.letters.Count);
                SkyCrabGlobalVariables.isGetNewTile = false;

            }

            if(SkyCrabGlobalVariables.isPlacedTilesByPlayers)
            {
                for (int i = 0; i < SkyCrabGlobalVariables.TilesToPlaceByPlayers.tilesToPlace.Count; i++)
                {
                    int x = SkyCrabGlobalVariables.TilesToPlaceByPlayers.tilesToPlace[i].position.x;
                    int y = SkyCrabGlobalVariables.TilesToPlaceByPlayers.tilesToPlace[i].position.y;
                    int PositionInBoardList = ((StandardBoard.RightBottom_.y+1) * y) + x;
                    string name = SkyCrabGlobalVariables.TilesToPlaceByPlayers.tilesToPlace[i].tile.Letter.character.ToString();
                    int value = (int)SkyCrabGlobalVariables.TilesToPlaceByPlayers.tilesToPlace[i].tile.Letter.points;

                    scrabbleGame.scrabbleBoard.SetScrabbleSquare(PositionInBoardList,x,y, name , value);
                }
                SkyCrabGlobalVariables.isPlacedTilesByPlayers = false;
            }


            if(SkyCrabGlobalVariables.anotherPlayersGetNewTile)
            {
                scrabbleGame.game.Puoches[0].RemoveAnyTiles(SkyCrabGlobalVariables.anotherPlayersGetNewTileCount);
                SkyCrabGlobalVariables.anotherPlayersGetNewTileCount = 0;
                SkyCrabGlobalVariables.anotherPlayersGetNewTile = false;
            }

            if(SkyCrabGlobalVariables.lostLetters.letters != null)
            {
                if (SkyCrabGlobalVariables.lostLetters.backToPouch)
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
                SkyCrabGlobalVariables.myMaxTurnTime = SkyCrabGlobalVariables.myMaxTurnTime.Subtract(TimeSpan.FromSeconds(1));
                if(SkyCrabGlobalVariables.game.Room.Rules.maxTurnTime.value == 0)
                    LeftTimeMyRound.Text = "Pozostały czas: " +  Environment.NewLine + "Brak limitu";
                else
                    LeftTimeMyRound.Text = "Pozostały czas: " + SkyCrabGlobalVariables.myMaxTurnTime.ToString(@"hh\:mm\:ss");
            }
            else
            {
                Play.IsEnabled = false;
                Exchange.IsEnabled = false;
                Pass.IsEnabled = false;
                TimeSpan tempTimeSpan = TimeSpan.FromSeconds(SkyCrabGlobalVariables.game.Room.Rules.maxTurnTime.value);
                LeftTimeMyRound.Text = "Pozostały czas: " + tempTimeSpan.ToString(@"hh\:mm\:ss");
            }
            
            CommandManager.InvalidateRequerySuggested();


        }


        private void InitBindingPlayers()
        {
            if (SkyCrabGlobalVariables.game == null) return;

             PlayerInGame[] playerInGame = scrabbleGame.game.Players;
            ScrabblePlayers = new List<ScrabblePlayers>();

            int maxLength = 0;

            foreach (var item in SkyCrabGlobalVariables.game.Players)
            {
                if (item.Walkover)
                    ScrabblePlayers.Add(new ScrabblePlayers("✖ " + item.Player.Nick, item.Points, "0:00", item.Rack.Tiles.Count));
                else
                {
                    if(SkyCrabGlobalVariables.game.CurrentPlayer.Player == item.Player)
                         ScrabblePlayers.Add(new ScrabblePlayers("☞ " + item.Player.Nick, item.Points, "0:00", item.Rack.Tiles.Count));
                    else
                        ScrabblePlayers.Add(new ScrabblePlayers(item.Player.Nick, item.Points, "0:00", item.Rack.Tiles.Count));
                }

                if (item.Player.Nick.Length > maxLength)
                    maxLength = item.Player.Nick.Length;

            }

            //dynamiczne ustawienie szerokości kolumny z nickiem gracza
            minPlayerHeaderLength.Width = 12*maxLength + 5;
            ListPlayers.ItemsSource = ScrabblePlayers;
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy chcesz opuścić pokój rozgrywki ? ", "Opuszczanie pokoju rozgrywki", MessageBoxButton.OKCancel);
            switch (result)
            {
                case MessageBoxResult.OK:
                    break;
                case MessageBoxResult.Cancel:
                    return;
            }

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
            TilesToPlace tilesToPlace = new TilesToPlace(); // obiekt klasy służącej do przesłania pozycji wykładanej płytki na plansżę, oraz wartości danej płytki
            tilesToPlace.lettersFromRack = new List<LetterWithNumber>();
            tilesToPlace.tilesToPlace = new List<TileOnBoard>();

            foreach (var item in listViewRack.SelectedItems)
            {
                var scrabbleTileFromRack = scrabbleGame.scrabbleRack.SearchIdTile((ScrabbleRackTiles)item);
                scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Add(scrabbleTileFromRack);
            }


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

            bool existBlankOnRack = false;
            countBlank = 0;

            for(int i=0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count;i++)
            {
                if (scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].tile.Tile.Blank)
                {
                    existBlankOnRack = true;
                    ++countBlank;
                }
            }


            if(existBlankOnRack)
            {
                
                if(isExistBlankCounter == 0 || (isExistBlankTwoCounter == 0 && isExistBlankCounter == 1 && countBlank == 2))
                {
                    DialogReplacement.Visibility = Visibility.Visible;
                    scrabbleBoard.Visibility = Visibility.Hidden;
                    return;
                }

                else
                {
                    DialogReplacement.Visibility = Visibility.Hidden;
                    scrabbleBoard.Visibility = Visibility.Visible;
                    existBlankOnRack = false;
                }
            }

            int tempCountBlanks = 0;

            for(int i=0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
            {
                if (!scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].tile.Tile.Blank)
                {
                    LetterWithNumber letterWithNumber = new LetterWithNumber();
                    letterWithNumber.letter = scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].tile.Tile.Letter;
                    TileOnBoard tileOnBoard = new TileOnBoard();
                    tileOnBoard.tile = scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].tile.Tile;
                    tileOnBoard.position = new PositionOnBoard(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Column, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Row);
                    tilesToPlace.lettersFromRack.Add(letterWithNumber);
                    tilesToPlace.tilesToPlace.Add(tileOnBoard);
                }
                else
                {
                    ++tempCountBlanks;
                    LetterWithNumber letterWithNumber = new LetterWithNumber();
                    letterWithNumber.letter = LetterSet.BLANK;
                    TileOnBoard tileOnBoard = new TileOnBoard();
                    if(tempCountBlanks == 1 )
                        tileOnBoard.tile = new Tile(true, PolishLetterSet.GetLetter(char.Parse(defineBlankValue)));
                    else if(tempCountBlanks == 2)
                        tileOnBoard.tile = new Tile(true, PolishLetterSet.GetLetter(char.Parse(defineBlankTwoValue)));
                    tileOnBoard.position = new PositionOnBoard(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Column, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Row);
                    tilesToPlace.lettersFromRack.Add(letterWithNumber);
                    tilesToPlace.tilesToPlace.Add(tileOnBoard);
                }
            }


            var placeTilesMsgAnswer = PlaceTilesMsg.SyncPost(App.clientConn, tilesToPlace, 1000);

            if (!placeTilesMsgAnswer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = placeTilesMsgAnswer.Value;

            if (answerValue.messageId == MessageId.ERROR)
            {
                bool existBlank = false;

                for (int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
                {
                    if (scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].tile.Tile.Blank)
                    {
                        existBlank = true;
                    }
                } 

                if(existBlank)
                {
                    if(isExistBlankCounter == 1 && isExistBlankTwoCounter == 0)
                    {
                        --isExistBlankCounter;
                    }
                    if(isExistBlankCounter == 1 && isExistBlankTwoCounter == 1)
                    {
                        --isExistBlankCounter;
                        --isExistBlankTwoCounter;
                    }
                }

                ErrorCode errorCode = (ErrorCode)answerValue.message;

                switch (errorCode)
                {
                    case ErrorCode.LETTERS_NOT_FROM_RACK:
                        {
                            MessageBox.Show("Próbujesz zdjąć nieistniejące płytki");
                            break;
                        }
                    case ErrorCode.LETTERS_NOT_MATH:
                        {
                            MessageBox.Show("Błąd wykładania płytek na planszy");
                            break;
                        }
                    case ErrorCode.WORD_NOT_IN_LINE:
                        {
                            MessageBox.Show("Słowa nie są ułożone w jednej linii!");
                            break;
                        }
                    case ErrorCode.NOT_IN_STARTING_SQUARE:
                        {
                            MessageBox.Show("Musisz wyłożyć płytki tak aby przechodziły przez środek planszy!");
                            break;
                        }

                    case ErrorCode.NOT_IN_GAME2:
                        {
                            MessageBox.Show("Nie ma Cię w grze!");
                            break;
                        }
                    case ErrorCode.TOO_LESS_TILES:
                        {
                            MessageBox.Show("Położono za mało płytek");
                            break;
                        }
                    case ErrorCode.FIVES_FIRST_VIOLATION:
                        {
                            MessageBox.Show("Wyłozono za mało płytek");
                            break;
                        }
                    case ErrorCode.NOT_YOUR_TURN:
                        {
                            MessageBox.Show("To nie jest Twoja kolejka!");
                            break;
                        }
                    case ErrorCode.INCORRECT_MOVE2:
                        {
                            MessageBox.Show("Nieprawidłowy ruch!");
                            break;
                        }
                    case ErrorCode.NOT_ADJANCENT:
                        {
                            MessageBox.Show("Płytki nie przylegają do siebie!");
                            break;
                        }
                    case ErrorCode.NOT_CONTINUOUS:
                        {
                            MessageBox.Show("Płytki nie są ułożone w sposób ciągły!");
                            break;
                        }
                    case ErrorCode.INCORECT_WORD:
                        {
                            MessageBox.Show("Niepoprawne słowo!");
                            break;
                        }
                    default:
                        {
                            MessageBox.Show("Nieznany błąd: " + errorCode);
                            break;
                        }
                }
                return;
            }

            if (answerValue.messageId == MessageId.OK)
            {
                // KONIEC WALIDACJI

                // jeśli ułożyłem 1 blanka , to muszę zrobić dekrementację licznika, żeby w razie kiedy wylosuję go po raz drugi, mógł sobie przypisać 

                bool existBlank = false;

                for (int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
                {
                    if (scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].tile.Tile.Blank)
                    {
                        existBlank = true;
                    }
                }

                if (existBlank)
                {
                    if (isExistBlankCounter == 1 && isExistBlankTwoCounter == 0) // działający fragment
                    {
                        --isExistBlankCounter;
                    }
                    if (isExistBlankCounter == 1 && isExistBlankTwoCounter == 1)
                    {
                        --isExistBlankCounter;
                        --isExistBlankTwoCounter;
                    }
                }

                //**************************************************************************************************************************************

                // ustawienie płytek na planszy
                int countBlanks = 0;
                for (int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
                {
                    if (!scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].tile.Tile.Blank)
                        scrabbleGame.scrabbleBoard.SetScrabbleSquare(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].PositionInListBox, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Column, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Row, scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Name, int.Parse(scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Value));
                    else
                    {
                        countBlanks++;
                        if(countBlanks==1)
                            scrabbleGame.scrabbleBoard.SetScrabbleSquare(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].PositionInListBox, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Column, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Row, defineBlankValue, 0);
                        else if(countBlanks==2)
                            scrabbleGame.scrabbleBoard.SetScrabbleSquare(scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].PositionInListBox, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Column, scrabbleTilesSelectedFromBoard.scrabbleTilesSelectedFromBoard[i].Row, defineBlankTwoValue, 0);
                    }
                }
                // usunięcie płytek ze stojaka

                for (int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
                    scrabbleGame.scrabbleRack.RemoveTile(scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Id);
            }

            

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

                // usunięcie płytek ze stojaka 

                for (int i = 0; i < scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack.Count; i++)
                {
                    scrabbleGame.scrabbleRack.RemoveTile(scrabbleTilesSelectedFromRack.scrabbleTilesSelectedFromRack[i].Id);
                }


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

        private void mbox_ok(object sender, RoutedEventArgs e)
        {
            if (DefineBlankTextBox.Text.Length > 0)
            {
                if(DefineBlankTextBox.Text.Length > 1)
                {
                    MessageBox.Show("Proszę podać jedną wartość!");
                    return;
                }

                if(Char.IsLetter(DefineBlankTextBox.Text[0]))
                {
                    if (isExistBlankCounter == 0)
                        defineBlankValue = DefineBlankTextBox.Text.ToUpper();
                    else if (isExistBlankCounter == 1)
                        defineBlankTwoValue = DefineBlankTextBox.Text.ToUpper();
                    DefineBlankTextBox.Text = "";
                }
                else
                {
                    MessageBox.Show("Podana wartość nie jest literą. Spróbuj wpisać ponownie!");
                    DefineBlankTextBox.Text = "";
                    return;
                }

                if (countBlank == 2 && isExistBlankTwoCounter==0 && isExistBlankCounter == 1)
                {
                    isExistBlankTwoCounter++;
                }
                
                if(isExistBlankTwoCounter==0) // wykona się w pierwszym przypadku
                    isExistBlankCounter++;

                Play_Click(sender, e);
            }


        }

        private void mbox_cancel(object sender, RoutedEventArgs e)
        {
            isExistBlankCounter = 0;
            isExistBlankTwoCounter = 0;
            DefineBlankTextBox.Text = "";
            defineBlankValue = "";
            defineBlankTwoValue = "";

            DialogReplacement.Visibility = System.Windows.Visibility.Hidden;
            scrabbleBoard.Visibility = System.Windows.Visibility.Visible;
        }
    } 
}
