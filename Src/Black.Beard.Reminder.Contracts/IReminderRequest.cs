using System;
using System.Collections.Generic;
using System.Text;

namespace Bb.Reminder
{

    public interface IReminderRequest : IDisposable
    {

        void Watch(WakeUpRequestModel model);

        void Cancel(Guid uuid);

    }

}
