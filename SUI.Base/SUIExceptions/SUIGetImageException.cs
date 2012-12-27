using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base;

namespace SUI.Base.SUIExceptions
{
    public class SUIGetImageException : SUIException  
    {
        private static string defaultMsg = SUIBaseErrorMessages.Img_CannotGetImg;

        public SUIGetImageException(): base(defaultMsg, null)
        {
        }

        public SUIGetImageException(string message) : base(message, null)
        {
        }

        public SUIGetImageException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUIGetImageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
