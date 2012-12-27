using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Collections;
using System.Threading;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIButton : SUIWindow
    {
        public static string ButtonClassName = "Button";
        public SUIButton(IntPtr hWnd)
            : base(hWnd)
        { }

        public SUIButton(SUIWindow win)
            : base(win)
        { }

        public string Text
        {
            get
            {
                return WindowText;
            }
        }

        public void Click()
        {
            Focus();
            SUISleeper.Sleep(1000);
            SUIMouse.MouseClick(this, (Width-X)/2, (Height-Y)/2);
        }

        public void Click(bool isMessage)
        {
            if (isMessage)
            {
                SUIWinAPIs.PostMessage(this.WindowHandle, SUIMessage.WM_LBUTTONDOWN, 0, 0);
                SUIWinAPIs.PostMessage(this.WindowHandle, SUIMessage.WM_LBUTTONUP, 0, 0);
            }
            else
            {
                Click();
            }
            
        }

        public int GetState()
        {
            return SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.BM_GETSTATE, 0, 0).ToInt32();
        }

        public bool IsDefaultPushButton
        {
            get
            {
                return (HasWindowStyle(SUIMessage.BS_DEFPUSHBUTTON) && !IsCheckBox && !IsAutoCheckBox && !IsRadioButton && !IsAutoRadioButton);
            }
        }

        public bool IsGroupBox
        {
            get
            {
                return HasWindowStyle(SUIMessage.BS_GROUPBOX);
            }
        }

        public bool IsCheckBox
        {
            get
            {
                return HasWindowStyle(SUIMessage.BS_CHECKBOX);
            }
        }

        public bool IsAutoCheckBox
        {
            get
            {
                return HasWindowStyle(SUIMessage.BS_AUTOCHECKBOX);
            }
        }

        public bool IsRadioButton
        {
            get
            {
                return HasWindowStyle(SUIMessage.BS_RADIOBUTTON);
            }
        }

        public bool IsAutoRadioButton
        {
            get
            {
                return HasWindowStyle(SUIMessage.BS_AUTORADIOBUTTON);
            }
        }
        //some button is checkbutton, not simply using click method
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

    public class SUIButtonCollection : ReadOnlyCollectionBase
    {
        public void Add(SUIButton button)
        {
            this.InnerList.Add(button);
        }

        public SUIButton this[int index]
        {
            get
            {
                return (SUIButton)this.InnerList[index];
            }
        }

        public SUIButton this[string text]
        {
            get
            {
                return null;
            }
        }

    }
}
