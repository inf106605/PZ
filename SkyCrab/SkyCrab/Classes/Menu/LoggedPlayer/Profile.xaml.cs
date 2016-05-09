using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : UserControl
    {
        public Profile()
        {
            InitializeComponent();
            SetDataToControls();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // walidacja hasła

            if (passTextbox.Password != "" && passTextbox.Password.Length < 5)
            {
                MessageBox.Show("Podane hasło jest za krótkie (Min. 5 znaków) !");
                return;
            }

            if (passTextbox.Password != "" && passTextbox.Password.Length > 20)
            {
                MessageBox.Show("Podane hasło jest za długie (Max. 20 znaków) !");
                return;
            }

            if(passTextbox.Password !=  passConTextbox.Password)
            {
                MessageBox.Show("Podane hasła różnią się od siebie!");
                return;
            }

            // walidacja e-mail'a

            bool isEmail = Regex.IsMatch(emailTextbox.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (!isEmail)
            {
                MessageBox.Show("Podany format e-mail'a jest nieprawidłowy!");
                return;
            }

            if (emailTextbox.Text != emailConTextbox.Text)
            {
                MessageBox.Show("Podane adresy e-mail się różnią!");
                return;
            }

            // walidacja nicku

            if(nickTextbox.Text.Length > 50)
            {
                MessageBox.Show("Podany nick jest za długi ( Max. 50 znaków) !");
                return;
            }

            PlayerProfile playerProfile = new PlayerProfile();

            playerProfile.EMail = emailTextbox.Text == "" ? null : emailTextbox.Text;
            playerProfile.Nick = nickTextbox.Text == "" ? null : nickTextbox.Text;
            playerProfile.Password = passTextbox.Password == "" ? null : passTextbox.Password;

            var answer = EditProfileMsg.SyncPost(App.clientConn, playerProfile, 1000);

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
                    case ErrorCode.NICK_IS_TOO_SHITTY:
                        {
                            MessageBox.Show("Podany nick jest nieodpowiedni!");
                            break;
                        }

                    case ErrorCode.EMAIL_OCCUPIED2:
                        {
                            MessageBox.Show("Nie można utworzyć konta o podanym adresie e-mail!");
                            break;
                        }
                }

                return;
            }

            if(answerValue.messageId == MessageId.OK)
            {
                PlayerProfile playerProfile1 = SkyCrabGlobalVariables.player.Profile;

                if(emailTextbox.Text != "")
                {
                    playerProfile1.EMail = emailTextbox.Text;
                }

                if(nickTextbox.Text != "")
                {
                    playerProfile1.Nick = nickTextbox.Text;
                }

                SkyCrabGlobalVariables.player.Profile = playerProfile1;

                SetDataToControls();

            }

        }

        private void SetDataToControls()
        {
            loginTextbox.Text = SkyCrabGlobalVariables.player.Profile.Login;
            passTextbox.Password = "";
            passConTextbox.Password = "";
            emailTextbox.Text = SkyCrabGlobalVariables.player.Profile.EMail;
            emailConTextbox.Text = SkyCrabGlobalVariables.player.Profile.EMail;
            nickTextbox.Text = SkyCrabGlobalVariables.player.Nick;
        }

        private void CancelRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenuLoggedPlayer());
        }
    }
}
