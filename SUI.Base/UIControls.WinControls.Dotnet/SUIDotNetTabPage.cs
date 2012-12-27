using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetTabPage : SUIDotNetControl
    {
        public  SUIDotNetTabPage(IntPtr hWnd) 
            : base(hWnd)
        {   
        }
        
        public SUIDotNetTabPage(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetTabPage(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
    }
}
