using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Collections;
using SUI.Base.UIControls.WinControls.Win32;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetForm : SUIDotNetControl
    {
        public SUIDotNetForm(IntPtr hWnd)
            : base(hWnd)
        { }
        
        public SUIDotNetForm(SUIWindow sui)
            : base(sui)
        { }

        public SUIDotNetForm(SUIDotNetControl ctrl)
            : base(ctrl)
        {
        }
    }
}
