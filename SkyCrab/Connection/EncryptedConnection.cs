using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            #if DEBUG
            WriteBytesToConsole("<", bytes);
            #endif
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
            #if DEBUG
            WriteBytesToConsole(">", bytes);
            #endif
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

        #if DEBUG
        protected void WriteBytesToConsole(String prefix, byte[] bytes)
        {
            Console.Write(prefix + " [");
            bool first = true;
            foreach (byte x in bytes)
            {
                if (first)
                {
                    first = false;
                    Console.Write(x);
                }
                else
                    Console.Write(", " + x);
            }
            Console.WriteLine("]");
        }
        #endif

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
