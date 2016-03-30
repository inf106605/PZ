using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace SkyCrab.connection
{
    internal abstract class EncryptedConnection : BasicConnection
    {

        protected const int rsaKeyBytes = 256;
        protected const int maxRsaCryptogram = rsaKeyBytes - 11;

        protected RijndaelManaged inputRijndael;
        protected RijndaelManaged outputRijndael;


        protected EncryptedConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
            Initialize();
        }

        protected abstract void Initialize();

        protected static RijndaelManaged CreateRijndaelManaged()
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Padding = PaddingMode.None;
            return rijndaelManaged;
        }

        protected override void WriteBytes(byte[] bytes)
        {
            byte[] unencryptedBytes = new byte[CalcCriptogramSize(bytes.Length)];
            Array.Copy(bytes, unencryptedBytes, bytes.Length);
            var encryptor = outputRijndael.CreateEncryptor();
            byte[] encryptedBytes = Transform(unencryptedBytes, encryptor);
            base.WriteBytes(encryptedBytes);
        }

        protected override byte[] ReadBytes(UInt16 size)
        {
            UInt16 encryptedSize = (UInt16)((size + 15) & (~0x000F));
            byte[] encryptedBytes = base.ReadBytes(encryptedSize);
            var decryptor = inputRijndael.CreateDecryptor();
            byte[] decryptedBytes = Transform(encryptedBytes, decryptor);
            byte[] bytes = new byte[size];
            Array.Copy(decryptedBytes, bytes, size);
            return bytes;
        }

        private static int CalcCriptogramSize(int dataSize)
        {
            int criptogramSize = (dataSize + 15) & (~0x000F);
            return criptogramSize;
        }

        private static byte[] Transform(byte[] bytes, ICryptoTransform transform)
        {
            using (var stream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                {
                    foreach (byte x in bytes)
                        cryptoStream.WriteByte(x);
                }

                byte[] transformedBytes = stream.ToArray();
                return transformedBytes;
            }
        }

        protected void WriteUnencryptedBytes(byte[] bytes)
        {
            base.WriteBytes(bytes);
        }

        protected byte[] ReadUnencryptedBytes(UInt16 size)
        {
            return base.ReadBytes(size);
        }

        protected static RSACryptoServiceProvider GetCSPFromFile(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(new FileStream(filePath, FileMode.Open)))
            {
                string xml = streamReader.ReadToEnd();
                var csp = new RSACryptoServiceProvider();
                csp.FromXmlString(xml);
                return csp;
            }
        }

        public override void Dispose()
        {
            if (inputRijndael != null)
                inputRijndael.Dispose();
            inputRijndael = null;
            if (outputRijndael != null)
                outputRijndael.Dispose();
            outputRijndael = null;
        }

    }
}
