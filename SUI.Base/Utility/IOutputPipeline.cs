using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.Utility
{
    //We only pass the string value as the event argument.
    public delegate void PipelineWriteHandler(string valueToWrite);

    public interface IOutputPipeline
    {
        void Write(int value);
        void Write(bool value);
        void Write(string value);
        void WriteLine(int value);
        void WriteLine(bool value);
        void WriteLine(string value);
        void WriteLine();
    }

    //For this class, you can simply create a new instance and add a event handler to "OnPipelineWrite"
    // event. You can implement your own output logic in event handler.
    public class SUIOutputPipeline : IOutputPipeline
    {
        public void Write(string value)
        {
            OnPipelineWrite(value);
        }

        public void Write(int value)
        {
            OnPipelineWrite(Convert.ToString(value));
        }

        public void Write(bool value)
        {
            OnPipelineWrite(Convert.ToString(value));
        }

        public void WriteLine(int value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(value);
            sb.AppendLine();
            OnPipelineWrite(sb.ToString());
        }

        public void WriteLine(bool value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(value);
            sb.AppendLine();
            OnPipelineWrite(sb.ToString());
        }

        public void WriteLine(string value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(value);
            sb.AppendLine();
            OnPipelineWrite(sb.ToString());
        }

        public void WriteLine()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            OnPipelineWrite(sb.ToString());
        }
        //This event is supposed to be called when Write() method is called.
        public event PipelineWriteHandler OnPipelineWrite;
    }
}
