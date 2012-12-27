using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using SUI.Base.UIControls.WinControls.Win32;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetTextBox : SUIDotNetControl
    {
        private SUITextBox txt = null;
        public  SUIDotNetTextBox(IntPtr hWnd) 
            : base(hWnd)
        {
            txt = new SUITextBox(hWnd);
        }
        
        public SUIDotNetTextBox(SUIWindow sui)
            : this(sui.WindowHandle)
        {
            
        }

        public SUIDotNetTextBox(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
           
        }

        public string Text
        {
            get
            {
                return txt.Text;
            }
            set
            {
                txt.Text = value;
            }
        }
        
    }
}
