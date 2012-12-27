using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetTabControl : SUIDotNetControl
    {
        public  SUIDotNetTabControl(IntPtr hWnd) 
            : base(hWnd)
        {
            
        }
        
        public SUIDotNetTabControl(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetTabControl(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
        
    }
}
