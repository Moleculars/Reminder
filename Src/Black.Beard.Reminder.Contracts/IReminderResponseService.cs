﻿using System;
using System.Collections.Generic;

namespace Bb.Reminder
{

    public interface IReminderResponseService : IDisposable
    {

        string MethodName { get; }

        void Push(Guid uuid, string address, string message, Dictionary<string, object> headers);

    }

}