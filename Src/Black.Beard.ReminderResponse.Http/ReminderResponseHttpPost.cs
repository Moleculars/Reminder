using Bb.Reminder;
using System;
using System.Collections.Generic;

namespace Black.Beard.ReminderResponse.Broker.Http
{

    public class ReminderResponseHttpPost : IReminderResponseService
    {

        public ReminderResponseHttpPost()
        {

        }

        public string MethodName => "http.post";

        public void Push(Guid uuid, string address, string message, Dictionary<string, object> headers)
        {

        }

    }
}
