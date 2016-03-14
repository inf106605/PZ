using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab.connection
{
    abstract class Connection
    {
        public const int PORT = 8888;

        private TcpClient tcpClient;
        

        protected Connection(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        public void Write(string text)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        public string Read()
        {
            byte[] bytes = new byte[100];
            tcpClient.GetStream().Read(bytes, 0, 100);
            string text = Encoding.UTF8.GetString(bytes);
            return text;
        }

        public void Close()
        {
            tcpClient.Close();
            tcpClient = null;
        }

    }
}
