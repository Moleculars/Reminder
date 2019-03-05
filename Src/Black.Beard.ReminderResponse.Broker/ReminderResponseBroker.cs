using Bb.Brokers;
using Bb.Reminder;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bb.ReminderResponse.Broker
{

    public class ReminderResponseBroker : IReminderResponseService, IDisposable
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

            try
            {
                _publisher.Publish(message, headers).Wait();
            }
            catch (Exception e)
            {
                _publisher.Dispose();
                _publisher = null;
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
                {
                    if (_publisher != null)
                    {
                        _publisher.Dispose();
                        _publisher = null;
                    }
                }

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
        private IBrokerPublisher _publisher;
        private readonly string _publisherName;

    }

}
