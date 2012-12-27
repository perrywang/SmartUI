using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Resource;
using System.IO;

namespace SUI.Base
{
    public class SUILogger
    {
        private static SUILogger logger = null;
        private SUILogger()
        {
        }

        public static SUILogger Logger
        {
            get
            {
                if (logger == null)
                    logger = new SUILogger();
                return logger;
            }
        }

        public void Log(string text)
        {
            StreamWriter writer = File.AppendText(InitStrings.LogFile);
            writer.Write(System.DateTime.Now.ToLongTimeString());
            writer.Write("\t");
            writer.WriteLine(text);
            writer.Flush();
            writer.Close();
        }

        public void Clear()
        {
            if (File.Exists(InitStrings.LogFile))
                File.Delete(InitStrings.LogFile);
        }
    }
}
