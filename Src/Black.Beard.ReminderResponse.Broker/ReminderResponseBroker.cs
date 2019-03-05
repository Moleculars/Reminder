using Bb.Brokers;
using Bb.Reminder;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bb.ReminderResponse.Broker
{

    public class ReminderResponseBroker : IReminderResponseService, IDisposable
    {

        public ReminderResponseBroker(IFactoryBroker broker)
        {
            _broker = broker;
            _publishers = new Dictionary<string, IBrokerPublisher>();
        }

        public string MethodName => "broker";

        public void Push(Guid uuid, string address, string message, Dictionary<string, object> headers)
        {

            if (!_publishers.TryGetValue(address, out IBrokerPublisher publisher))
                lock (_lock)
                    if (!_publishers.TryGetValue(address, out publisher))
                        _publishers.Add(address, publisher = _broker.CreatePublisher(address));

            try
            {
                publisher.Publish(message, headers).Wait();
            }
            catch (Exception e)
            {

                lock (_lock)
                    _publishers.Remove(address);

                publisher.Dispose();
                publisher = null;

                Trace.WriteLine(new { e.Message, Exception = e });
                throw;
            }

        }

        #region IDisposable Support

        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    foreach (var item in _publishers)
                        item.Value.Dispose();

                disposedValue = true;
            }
        }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }
        #endregion

        private IFactoryBroker _broker;
        private Dictionary<string, IBrokerPublisher> _publishers;
        private readonly object _lock = new object();

    }

}
