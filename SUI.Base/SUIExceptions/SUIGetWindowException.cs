using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base;

namespace SUI.Base.SUIExceptions
{
    public class SUIGetWindowException : SUIException
    {
        private static string defaultMsg = SUIBaseErrorMessages.Window_CannotGetWindow;

        public SUIGetWindowException(): base(defaultMsg, null)
        {
        }

        public SUIGetWindowException(string message) : base(message, null)
        {
        }

        public SUIGetWindowException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUIGetWindowException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
