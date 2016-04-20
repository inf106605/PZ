using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu;
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
            PlayerProfile playerProfile = new PlayerProfile();

            playerProfile.login = SkyCrabGlobalVariables.player.Profile.login;
            playerProfile.eMail = SkyCrabGlobalVariables.player.Profile.eMail;
            playerProfile.nick = SkyCrabGlobalVariables.player.Nick;

            var answer = EditProfileMsg.SyncPostEditProfile(App.clientConn, playerProfile, 1000);

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
                    case ErrorCode.PASSWORD_TOO_SHORT2:
                        {
                            MessageBox.Show("Podane hasło jest za krótkie!");
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
                    playerProfile1.eMail = emailTextbox.Text;
                }

                if(nickTextbox.Text != "")
                {
                    playerProfile1.nick = nickTextbox.Text;
                }

                SkyCrabGlobalVariables.player.Profile = playerProfile1;

                SetDataToControls();

            }

        }

        private void SetDataToControls()
        {
            loginTextbox.Text = SkyCrabGlobalVariables.player.Profile.login;
            passTextbox.Password = "";
            passConTextbox.Password = "";
            emailTextbox.Text = SkyCrabGlobalVariables.player.Profile.eMail;
            emailConTextbox.Text = SkyCrabGlobalVariables.player.Profile.eMail;
            nickTextbox.Text = SkyCrabGlobalVariables.player.Nick;
        }

        private void CancelRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenuLoggedPlayer());
        }
    }
}
