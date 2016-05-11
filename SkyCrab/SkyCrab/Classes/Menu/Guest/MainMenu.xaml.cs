using System.Windows;
using System.Windows.Controls;
using SkyCrab.Classes.Menu;
using SkyCrab.Classes.Menu.Guest;
using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts;
using SkyCrab.Connection.PresentationLayer.Messages;

namespace SkyCrab.Menu
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        // Akcja do przycisku - Graj jako gość

        private void PlayAsGuest_Button_Click(object sender, RoutedEventArgs e)
        {
            var answer = LoginAsGuestMsg.SyncPost(App.clientConn, 2000);

            if (!answer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = answer.Value;

            if (answerValue.messageId == MessageId.ERROR)
            {
                ErrorCode errorCode = (ErrorCode)answerValue.message;

                switch (errorCode)
                {
                    case ErrorCode.SESSION_ALREADY_LOGGED:
                        {
                            MessageBox.Show("Twój program jest już zalogowany na innego użytkownika");
                            break;
                        }
                }

                return;
            }

            if (answerValue.messageId == MessageId.LOGIN_OK)
            {
                SkyCrabGlobalVariables.player = (Player)answerValue.message;
                Switcher.Switch(new PlayAsGuest());
            }
        }

        // Akcja do przycisku - Zaloguj

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Login());
        }

        // Akcja do przycisku - Zarejestruj

        private void Registration_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Registration());
        }

        // Akcja do przycisku - Pomoc

        private void Help_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Help());
        }

        // Akcja do przycisku - O programie

        private void AboutProgram_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new About());
        }

        // Akcja do przycisku - Zakończ

        private void Shuttdown_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Rank_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new RankGuest());
        }
    }
}
