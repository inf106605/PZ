using System.Windows;
using System.Windows.Controls;
using SkyCrab.Menu;
using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using System.Text.RegularExpressions;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts;

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

            // walidacja loginu

            if(loginTextbox.Text.Length < 3)
            {
                MessageBox.Show("Podany login jest zbyt krótki (Min 3 znaki) !");
                return;
            }

            if(loginTextbox.Text.Length > 20)
            {
                MessageBox.Show("Podany login jest zbyt długi (Max 20 znaków) !");
                return;
            }

            // walidacja hasła

            if(passTextbox.Password.Length < 5)
            {
                MessageBox.Show("Podane hasło jest za krótkie (Min 5 znaków) !");
                return;
            }

            if(passTextbox.Password.Length > 20)
            {
                MessageBox.Show("Podane hasło jest za długie (Max 20 znaków) !");
                return;
            }

            if (passTextbox.Password != passConTextbox.Password)
            {
                MessageBox.Show("Podane hasła się różnią!");
                return;
            }

            // walidacja e-mail'a

            bool isEmail = Regex.IsMatch(emailTextbox.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if(!isEmail)
            {
                MessageBox.Show("Podany format e-mail'a jest nieprawidłowy!");
                return;
            }

            if(emailTextbox.Text != emailConTextbox.Text)
            {
                MessageBox.Show("Podane adresy e-mail się różnią!");
                return;
            }

            if(!termsCheckBox.IsChecked.Value)
            {
                MessageBox.Show("Zatwierdź regulamin!");
                return;
            }

            playerProfile.Login = loginTextbox.Text;
            playerProfile.Password = passTextbox.Password;
            playerProfile.EMail = emailTextbox.Text;

            var answer = RegisterMsg.SyncPost(App.clientConn, playerProfile, 5000);

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
                    case ErrorCode.SESSION_ALREADY_LOGGED2:
                        {
                            MessageBox.Show("Twój program jest już zalogowany na innego użytkownika");
                            break;
                        }

                    case ErrorCode.LOGIN_OCCUPIED:
                        {
                            MessageBox.Show("Podany login jest zajęty!");
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
                SkyCrabGlobalVariables.player = (Player)answerValue.message;
                Switcher.Switch(new MainMenuLoggedPlayer());
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
