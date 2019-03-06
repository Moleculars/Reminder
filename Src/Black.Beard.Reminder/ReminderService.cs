using Bb.Reminder.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Reminder
{

    public class ReminderService : IReminderRequest, IDisposable
    {

        public ReminderService(IReminderStore store, params IReminderResponseService[] services)
        {

            _store = store;
            _store.WakeUp = WakeUp;
            _methods = new Dictionary<string, IReminderResponseService>();

            foreach (var item in services)
                _methods.Add(item.MethodName, item);

        }

        public void Watch(WakeUpRequestModel model)
        {
            if (!_methods.TryGetValue(model.Binding, out IReminderResponseService service))
                throw new InvalidMethodException(model.Binding);
            _store.Watch(model);
        }

        public void Cancel(Guid uuid)
        {
            _store.Cancel(uuid);
        }

        private void WakeUp(Bb.Reminder.WakeUpRequestModel model)
        {

            if (_methods.TryGetValue(model.Binding, out IReminderResponseService service))
            {

                byte[] bytes = Convert.FromBase64String(model.Message);
                string message = System.Text.Encoding.UTF8.GetString(bytes)
                    ;

                Dictionary<string, object> headers;
                if (!string.IsNullOrEmpty(model.Headers))
                    headers = model.Headers.Split(';')
                        .Where(c => !string.IsNullOrEmpty(c))
                        .Select(c => c.Trim().Split('='))
                        .Where(c => !string.IsNullOrEmpty(c[0]) && !string.IsNullOrEmpty(c[1]))
                        .ToDictionary(c => c[0], c => (object)c[1]);
                else
                    headers = new Dictionary<string, object>();

                service.Push(model.Uuid, model.Address, message, headers);

            }
            else
                throw new Exception($"Missing method {model.Binding}");

        }

        #region IDisposable Support

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
