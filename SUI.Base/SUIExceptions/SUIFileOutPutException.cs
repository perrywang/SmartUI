using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.SUIExceptions
{
    public class SUIFileOutPutException : SUIException
    {
        private static string defaultMsg = SUIBaseErrorMessages.File_OutPut_Err;

        public SUIFileOutPutException(): base(defaultMsg, null)
        {
        }

        public SUIFileOutPutException(string message) : base(message, null)
        {
        }

        public SUIFileOutPutException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUIFileOutPutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
