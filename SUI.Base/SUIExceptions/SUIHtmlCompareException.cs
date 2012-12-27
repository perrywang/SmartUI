using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.SUIExceptions
{
    public class SUIHtmlCompareException : SUIException
    {
        private static string defaultMsg = SUIBaseErrorMessages.HTMLCompareErr;

        public SUIHtmlCompareException(): base(defaultMsg, null)
        {
        }

        public SUIHtmlCompareException(string message) : base(message, null)
        {
        }

        public SUIHtmlCompareException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUIHtmlCompareException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
