using System.Windows;
using System.Windows.Controls;
using SkyCrab.Menu;
using SkyCrab.Classes.Menu.Guest;
using SkyCrab.Connection.PresentationLayer;
using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.Utils;
using SkyCrab.Connection.PresentationLayer.Messages;

namespace SkyCrab.Classes.Menu
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

        private void ButtonLoginReturn_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void ButonLoginConfirm_Click(object sender, RoutedEventArgs e)
        {
            // walidacja
            
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.login = loginTextbox.Text;
            playerProfile.password = passTextbox.Text;

            using (AnswerSynchronizer answerSynchronizer = new AnswerSynchronizer())
            {
                Connection.PresentationLayer.Messages.Menu.Login.AsyncPostLogin(App.clientConn, playerProfile, AnswerSynchronizer.Callback, answerSynchronizer);
                answerSynchronizer.Wait(1000);
                if (!answerSynchronizer.Answer.HasValue)
                {
                    MessageBox.Show("Brak odpowiedzi od serwera!");
                    return;
                }

                var answerValue = answerSynchronizer.Answer.Value;

                if (answerValue.messageId == MessageId.ERROR)
                {
                    ErrorCode errorCode = (ErrorCode)answerValue.message;

                    switch (errorCode)
                    {
                        case ErrorCode.USER_ALREADY_LOGGED:
                            {
                                MessageBox.Show("Użytkownik jest zalogowany");
                                break;
                            }
                        case ErrorCode.WRONG_LOGIN_OR_PASSWORD:
                            {
                                MessageBox.Show("Błąd logowania");
                                break;
                            }
                    }

                    return;
                }

                if (answerValue.messageId == MessageId.LOGIN_OK)
                {
                    Player player = (Player)answerValue.message;
                    Switcher.Switch(new MainMenuLoggedPlayer());
                }
            }
        }

        private void ButtonForgottenPassword_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ForgottenPassword());
        }
    }
}
