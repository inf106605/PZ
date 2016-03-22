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

        protected EncryptedConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
            Initialize();
        }

        protected abstract void Initialize();

        protected override void WriteBytes(byte[] bytes)
        {
            //TODO
            base.WriteBytes(bytes);
        }

        protected override byte[] ReadBytes(UInt16 size)
        {
            //TODO
            return base.ReadBytes(size);
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
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            string xml = streamReader.ReadToEnd();
            var csp = new RSACryptoServiceProvider(2048);
            csp.FromXmlString(xml);
            return csp;
        }

    }
}
