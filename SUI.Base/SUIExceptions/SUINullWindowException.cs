using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.SUIExceptions
{
    public class SUINullWindowException : SUIException
    {
        private static string defaultMsg = SUIBaseErrorMessages.Window_CannotInitializeWindow;

        public SUINullWindowException()
            : base(defaultMsg, null)
        {
        }

        public SUINullWindowException(string message)
            : base(message, null)
        {
        }

        public SUINullWindowException(Exception innerException)
            : base(defaultMsg, innerException)
        {
        }

        public SUINullWindowException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
