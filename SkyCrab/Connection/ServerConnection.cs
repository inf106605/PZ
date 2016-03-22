using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SkyCrab.connection
{
    class ServerConnection : DataConnection
    {

        private const String keysFilePath = "./keys_top_secret.txt";
        private const String publicKeyFilePath = "./public_key.txt";
        private static readonly RSACryptoServiceProvider rsa_csp = GetCSP();



        public static void Inicjalize()
        {
            //Do nothing. Just let 'rsa_csp' to initialize.
        }

        public ServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

        protected override void Initialize()
        {
            //TODO
        }

        private static RSACryptoServiceProvider GetCSP()
        {
            try
            {
                return GetCSPFromFile(keysFilePath);
            } catch (Exception)
            {
                return GenerateCSP();
            }
        }

        private static RSACryptoServiceProvider GenerateCSP()
        {
            var csp = new RSACryptoServiceProvider(2048);
            String xml = csp.ToXmlString(true);
            FileStream fileStream = new FileStream(keysFilePath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(xml);
            streamWriter.Close();
            xml = csp.ToXmlString(false);
            fileStream = new FileStream(publicKeyFilePath, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(xml);
            streamWriter.Close();
            return csp;
        }

    }
}
