﻿using System;
using System.Windows;
using System.Windows.Controls;
using SkyCrab.Menu;

namespace SkyCrab
{
    /// <summary>
    /// Interaction logic for PageSwitcher.xaml
    /// </summary>

    public interface ISwitchable
    {
        void UtilizeState(object state);
    }
    
    public partial class PageSwitcher : Window
    {
        public PageSwitcher()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new MainMenu());
        }

        public void Navigate(UserControl nextPage)
        {
            this.Content = nextPage;
        }

        public void Navigate(UserControl nextPage, object state)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;
            if (s != null)
            {
                s.UtilizeState(state);
            }
            else
                throw new ArgumentException("NextPage is not ISwitchable!" + nextPage.Name.ToString());
        }

        private void button_test_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Test application");
        }
    }
}
