using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUICheckBox : SUIButton
    {
        public SUICheckBox(SUIWindow win)
            : base(win.WindowHandle)
        { }

        public SUICheckBox(IntPtr hWnd)
            : base(hWnd)
        { }

    }
}
