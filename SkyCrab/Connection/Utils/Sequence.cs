using System;

namespace SkyCrab.Connection.Utils
{
    internal interface ISequence
    {
        UInt16 Value { get; }
    }

    internal sealed class Sequence : ISequence
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

    internal sealed class FirstHalfSequence : ISequence
    {

        private readonly Sequence sequence = new Sequence();

        public UInt16 Value
        {
            get
            {
                return (UInt16)(sequence.Value & 0x7FFF);
            }
        }

    }

    internal sealed class SecondHalfSequence : ISequence
    {

        private readonly Sequence sequence = new Sequence();

        public UInt16 Value
        {
            get
            {
                return (UInt16)(-(Int16)(sequence.Value & 0x7FFF));
            }
        }

    }

}
