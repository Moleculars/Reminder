using System;
using System.Collections.Generic;

namespace Bb.Reminder
{

    public interface IReminderResponseService : IDisposable
    {

        string MethodName { get; }

        /// <summary>
        /// Pushes the specified item. All arguments are injected by the message <see cref="WakeUpRequestModel" in Reminder service/>
        /// </summary>
        /// <param name="uuid">The UUID. is unique identifier given by caller. (consider unicity is caller concern)</param>
        /// <param name="address">The address of service to call. if the binding is broker, address is a publisher name, if the binding is http.* address is url valid</param>
        /// <param name="message">The message. deserialized from base 64</param>
        /// <param name="headers">The headers. converted in dictionary'</param>
        void Push(Guid uuid, string address, string message, Dictionary<string, object> headers);

    }

}


