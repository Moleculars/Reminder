
using System;
using System.Collections.Generic;

namespace Bb.Reminder
{

    [System.Diagnostics.DebuggerDisplay("{Uuid}")]
    public class WakeUpRequestModel
    {

        public WakeUpRequestModel()
        {
            this.Headers = new Dictionary<string, object>();
        }

        public Guid Uuid { get; set; }

        public DateTimeOffset CurrentDateCaller { get; set; }

        public int DelayInMinute { get; set; }

        public string Address { get; set; }

        public string Binding { get; set; }

        public string Message { get; set; }

        public Dictionary<string, object> Headers { get; set; }

    }

}