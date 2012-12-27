using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.SUIExceptions
{
    public class SUIIENotFoundException : SUIException
    {
        private static string defaultMsg = SUIBaseErrorMessages.IE_CannotFindIEWindow;

        public SUIIENotFoundException(): base(defaultMsg, null)
        {
        }

        public SUIIENotFoundException(string message) : base(message, null)
        {
        }

        public SUIIENotFoundException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUIIENotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
