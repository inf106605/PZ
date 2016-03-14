using SkyCrab.connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Connection connection = new ClientConnection("localhost");
                connection.Write("Message from client.");
                string text = connection.Read();
                Console.WriteLine(text);
                connection.Close();
                Console.WriteLine("OK");
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
