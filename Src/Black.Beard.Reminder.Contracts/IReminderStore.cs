using Bb.Reminder;
using System;

namespace Bb.Reminder
{

    public interface IReminderStore
    {

        void Watch(WakeUpRequestModel model);

        void Cancel(Guid uuid);

        Action<WakeUpRequestModel> WakeUp { get; set; }

    }

}