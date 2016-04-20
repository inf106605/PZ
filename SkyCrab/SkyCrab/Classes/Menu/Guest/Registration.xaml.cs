using System.Windows;
using System.Windows.Controls;
using SkyCrab.Menu;
using SkyCrab.Connection.PresentationLayer.Messages.Menu;
using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages;

namespace SkyCrab.Classes.Menu
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : UserControl
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerProfile playerProfile = new PlayerProfile();

            // walidacja

            playerProfile.login = loginTextbox.Text;
            playerProfile.password = passTextbox.Password;
            playerProfile.eMail = emailTextbox.Text;

            var answer = RegisterMsg.SyncPostRegister(App.clientConn, playerProfile, 1000);

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
                    case ErrorCode.LOGIN_OCCUPIED:
                        {
                            MessageBox.Show("Podany login jest zajęty!");
                            break;
                        }
                    case ErrorCode.PASSWORD_TOO_SHORT:
                        {
                            MessageBox.Show("Podane hasło jest za krótkie!");
                            break;
                        }

                    case ErrorCode.EMAIL_OCCUPIED:
                        {
                            MessageBox.Show("Nie można utworzyć konta o podanym adresie e-mail!");
                            break;
                        }
                }

                return;
            }

            if (answerValue.messageId == MessageId.LOGIN_OK)
            {
                Player player = (Player)answerValue.message;
                Switcher.Switch(new MainMenuLoggedPlayer());
                return;
            }
        }

        private void CancelRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

       private void RulesLink_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Rules());
        }
    }
}
