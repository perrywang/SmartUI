using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.SUIExceptions
{
    public class SUIMultiThreadException : SUIException
    {
        private static string defaultMsg = SUIBaseErrorMessages.Thread_MultiThreadErr;

        public SUIMultiThreadException(): base(defaultMsg, null)
        {
        }

        public SUIMultiThreadException(string message) : base(message, null)
        {
        }

        public SUIMultiThreadException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUIMultiThreadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
