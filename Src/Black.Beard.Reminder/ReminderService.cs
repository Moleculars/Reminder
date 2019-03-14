using Bb.Reminder.Exceptions;
using System;
using System.Collections.Generic;

namespace Bb.Reminder
{

    /// <summary>
    /// Implementation of ref="Bb.Reminder.IReminderRequest" />
    /// </summary>
    /// <seealso cref="Bb.Reminder.IReminderRequest" />
    /// <seealso cref="System.IDisposable" />
    public class ReminderService : IReminderRequest, IDisposable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ReminderService"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public ReminderService(IReminderStore store)
        {

            _store = store;
            _store.WakeUp = WakeUp;
            _methods = new Dictionary<string, IReminderResponseService>();

        }

        /// <summary>
        /// Adds the specified responses services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void AddResponseServices(params IReminderResponseService[] services)
        {

            foreach (var item in services)
                _methods.Add(item.MethodName, item);

        }

        /// <summary>
        /// Gets the available bindings in the current instance.
        /// </summary>
        /// <returns></returns>
        public string[] GetAvailableBindings()
        {
            List<string> result = new List<string>();
            foreach (var item in _methods.Values)
                result.Add(item.MethodName);
            return result.ToArray();
        }

        /// <summary>
        /// Watches the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <exception cref="InvalidMethodException">if the parameter binding not match with registred responses</exception>
        public void Watch(WakeUpRequestModel model)
        {
            if (!_methods.TryGetValue(model.Binding, out IReminderResponseService service))
                throw new InvalidMethodException(model.Binding);
            _store.Watch(model);
        }

        /// <summary>
        /// Cancels the watching.
        /// </summary>
        /// <param name="uuid">The UUID.</param>
        public void Cancel(Guid uuid)
        {
            _store.Cancel(uuid);
        }

        /// <summary>
        /// method to invoke when d time is expirated.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <exception cref="Exception">Missing method {model.Binding}</exception>
        private void WakeUp(Bb.Reminder.WakeUpRequestModel model)
        {

            if (_methods.TryGetValue(model.Binding, out IReminderResponseService service))
                service.Push(model.Uuid, model.Address, model.GetMessage(), model.GetHeaders());

            else
                throw new Exception($"Missing method {model.Binding}");

        }

        #region IDisposable Support

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _store.WakeUp -= WakeUp;

                    foreach (var item in _methods)
                        item.Value.Dispose();

                }

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~ReminderService() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }

        #endregion

        private readonly IReminderStore _store;
        private bool disposedValue = false; // Pour détecter les appels redondants
        private Dictionary<string, IReminderResponseService> _methods;

    }

}
