using SkyCrab.Connection.SessionLayer;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace SkyCrab.Connection.PresentationLayer
{
    internal abstract class EncryptedConnection : QueuedConnection
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

        protected override byte[] SyncReadBytes(UInt16 size)
        {
            UInt16 criptogramSize = CalculateCriptogramSize(size);
            byte[] encryptedBytes = base.SyncReadBytes(criptogramSize);
            byte[] decryptedBytes = DecryptBytes(encryptedBytes, size);
            return decryptedBytes;
        }

        protected override void SyncWriteBytes(object writingBlock, byte[] bytes)
        {
            byte[] encryptedBytes = EncryptBytes(bytes);
            base.SyncWriteBytes(writingBlock, encryptedBytes);
        }

        protected override void AsyncWriteBytes(object writingBlock, byte[] bytes, Callback callback = null, object state = null)
        {
            byte[] encryptedBytes = EncryptBytes(bytes);
            base.AsyncWriteBytes(writingBlock, encryptedBytes, callback, state);
        }

        protected byte[] ReadUnencryptedBytes(UInt16 size)
        {
            return base.SyncReadBytes(size);
        }

        protected void WriteUnencryptedBytes(object writingBlock, byte[] bytes)
        {
            base.AsyncWriteBytes(writingBlock, bytes);
        }

        private byte[] EncryptBytes(byte[] unencryptedBytes)
        {
            byte[] bytes = new byte[CalculateCriptogramSize((UInt16) unencryptedBytes.Length)];
            Array.Copy(unencryptedBytes, bytes, unencryptedBytes.Length);
            var encryptor = outputRijndael.CreateEncryptor();
            byte[] encryptedBytes = Transform(bytes, encryptor);
            return encryptedBytes;
        }

        private byte[] DecryptBytes(byte[] encryptedBytes, UInt16 size)
        {
            var decryptor = inputRijndael.CreateDecryptor();
            byte[] decryptedBytes = Transform(encryptedBytes, decryptor);
            byte[] bytes = new byte[size];
            Array.Copy(decryptedBytes, bytes, size);
            return bytes;
        }

        private UInt16 CalculateCriptogramSize(uint size)
        {
            UInt16 encryptedSize = (UInt16)((size + 15) & (~0x000F));
            return encryptedSize;
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
