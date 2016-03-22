using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SkyCrab.connection
{
    class ClientConnection : DataConnection
    {
        
        private const String publicKeyFilePath = "./public_key.txt";
        private static readonly RSACryptoServiceProvider rsa_csp = GetCSPFromFile(publicKeyFilePath);



        public static void Inicjalize()
        {
            //Do nothing. Just let 'rsa_csp' to initialize.
        }

        public ClientConnection(string host, int readTimeout) :
            base(new TcpClient(host, PORT), readTimeout)
        {
        }

        protected override void Initialize()
        {
            //TODO
        }

    }
}
