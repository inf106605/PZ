using SkyCrab.Connection.PresentationLayer.Messages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd o podanej treści: " + ex.Message);
            }
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
    
            try {

                String path = @"connectionConfig.txt";

                if (!File.Exists(path))
                {
                    // Create the file.
                    using (FileStream fs = File.Create(path))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes("127.0.0.1");
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }

                using (StreamReader sr = new StreamReader("connectionConfig.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String host = sr.ReadLine();
                    ClientConnection.PreLoadStaticMembers();

                    clientConn = new ClientConnection(host, ClientConnection.PORT, 1000);
                    clientConn.AddDisposedListener((connection, errors) =>
                            {
                                if (errors)
                                    MessageBox.Show("Wystąpił błąd o podanej treści: " + new AggregateException(clientConn.Exceptions));
                                clientConn = null; //TODO handle exception (from argument, it is not throwed)
                            });
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
