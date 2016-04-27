using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class VersionTranscoder : ITranscoder<Version>
    {

        private static readonly VersionTranscoder instance = new VersionTranscoder();
        public static VersionTranscoder Get
        {
            get { return instance; }
        }


        private VersionTranscoder()
        {
        }

        public Version Read(DataConnection dataConnection)
        {
            Int32 major = Int32Transcoder.Get.Read(dataConnection);
            Int32 minor = Int32Transcoder.Get.Read(dataConnection);
            Int32 build = Int32Transcoder.Get.Read(dataConnection);
            if (build == -1)
            {
                Version data = new Version(major, minor);
                return data;
            }
            Int32 revision = Int32Transcoder.Get.Read(dataConnection);
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
            Int32Transcoder.Get.Write(dataConnection, writingBlock, data.Major);
            Int32Transcoder.Get.Write(dataConnection, writingBlock, data.Minor);
            Int32Transcoder.Get.Write(dataConnection, writingBlock, data.Build);
            Int32Transcoder.Get.Write(dataConnection, writingBlock, data.Revision);
        }

    }
}
