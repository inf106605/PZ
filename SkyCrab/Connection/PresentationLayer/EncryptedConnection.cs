//#define DO_NOT_ENCRYPT
#if DO_NOT_ENCRYPT
#warning "Connection is not encrypted!"
#endif

using SkyCrab.Connection.SessionLayer;
using SkyCrab.Connection.Utils;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace SkyCrab.Connection.PresentationLayer
{
    public sealed class WrongAsymetricKeyException : SkyCrabConnectionException
    {
        public WrongAsymetricKeyException(Exception innerException) :
            base("", innerException)
        {
        }
    }

    public abstract class EncryptedConnection : QueuedConnection
    {
        
        protected const int rsaKeyBytes = 256;
        protected const int maxRsaCryptogram = rsaKeyBytes - 11;

        protected RijndaelManaged inputRijndael;
        protected RijndaelManaged outputRijndael;


        protected EncryptedConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
            #if !DO_NOT_ENCRYPT
            try
            {
                Initialize();
            }
            catch (CryptographicException e)
            {
                throw new WrongAsymetricKeyException(e);
            }
            #endif
        }

        protected abstract void Initialize();

        protected static RijndaelManaged CreateRijndaelManaged()
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Padding = PaddingMode.None;
            return rijndaelManaged;
        }

        internal override byte[] SyncReadBytes(UInt16 size)
        {
            #if DO_NOT_ENCRYPT
            byte[] bytes = base.SyncReadBytes(size);
            return bytes;
            #else
            UInt16 criptogramSize = CalculateCriptogramSize(size);
            byte[] encryptedBytes = base.SyncReadBytes(criptogramSize);
            byte[] decryptedBytes = DecryptBytes(encryptedBytes, size);
            return decryptedBytes;
            #endif
        }

        internal override void AsyncWriteBytes(object writingBlock, byte[] bytes, Callback callback = null, object state = null)
        {
            #if DO_NOT_ENCRYPT
            base.AsyncWriteBytes(writingBlock, bytes, callback, state);
            #else
            byte[] encryptedBytes = EncryptBytes(bytes);
            base.AsyncWriteBytes(writingBlock, encryptedBytes, callback, state);
            #endif
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
            if (unencryptedBytes == null)
                return null;
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

        protected override void StopCreatingMessages()
        {
            base.StopCreatingMessages();
        }

        protected override void DoDispose()
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
