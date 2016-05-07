using System;

namespace SkyCrab.Connection.Utils
{
    sealed class Sequence
    {

        private readonly object _sequenceLock = new object();
        private volatile UInt16 currentValue = 0;

        public UInt16 Value
        {
            get
            {
                lock (_sequenceLock)
                    return ++currentValue;
            }
        }

    }
}
