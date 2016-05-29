using SkyCrab.Classes.Menu.Guest;
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

namespace SkyCrab.Classes.Menu.LoggedPlayer
{
    /// <summary>
    /// Interaction logic for AboutProgramLoggedPlayer.xaml
    /// </summary>
    public partial class AboutProgramLoggedPlayer : UserControl
    {
        public AboutProgramLoggedPlayer()
        {
            InitializeComponent();
            AboutProgramContent.AppendText(AboutProgramClass.PrintText());
        }

        private void ReturnMainMenuLoggedPlayer_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenuLoggedPlayer());
        }
    }
}
