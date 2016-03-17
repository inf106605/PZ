using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrab.connection
{
    abstract class DataConnection : BasicConnection
    {

        protected DataConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

        public void WriteUInt16(UInt16 number)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(number >> 8);
            bytes[1] = (byte)(number >> 0);
            WriteBytes(bytes);
        }

        public UInt16 ReadUInt16()
        {
            byte[] bytes = ReadBytes(2);
            UInt16 number = (UInt16)(((UInt16)bytes[0]) << 8 |
                            ((UInt16)bytes[1]) << 0);
            return number;
        }

        public void WriteUInt32(UInt32 number)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(number >> 24);
            bytes[1] = (byte)(number >> 16);
            bytes[2] = (byte)(number >> 8);
            bytes[3] = (byte)(number >> 0);
            WriteBytes(bytes);
        }

        public UInt32 ReadUInt32()
        {
            byte[] bytes = ReadBytes(4);
            UInt32 number = ((UInt32)bytes[0]) << 24 |
                            ((UInt32)bytes[1]) << 16 |
                            ((UInt32)bytes[2]) << 8 |
                            ((UInt32)bytes[3]) << 0;
            return number;
        }

    }
}
