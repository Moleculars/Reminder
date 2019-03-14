using System;
using System.Collections.Generic;
using System.Text;

namespace Bb.Reminder
{

    /// <summary>
    /// Internface to implement in storage
    /// </summary>
    public interface IInitializer
    {

        /// <summary>
        /// Initializes current object
        /// </summary>
        /// <param name="o">The o.</param>
        void Initialize(object o);

    }
}
