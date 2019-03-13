using Bb.Reminder;
using System;

namespace Bb.Reminder
{

    public interface IReminderStore : IDisposable
    {

        /// <summary>
        /// Watches the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        void Watch(WakeUpRequestModel model);

        /// <summary>
        /// Cancels the specified UUID.
        /// </summary>
        /// <param name="uuid">The UUID.</param>
        void Cancel(Guid uuid);

        /// <summary>
        /// Gets or sets the wake up.
        /// </summary>
        /// <value>
        /// The wake up.
        /// </value>
        Action<WakeUpRequestModel> WakeUp { get; set; }

    }

}