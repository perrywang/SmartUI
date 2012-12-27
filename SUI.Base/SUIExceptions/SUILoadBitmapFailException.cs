using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base;

namespace SUI.Base.SUIExceptions
{
    public class SUILoadBitmapFailException : SUIException
    {
        private static string defaultMsg = SUIBaseErrorMessages.Bmp_Cannotloadbmp;

        public SUILoadBitmapFailException(): base(defaultMsg, null)
        {
        }

        public SUILoadBitmapFailException(string message) : base(message, null)
        {
        }

        public SUILoadBitmapFailException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUILoadBitmapFailException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
