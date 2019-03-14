using System;
using System.Collections.Generic;
using System.Text;

namespace Bb.Reminder.Exceptions
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class InvalidMethodException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMethodException"/> class.
        /// </summary>
        public InvalidMethodException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMethodException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidMethodException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMethodException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InvalidMethodException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMethodException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected InvalidMethodException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
