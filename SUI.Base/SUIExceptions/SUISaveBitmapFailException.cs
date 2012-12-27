using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base;

namespace SUI.Base.SUIExceptions
{
    public class SUISaveBitmapFailException : SUIException
    {
        private static string defaultMsg = SUIBaseErrorMessages.Bmp_SavebmpErr;

        public SUISaveBitmapFailException(): base(defaultMsg, null)
        {
        }

        public SUISaveBitmapFailException(string message) : base(message, null)
        {
        }

        public SUISaveBitmapFailException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUISaveBitmapFailException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
