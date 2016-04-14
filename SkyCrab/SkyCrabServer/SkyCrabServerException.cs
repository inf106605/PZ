using SkyCrab.Common_classes;
using System;

namespace SkyCrabServer
{
    class SkyCrabServerException : SkyCrabException
    {

        public SkyCrabServerException() :
            base()
        {
        }

        public SkyCrabServerException(String message) :
            base(message)
        {
        }

        public SkyCrabServerException(String message, Exception innerException) :
            base(message, innerException)
        {
        }

    }
}
