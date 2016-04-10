using System;
using System.Net.Sockets;
using System.Text;

namespace SkyCrab.Connection.PresentationLayer
{
    //TODO make methods not public
    //TODO export classes to separate files?
    internal abstract class DataConnection : EncryptedConnection
    {

        public abstract class Writer
        {
            protected object writingBlock;

            protected Writer(object writingBlock)
            {
                this.writingBlock = writingBlock;
            }

            public abstract void Write(DataConnection dataConnection, byte[] bytes);
        }

        private sealed class SynchronicWriter : Writer
        {
            public SynchronicWriter(object writingBlock) :
                base(writingBlock)
            {
            }

            public override void Write(DataConnection dataConnection, byte[] bytes)
            {
                dataConnection.SyncWriteBytes(writingBlock, bytes);
            }
        }

        private sealed class AsynchronicWriter : Writer
        {
            private Callback callback;
            private object state;

            public AsynchronicWriter(object writingBlock, Callback callback, object state) :
                base(writingBlock)
            {
                this.callback = callback;
                this.state = state;
            }

            public override void Write(DataConnection dataConnection, byte[] bytes)
            {
                dataConnection.AsyncWriteBytes(writingBlock, bytes, callback, state);
            }
        }

        public interface Transcoder<T>
        {
            T Read(DataConnection dataConnection);
            void Write(DataConnection dataConnection, Writer writer, T data);
        }

        private sealed class Int8Transcoder : Transcoder<sbyte>
        {
            public sbyte Read(DataConnection dataConnection)
            {
                byte[] bytes = dataConnection.SyncReadBytes(1);
                sbyte result = (sbyte)bytes[0];
                return result;
            }

            public void Write(DataConnection dataConnection, Writer writer, sbyte data)
            {
                byte[] bytes = new byte[1] { (byte)data };
                writer.Write(dataConnection, bytes);
            }
        }

        private sealed class UInt8Transcoder : Transcoder<byte>
        {
            public byte Read(DataConnection dataConnection)
            {
                byte[] bytes = dataConnection.SyncReadBytes(1);
                byte data = bytes[0];
                return data;
            }

            public void Write(DataConnection dataConnection, Writer writer, byte data)
            {
                byte[] bytes = new byte[1] { data };
                writer.Write(dataConnection, bytes);
            }
        }

        private sealed class UInt16Transcoder : Transcoder<UInt16>
        {
            public UInt16 Read(DataConnection dataConnection)
            {
                byte[] bytes = dataConnection.SyncReadBytes(2);
                UInt16 data = (UInt16)(((UInt16)bytes[0]) << 8 |
                                ((UInt16)bytes[1]) << 0);
                return data;
            }

            public void Write(DataConnection dataConnection, Writer writer, UInt16 data)
            {
                byte[] bytes = new byte[2];
                bytes[0] = (byte)(data >> 8);
                bytes[1] = (byte)(data >> 0);
                writer.Write(dataConnection, bytes);
            }
        }

        private sealed class UInt32Transcoder : Transcoder<UInt32>
        {
            public UInt32 Read(DataConnection dataConnection)
            {
                byte[] bytes = dataConnection.SyncReadBytes(4);
                UInt32 data = ((UInt32)bytes[0]) << 24 |
                                ((UInt32)bytes[1]) << 16 |
                                ((UInt32)bytes[2]) << 8 |
                                ((UInt32)bytes[3]) << 0;
                return data;
            }

            public void Write(DataConnection dataConnection, Writer writer, UInt32 data)
            {
                byte[] bytes = new byte[4];
                bytes[0] = (byte)(data >> 24);
                bytes[1] = (byte)(data >> 16);
                bytes[2] = (byte)(data >> 8);
                bytes[3] = (byte)(data >> 0);
                writer.Write(dataConnection, bytes);
            }
        }

        private sealed class StringTranscoder : Transcoder<String>
        {
            private readonly UInt16Transcoder uint16Transcoder;

            public StringTranscoder(UInt16Transcoder uint16Transcoder)
            {
                this.uint16Transcoder = uint16Transcoder;
            }

            public String Read(DataConnection dataConnection)
            {
                UInt16 length = uint16Transcoder.Read(dataConnection);
                byte[] bytes = dataConnection.SyncReadBytes(length);
                string data = Encoding.UTF8.GetString(bytes);
                return data;
            }

            public void Write(DataConnection dataConnection, Writer writer, String data)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                UInt16 lenght = (UInt16)bytes.Length;
                uint16Transcoder.Write(dataConnection, writer, lenght);
                writer.Write(dataConnection, bytes);
            }
        }


        public static readonly Transcoder<sbyte> int8Transcoder = new Int8Transcoder();
        public static readonly Transcoder<byte> uint8Transcoder = new UInt8Transcoder();
        public static readonly Transcoder<UInt16> uint16Transcoder = new UInt16Transcoder();
        public static readonly Transcoder<UInt32> uint32Transcoder = new UInt32Transcoder();
        public static readonly Transcoder<String> stringTranscoder = new StringTranscoder((UInt16Transcoder) uint16Transcoder);


        protected DataConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

        public T SyncReadData<T>(Transcoder<T> transcoder)
        {
            T data = transcoder.Read(this);
            return data;
        }

        public void SyncWriteData<T>(Transcoder<T> transcoder, object writingBlock, T data)
        {
            SynchronicWriter writer = new SynchronicWriter(writingBlock);
            transcoder.Write(this, writer, data);
        }

        public void AsyncWriteData<T>(Transcoder<T> transcoder, object writingBlock, T data, Callback callback = null, object state = null)
        {
            AsynchronicWriter writer = new AsynchronicWriter(writingBlock, callback, state);
            transcoder.Write(this, writer, data);
        }

    }
}
