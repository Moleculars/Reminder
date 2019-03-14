using System;

namespace Bb.Reminder
{

    /// <summary>
    /// Interface that describe reminder base
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IReminderRequest : IDisposable
    {

        /// <summary>
        /// Watches the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <exception cref="InvalidMethodException">if the parameter binding not match with registred responses</exception>
        void Watch(WakeUpRequestModel model);

        /// <summary>
        /// Cancels the watching.
        /// </summary>
        /// <param name="uuid">The UUID.</param>
        void Cancel(Guid uuid);

    }

}
