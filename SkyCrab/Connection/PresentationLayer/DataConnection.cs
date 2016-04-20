using SkyCrab.Common_classes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using SkyCrab.Connection.PresentationLayer.Messages;
using System;
using System.Net.Sockets;

namespace SkyCrab.Connection.PresentationLayer
{
    public abstract class DataConnection : EncryptedConnection
    {

        internal static readonly ITranscoder<sbyte> int8Transcoder = new Int8Transcoder();
        internal static readonly ITranscoder<byte> uint8Transcoder = new UInt8Transcoder();
        internal static readonly ITranscoder<UInt16> uint16Transcoder = new UInt16Transcoder();
        internal static readonly ITranscoder<Int32> int32Transcoder = new Int32Transcoder();
        internal static readonly ITranscoder<UInt32> uint32Transcoder = new UInt32Transcoder();
        internal static readonly ITranscoder<Int64> int64Transcoder = new Int64Transcoder();
        internal static readonly ITranscoder<UInt64> uint64Transcoder = new UInt64Transcoder();
        internal static readonly ITranscoder<String> stringTranscoder = new StringTranscoder();
        internal static readonly ITranscoder<DateTime> dateTimeTranscoder = new DateTimeTranscoder();
        internal static readonly ITranscoder<Version> versionTranscoder = new VersionTranscoder();

        internal static readonly ITranscoder<MessageId> messageIdTranscoder = new MessageIdTranscoder();
        internal static readonly ITranscoder<ErrorCode> errorCodeTranscoder = new ErrorCodeTranscoder();
        internal static readonly ITranscoder<String> loginTranscoder = new LimitedStringTranscoder(LengthLimit.Login);
        internal static readonly ITranscoder<String> passwordTranscoder = new LimitedStringTranscoder(LengthLimit.Password);
        internal static readonly ITranscoder<String> eMailTranscoder = new LimitedStringTranscoder(LengthLimit.EMail);
        internal static readonly ITranscoder<String> nickTranscoder = new LimitedStringTranscoder(LengthLimit.Nick);


        protected DataConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

        internal T SyncReadData<T>(ITranscoder<T> transcoder)
        {
            T data = transcoder.Read(this);
            return data;
        }

        internal void AsyncWriteData<T>(ITranscoder<T> transcoder, object writingBlock, T data)
        {
            transcoder.Write(this, writingBlock, data);
        }

    }
}
