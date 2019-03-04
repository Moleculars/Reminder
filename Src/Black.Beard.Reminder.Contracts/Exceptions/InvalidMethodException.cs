using System;
using System.Collections.Generic;
using System.Text;

namespace Bb.Reminder.Exceptions
{

    [Serializable]
    public class InvalidMethodException : Exception
    {
        public InvalidMethodException() { }
        public InvalidMethodException(string message) : base(message) { }
        public InvalidMethodException(string message, Exception inner) : base(message, inner) { }
        protected InvalidMethodException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
