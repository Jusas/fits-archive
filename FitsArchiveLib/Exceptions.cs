using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib
{
    public class FitsFileException : Exception
    {
        public FitsFileException()
        {
        }

        public FitsFileException(string message) : base(message)
        {
        }

        public FitsFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FitsFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class FitsDatabaseException : Exception
    {
        public FitsDatabaseException()
        {
        }

        public FitsDatabaseException(string message) : base(message)
        {
        }

        public FitsDatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FitsDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
