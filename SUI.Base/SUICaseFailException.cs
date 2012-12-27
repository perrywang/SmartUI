using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base
{
    public class SUICaseFailException : SUIException
    {
        public SUICaseFailException(string message) : base(message, null)
        {
        }

        public SUICaseFailException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
