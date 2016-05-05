using System;

namespace SkyCrabServer.ServerClasses
{
    sealed class Sequence
    {

        private readonly object _sequenceLock = new object();
        private volatile UInt32 currentValue = 0;

        public UInt32 Value
        {
            get
            {
                lock (_sequenceLock)
                    return ++currentValue;
            }
        }

    }
}
