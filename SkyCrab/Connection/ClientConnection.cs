using System;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace SkyCrab.Connection
{
    class ClientConnection : DataConnection
    {
        
        private const String publicKeyFilePath = "./public_key.txt";
        private static readonly RSACryptoServiceProvider rsa_csp = GetCSPFromFile(publicKeyFilePath);


        public static void Inicjalize()
        {
            //Do nothing. Just let 'rsa_csp' to initialize.
            if (rsa_csp == null)
                throw new Exception();
        }

        public ClientConnection(string host, int readTimeout) :
            base(new TcpClient(host, PORT), readTimeout)
        {
        }

        protected override void Initialize()
        {
            GenerateSessionKey();
            SendSessionKeyToServer();
            ReceiveIVFromServer();
        }

        private void GenerateSessionKey()
        {
            inputRijndael = CreateRijndaelManaged();
            inputRijndael.GenerateKey();
            inputRijndael.GenerateIV();
        }

        private void SendSessionKeyToServer()
        {
            byte[] plainKey = new byte[inputRijndael.Key.Length + inputRijndael.IV.Length];
            inputRijndael.Key.CopyTo(plainKey, 0);
            inputRijndael.IV.CopyTo(plainKey, inputRijndael.Key.Length);
            byte[] encryptedKey = rsa_csp.Encrypt(plainKey, false);
            WriteUnencryptedBytes(encryptedKey);
        }

        private void ReceiveIVFromServer()
        {
            outputRijndael = CreateRijndaelManaged();
            byte[] key = new byte[inputRijndael.Key.Length];
            inputRijndael.Key.CopyTo(key, 0);
            outputRijndael.Key = key;
            byte[] iv = ReadBytes((UInt16)outputRijndael.IV.Length);
            outputRijndael.IV = iv;
        }

        public static void Deinicjalize()
        {
            if (rsa_csp != null)
                rsa_csp.Dispose();
        }

    }
}
