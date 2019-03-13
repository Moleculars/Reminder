
using System;

namespace Bb.Reminder
{

    [System.Diagnostics.DebuggerDisplay("{Uuid} -> {Binding}")]
    public class WakeUpRequestModel
    {

        public WakeUpRequestModel()
        {

        }

        /// <summary>
        /// Gets or sets the UUID. Is unique identified given by caller. (consider unicity is call concern)
        /// </summary>
        /// <value>
        /// The UUID.
        /// </value>
        public Guid Uuid { get; set; }

        /// <summary>
        /// Gets or sets the delay in minute. time in minute when the message must be relayed
        /// </summary>
        /// <value>
        /// The delay in minute.
        /// </value>
        public int DelayInMinute { get; set; }

        /// <summary>
        /// Gets or sets the address. adresse of service. if the binding is broker, address is a publisher name, if the binding is http.* address is url valid
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the binding. way to relay the message
        /// </summary>
        /// <value>
        /// The binding.
        /// </value>
        public string Binding { get; set; }

        /// <summary>
        /// Gets or sets the message in base 64.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the headers to push with the request ex: (key1=value1;key2=value2; ...). 
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public string Headers { get; set; }

    }

}