using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SUI.Base.Utility;

namespace SUI.Base
{
    public class SUIException : ApplicationException
    {
        private static string defaultMsg = "Unknown SUIException";
        public static bool blShowStackTrace = true; 

        public SUIException()
            : base(defaultMsg, null)
        {
        }

        public SUIException(string message) : base(message, null)
        {
        }

        public SUIException(Exception innerException)
            : base(defaultMsg, innerException)
        {
        }

        public SUIException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public virtual string logText()
        {
            string text = "Exception: \"" + Message +"\"\n";
            if (this.InnerException != null)
            {
                text += ",which is caused by"; 
                if (InnerException is SUIException)
                {
                    text +=((SUIException)InnerException).logText();
                }
                else
                {
                    text += InnerException.Message;
                }
            }
            text += "\n";
            if (blShowStackTrace)
            {
                text += getStackTrace();
            }
            return text; 
        }

        private string getStackTrace()
        {
            string trace = StackTrace;
            if (this.InnerException != null)
            {
                if (InnerException is SUIException)
                {
                    trace += ((SUIException)InnerException).getStackTrace();
                }
                else
                {
                    trace += InnerException.StackTrace;
                }
            }

            return trace;

        }
    }

    public class SUIBaseLayerException : SUIException
    {
        private static string defaultMsg = "Unknown SUIBaseLayerException";

        public SUIBaseLayerException(): base(defaultMsg, null)
        {
        }

        public SUIBaseLayerException(string message) : base(message, null)
        {
        }

        public SUIBaseLayerException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUIBaseLayerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class SUIWindowException : SUIException
    {
        private static string defaultMsg = "Unknown SUIWindowException";

        public SUIWindowException(): base(defaultMsg, null)
        {
        }

        public SUIWindowException(string message) : base(message, null)
        {
        }

        public SUIWindowException(Exception innerException): base(defaultMsg, innerException)
        {
        }

        public SUIWindowException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override string logText()
        {
            return "";
        }
    }

    public class SUIFileException : SUIException
    {
        private static string defaultMsg = "Unknown SUIFileException";
        private static string unknown = "unknown";
        private string filePath;
        private string fileName;
        private FileOperationType operation;

        public enum FileOperationType : int
        {
            open,
            create,
            Read,
            Write,
            Other,
            Unknown
        }

        public SUIFileException(string FileName, string Message, FileOperationType t)
            : base(Message,null)
        {
            filePath = SUIUtil.getFilePathFromFileFullPath(FileName);
            
            if(filePath == null)
            {
                filePath = unknown;
                fileName = unknown;
            }else{
                fileName = SUIUtil.getFilePathFromFileFullPath(FileName);
                if(fileName == null)
                {
                    fileName = unknown;
                }
            }
            operation = t;
        }

        public SUIFileException(string FileName, string Message, FileOperationType t, Exception e):base(Message,e)
        {
            filePath = SUIUtil.getFilePathFromFileFullPath(FileName);

            if (filePath == null)
            {
                filePath = unknown;
                fileName = unknown;
            }
            else
            {
                fileName = SUIUtil.getFilePathFromFileFullPath(FileName);
                if (fileName == null)
                {
                    fileName = unknown;
                }
            }
            operation = t;
        }

        public SUIFileException()
            : base(defaultMsg, null)
        {
            filePath = unknown;
            fileName = unknown;
            operation = FileOperationType.Unknown;
        }

        public SUIFileException(string message)
            : base(message, null)
        {
            filePath = unknown;
            fileName = unknown;
            operation = FileOperationType.Unknown;
        }

        public SUIFileException(Exception innerException)
            : base(defaultMsg, innerException)
        {
            filePath = unknown;
            fileName = unknown;
            operation = FileOperationType.Unknown;
        }

        public SUIFileException(string message, Exception innerException)
            : base(message, innerException)
        {
            filePath = unknown;
            fileName = unknown;
            operation = FileOperationType.Unknown;
        }

        public override string logText()
        {
            return "";
        }
    }

}
