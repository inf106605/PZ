using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace SkyCrab.Connection
{
    class ServerConnection : DataConnection
    {

        private const String keysFilePath = "./keys_top_secret.txt";
        private const String publicKeyFilePath = "./public_key.txt";
        private static readonly RSACryptoServiceProvider rsa_csp = GetCSP();


        public static void Inicjalize()
        {
            //Do nothing. Just let 'rsa_csp' to initialize.
            if (rsa_csp == null)
                throw new Exception();
        }

        public ServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

        protected override void Initialize()
        {
            ReceiveSessionKeyFromServer();
            GenerateIV();
            SendIVToClient();
        }

        private void ReceiveSessionKeyFromServer()
        {
            outputRijndael = CreateRijndaelManaged();
            byte[] encryptedKey = ReadUnencryptedBytes(rsaKeyBytes);
            byte[] plainKey = rsa_csp.Decrypt(encryptedKey, false);
            byte[] key = new byte[outputRijndael.Key.Length];
            Array.Copy(plainKey, 0, key, 0, outputRijndael.Key.Length);
            outputRijndael.Key = key;
            byte[] iv = new byte[outputRijndael.IV.Length];
            Array.Copy(plainKey, outputRijndael.Key.Length, iv, 0, outputRijndael.IV.Length);
            outputRijndael.IV = iv;
        }

        private void GenerateIV()
        {
            inputRijndael = CreateRijndaelManaged();
            byte[] key = new byte[outputRijndael.Key.Length];
            outputRijndael.Key.CopyTo(key, 0);
            inputRijndael.Key = key;
        }

        private void SendIVToClient()
        {
            inputRijndael.GenerateIV();
            WriteBytes(inputRijndael.IV);
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
            var csp = new RSACryptoServiceProvider(rsaKeyBytes * 8);
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
