using System.Windows;
using System.Windows.Controls;
using SkyCrab.Menu;

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
