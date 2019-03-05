using Bb.Reminder;
using System;
using System.Collections.Generic;

namespace Black.Beard.ReminderResponse.Broker.Http
{
    public class ReminderResponseHttpPut : IReminderResponseService, IDisposable
    {

        public ReminderResponseHttpPut()
        {

        }

        public string MethodName => "http.put";

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Push(Guid uuid, string address, string message, Dictionary<string, object> headers)
        {

        }

    }
}
