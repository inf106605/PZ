using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class VersionTranscoder : ITranscoder<Version>
    {

        private static readonly Int32Transcoder int32Transcoder = new Int32Transcoder();


        public Version Read(DataConnection dataConnection)
        {
            Int32 major = int32Transcoder.Read(dataConnection);
            Int32 minor = int32Transcoder.Read(dataConnection);
            Int32 build = int32Transcoder.Read(dataConnection);
            if (build == -1)
            {
                Version data = new Version(major, minor);
                return data;
            }
            Int32 revision = int32Transcoder.Read(dataConnection);
            if (revision == -1)
            {
                Version data = new Version(major, minor, build);
                return data;
            }
            else
            {
                Version data = new Version(major, minor, build, revision);
                return data;
            }
        }

        public void Write(DataConnection dataConnection, object writingBlock, Version data)
        {
            int32Transcoder.Write(dataConnection, writingBlock, data.Major);
            int32Transcoder.Write(dataConnection, writingBlock, data.Minor);
            int32Transcoder.Write(dataConnection, writingBlock, data.Build);
            int32Transcoder.Write(dataConnection, writingBlock, data.Revision);
        }

    }
}
