using SkyCrab.Connection.PresentationLayer.Messages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
                using (StreamReader sr = new StreamReader("connectionConfig.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String host = sr.ReadLine();
                    ClientConnection.PreLoadStaticMembers();
                    clientConn = new ClientConnection(host,1000);
                    clientConn.AddConnectionCloseListener((connection, exceptions) => clientConn = null); //TODO handle exception (from argument, it is not throwed)
                }
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
