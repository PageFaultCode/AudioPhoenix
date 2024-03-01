using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioData.Exceptions
{
    public class AudioDataException : Exception
    {
        public AudioDataException() { }
        public AudioDataException(string message) : base(message) { }

        public AudioDataException(string message, Exception innerException) : base(message, innerException) { }
    }
}
