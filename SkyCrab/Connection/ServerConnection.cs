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
            using (StreamWriter streamWriter = new StreamWriter(new FileStream(keysFilePath, FileMode.Create)))
            {
                string xml = csp.ToXmlString(true);
                streamWriter.Write(xml);
                streamWriter.Close();
            }
            using (StreamWriter streamWriter = new StreamWriter(new FileStream(publicKeyFilePath, FileMode.Create)))
            {
                string xml = csp.ToXmlString(false);
                streamWriter.Write(xml);
                streamWriter.Close();
            }
            return csp;
        }

        public static void Deinicjalize()
        {
            if (rsa_csp != null)
                rsa_csp.Dispose();
        }

    }
}
