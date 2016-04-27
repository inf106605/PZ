using SkyCrab.Connection.PresentationLayer.Messages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SkyCrab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static ClientConnection clientConn;

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                DisconnectMsg.AsyncPostDisconnect(clientConn);
                clientConn.Dispose();
                ClientConnection.DisposeStaticMembers();
            }
            catch (Exception)
            {
                //MessageBox.Show("Wystąpił błąd o podanej treści: " + ex.Message);
            }
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
    
            try {
                ClientConnection.PreLoadStaticMembers();
                clientConn = new ClientConnection("127.0.0.1", 100);
            } catch(Exception ex)
            {
                MessageBox.Show("Nie udało się połączyć: " + ex.Message);
                try
                {
                    clientConn.Dispose();
                }
                catch
                {
                }
            }

        }

    }
}
