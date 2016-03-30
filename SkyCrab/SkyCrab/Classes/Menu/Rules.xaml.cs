using System.Windows;
using System.Windows.Controls;
namespace SkyCrab.Classes.Menu
{
    /// <summary>
    /// Interaction logic for Rules.xaml
    /// </summary>
    public partial class Rules : UserControl
    {
        public Rules()
        {
            InitializeComponent();
        }

        private void ReturnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Registration());
        }
    }
}
