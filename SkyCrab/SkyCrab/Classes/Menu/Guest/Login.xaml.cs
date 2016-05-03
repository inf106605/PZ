using System.Windows;
using System.Windows.Controls;
using SkyCrab.Menu;
using SkyCrab.Classes.Menu.Guest;
using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts;

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
            if(loginTextbox.Text.Length < 3)
            {
                MessageBox.Show("Podana nazwa użytkownika jest za krótka!");
                return;
            }
            if(passTextbox.Password.Length < 5)
            {
                MessageBox.Show("Twoje hasło jest za krótkie!");
                return;
            }

            PlayerProfile playerProfile = new PlayerProfile();

            playerProfile.Login = loginTextbox.Text;
            playerProfile.Password = passTextbox.Password;

            var answer = LoginMsg.SyncPostLogin(App.clientConn, playerProfile, 1000);

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
                    Switcher.Switch(new MainMenuLoggedPlayer());
                    return;
                }
        }
        

        private void ButtonForgottenPassword_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ForgottenPassword());
        }
    }
}
