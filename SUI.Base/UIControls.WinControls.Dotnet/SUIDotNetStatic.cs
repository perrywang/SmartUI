using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetStatic : SUIDotNetControl
    {
        public  SUIDotNetStatic(IntPtr hWnd) 
            : base(hWnd)
        {
            
        }
        
        public SUIDotNetStatic(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetStatic(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }

        public void Click()
        {
            SUIMouse.MouseClick(this, (Width - X) / 2, (Height - Y) / 2);
        }
    }
}
