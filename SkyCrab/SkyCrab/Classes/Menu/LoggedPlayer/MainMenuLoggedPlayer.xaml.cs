using SkyCrab.Classes.Menu.LoggedPlayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu;
using SkyCrab.Menu;
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
    /// Interaction logic for MainMenuLoggedPlayer.xaml
    /// </summary>
    public partial class MainMenuLoggedPlayer : UserControl
    {
        public MainMenuLoggedPlayer()
        {
            InitializeComponent();
        }

        private void PlayAsLoggedPlayer_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new PlayAsLoggedPlayer());
        }

        private void Profil_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Profile());
        }

        private void History_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HistoryGames());
        }

        private void Help_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HelpLoggedPlayer());
        }

        private void AboutProgram_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AboutProgramLoggedPlayer());
        }

        private void Logout_Button_Click(object sender, RoutedEventArgs e)
        {
            var answer = LogoutMsg.SyncPostLogout(App.clientConn, 1000);

            if(!answer.HasValue)
            {
                MessageBox.Show("Brak odpowiedzi od serwera!");
                return;
            }

            var answerValue = answer.Value;

            if(answerValue.messageId == MessageId.ERROR)
            {
                MessageBox.Show("Użytkownik nie jest zalogowany!");
                return;
            }

            if (answerValue.messageId == MessageId.OK)
            {
                Switcher.Switch(new MainMenu());
                return;
            }
        }

        private void Shuttdown_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Friend_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Friends());
        }

        private void Rank_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new RankLoggedPlayer());
        }
    }
}
