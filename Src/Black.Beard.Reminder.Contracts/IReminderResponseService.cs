using System;
using System.Collections.Generic;

namespace Bb.Reminder
{

    public interface IReminderResponseService : IDisposable
    {

        string MethodName { get; }

        /// <summary>
        /// Pushes the specified item.
        /// </summary>
        /// <param name="uuid">The UUID. is unique identified given by caller. (consider unicity is call concern)</param>
        /// <param name="address">The address.</param>
        /// <param name="message">The message.</param>
        /// <param name="headers">The headers.'</param>
        void Push(Guid uuid, string address, string message, Dictionary<string, object> headers);

    }

}