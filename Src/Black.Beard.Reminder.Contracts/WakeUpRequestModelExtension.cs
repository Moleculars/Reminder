
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Reminder
{
    /// <summary>
    /// Wakeup request extension
    /// </summary>
    public static class WakeUpRequestModelExtension
    {

        /// <summary>
        /// Get property Headers and translate in dictionary
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetHeaders(this WakeUpRequestModel self)
        {
            Dictionary<string, object> headers;
            if (!string.IsNullOrEmpty(self.Headers))
                headers = self.Headers.Split(';')
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Select(c => c.Trim().Split('='))
                    .Where(c => !string.IsNullOrEmpty(c[0]) && !string.IsNullOrEmpty(c[1]))
                    .ToDictionary(c => c[0], c => (object)c[1]);
            else
                headers = new Dictionary<string, object>();
            return headers;
        }

        /// <summary>
        /// Gets the message from Message property and convert from base 64.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        public static string GetMessage(this WakeUpRequestModel self)
        {
            byte[] bytes = Convert.FromBase64String(self.Message);
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            return message;
        }

        /// <summary>
        /// Sets the message from payload and store in base 64.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        public static WakeUpRequestModel SetMessage(this WakeUpRequestModel self, string payload)
        {
            self.Message = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payload));
            return self;
        }

    }

}