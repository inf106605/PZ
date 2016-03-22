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
using SkyCrab.Classess;

namespace SkyCrab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_test_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Test application");
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            // ... Create a new BitmapImage.
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri("crab_logo.png", UriKind.Relative);
            b.CacheOption = BitmapCacheOption.OnLoad;
            b.EndInit();

            // ... Get Image reference from sender.
            var image = sender as Image;
            // ... Assign Source.
            image.Source = b;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
