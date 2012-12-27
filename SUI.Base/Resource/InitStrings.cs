using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SUI.Base.Utility;

namespace SUI.Base.Resource
{
    public class InitStrings
    {
        public static string LogFile = "Running.log";
        public static string WOW6432 = SUIUtil.IsX64 ? @"\Wow6432Node" : "";
    }
}
