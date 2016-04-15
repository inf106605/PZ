using System.Windows;
using System.Windows.Controls;
using SkyCrab.Menu;

namespace SkyCrab.Classes.Menu
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : UserControl
    {
        public Help()
        {
            InitializeComponent();
        }

        private void ReturnMenu_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }
    }
}
