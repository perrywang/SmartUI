using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIPropertySheet : SUIDialog
    {
        public SUIPropertySheet(IntPtr hWnd)
            : base(hWnd)
        { }

        public SUIPropertySheet(SUIWindow win)
            : base(win.WindowHandle)
        { }

        public SUIPropertyPage CurrentPage
        {
            get
            {
                int handle = SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.PSM_GETCURRENTPAGEHWND, IntPtr.Zero, IntPtr.Zero);
                return new SUIPropertyPage(new IntPtr(handle));
            }
        }

        public SUIPropertyPage ShowPage(int index)
        {
            SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.PSM_SETCURSEL, new IntPtr(index), IntPtr.Zero);
            return CurrentPage;
        }
    }
}
