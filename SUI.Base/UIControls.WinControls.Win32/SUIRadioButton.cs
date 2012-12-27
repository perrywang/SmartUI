using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIRadioButton : SUIButton
    {
        public SUIRadioButton(SUIWindow win)
            : base(win.WindowHandle)
        { }
        public SUIRadioButton(IntPtr hWnd)
            : base(hWnd)
        { }

        public void Check()
        {
            if (!IsChecked)
                Click();
        }

        public bool IsChecked
        {
            get
            {
                return GetState() == SUIMessage.BST_CHECKED;
            }
        }

        public void Uncheck()
        {
            if (IsChecked)
                Click();
        }
    }
}
