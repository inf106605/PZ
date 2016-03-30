using SkyCrab.Common_classes;
using System;

namespace SkyCrab.Connection
{
    public class SkyCrabConnectionException : SkyCrabException
    {

        public SkyCrabConnectionException() :
            base()
        {
        }
        
        public SkyCrabConnectionException(String message) :
            base(message)
        {
        }
        
        public SkyCrabConnectionException(String message, Exception innerException) :
            base(message, innerException)
        {
        }

    }
}
