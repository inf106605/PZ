using SkyCrab.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

namespace SkyCrab.Classes.Menu.Guest
{
    /// <summary>
    /// Interaction logic for ForgottenPassword.xaml
    /// </summary>
    public partial class ForgottenPassword : UserControl
    {
        public ForgottenPassword()
        {
            InitializeComponent();
        }

        private void ButonLoginConfirm_Click(object sender, RoutedEventArgs e)
        {
            // check if email is correctly

            bool isEmail = Regex.IsMatch(emailTextbox.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (isEmail)
            {
                MessageBox.Show("Prawidłowy email!");
            }
            else
            {
                MessageBox.Show("Nieprawidłowy email!");
            }
        }

        private void ButtonLoginReturn_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void emailTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
