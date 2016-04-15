using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SkyCrab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ClientConnection clientConn;

        App()
        {
            ClientConnection.Inicjalize();
            clientConn = new ClientConnection("127.0.0.1",100);
        }

        ~App()
        {
            clientConn.Dispose();
            ClientConnection.Deinicjalize();
        }
    }
}
