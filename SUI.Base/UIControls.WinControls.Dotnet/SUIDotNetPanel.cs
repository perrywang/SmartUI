using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetPanel : SUIDotNetControl
    {
        public  SUIDotNetPanel(IntPtr hWnd) 
            : base(hWnd)
        {   
        }
        
        public SUIDotNetPanel(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetPanel(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
    }
}
