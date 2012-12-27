using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetContextMenuStrip : SUIDotNetMenuStrip
    {
        public  SUIDotNetContextMenuStrip(IntPtr hWnd) 
            : base(hWnd)
        {
        }
        public SUIDotNetContextMenuStrip(SUIWindow sui)
            : base(sui)
        {
        }

        public SUIDotNetContextMenuStrip(SUIDotNetControl ctrl)
            : base(ctrl)
        {
        }
    }
}
