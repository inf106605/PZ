using System.Windows;
using System.Windows.Controls;
using SkyCrab.Classes.Menu;

namespace SkyCrab.Menu
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        // Akcja do przycisku - Graj jako gość

        private void PlayAsGuest_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new PlayAsGuest());
        }

        // Akcja do przycisku - Zaloguj

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Login());
        }

        // Akcja do przycisku - Zarejestruj

        private void Registration_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Registration());
        }

        // Akcja do przycisku - Pomoc

        private void Help_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Help());
        }

        // Akcja do przycisku - O programie

        private void AboutProgram_Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new About());
        }

        // Akcja do przycisku - Zakończ

        private void Shuttdown_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
