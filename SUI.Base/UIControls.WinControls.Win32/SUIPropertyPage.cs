using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIPropertyPage : SUIDialog
    {
        public SUIPropertyPage(IntPtr hWnd)
            : base(hWnd)
        { }

        public SUIPropertyPage(SUIWindow win)
            : base(win.WindowHandle)
        { }
    }
}
