using Bb.Reminder;
using System;

namespace Bb.Reminder
{

    public interface IReminderStore : IDisposable
    {

        void Watch(WakeUpRequestModel model);

        void Cancel(Guid uuid);

        Action<WakeUpRequestModel> WakeUp { get; set; }

    }

}