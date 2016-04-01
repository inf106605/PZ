using System.Windows;
using System.Windows.Controls;
using SkyCrab.Menu;
using SkyCrab.Classes.Menu.Guest;

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
            Switcher.Switch(new MainMenuLoggedPlayer());
        }

        private void ButtonForgottenPassword_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ForgottenPassword());
        }
    }
}
