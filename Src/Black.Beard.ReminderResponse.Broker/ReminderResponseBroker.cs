using Bb.Brokers;
using Bb.Reminder;
using System;
using System.Collections.Generic;

namespace Bb.ReminderResponse.Broker
{

    public class ReminderResponseBroker : IReminderResponseService
    {

        public ReminderResponseBroker(IFactoryBroker broker, string publisherName)
        {
            _broker = broker;
            _publisherName = publisherName;
        }

        public string MethodName => "broker";

        public void Push(Guid uuid, string address, string message, Dictionary<string, object> headers)
        {

            if (_publisher == null)
                _publisher = _broker.CreatePublisher(_publisherName);

            _publisher.Publish(message, headers).Wait();

        }

        private IFactoryBroker _broker;
        private IBrokerPublisher _publisher;
        private readonly string _publisherName;

    }

}
