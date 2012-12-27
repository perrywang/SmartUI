using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.UIControls.WinControls.Win32;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetListView : SUIDotNetControl
    {
        SUIListView suiListView = null;
        public  SUIDotNetListView(IntPtr hWnd) 
            : base(hWnd)
        {
            suiListView = new SUIListView(hWnd);
        }
        
        public SUIDotNetListView(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetListView(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
    }
}
