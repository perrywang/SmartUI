using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Collections;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIDialog : SUIWindow
    {
        public static string DialogClassName = @"#32770";

        public SUIDialog(IntPtr hWnd)
            : base(hWnd)
        { }

        public SUIDialog(SUIWindow win)
            : base(win)
        { }

        public string Caption
        {
            get
            {
                return WindowText;
            }
        }

        public SUIWindow GetDialogItem(int dlgItemID)
        {
            IntPtr itemHandle = SUIWinAPIs.GetDlgItem(WindowHandle, dlgItemID);
            if(itemHandle != null && !itemHandle.Equals(IntPtr.Zero))
                return new SUIWindow(itemHandle);
            return null;
        }
        private int EnumControls(IntPtr ControlHandle, int lparam)
        {
            if (SUIWinAPIs.IsWindowVisible(ControlHandle))
            {
                SUIWindow controlWindow = new SUIWindow(ControlHandle);
                //if (controlWindow.IsDialog)
                //{
                //    SUIWinAPIs.EnumChildWindows(ControlHandle, new EnumSMARTUIWindowsProc(EnumControls), 0);
                //}
                //else
                string WindowClassName = controlWindow.ClassName;
                if (WindowClassName.Equals("Button") || WindowClassName.Equals("Edit") || WindowClassName.Equals("ComboBox") || WindowClassName.Equals("Static")
                    || WindowClassName.StartsWith("WindowsForms10.BUTTON.app"))
                {
                    int controlID = SUIWinAPIs.GetDlgCtrlID(ControlHandle);
                    if (!controlList.ContainsKey(controlID))
                    {
                        controlList.Add(controlID, controlWindow);
                    }
                }
            }
            return 1;
        }
        private IDictionary<int, SUIWindow> controlList = null;
        public  IDictionary<int, SUIWindow> ControlList
        {
            get
            {
                if (controlList == null)
                {
                    controlList = new Dictionary<int, SUIWindow>();
                    SUIWinAPIs.EnumChildWindows(this.WindowHandle, new EnumSMARTUIWindowsProc(EnumControls), 0);
                }
                return controlList;
            }
        }
        public void RefreshControlList()
        {
            if (controlList != null)
            {
                try
                {
                    SUIWinAPIs.EnumChildWindows(this.WindowHandle, new EnumSMARTUIWindowsProc(EnumControls), 0);
                }
                catch (Exception e)
                {
                    throw new SUIException("Refresh Dialog ControlList failed!");
                }
            }
        }
        public SUIButtonCollection Buttons
        {
            get
            {
                SUIButtonCollection buttons = new SUIButtonCollection();
                foreach (SUIWindow win in Children)
                {
                    if (win.IsButton)
                        buttons.Add(new SUIButton(win));
                }
                return buttons;
            }
            
        }

        public SUIDialogCollection ChildDialogs
        {
            get
            {
                SUIDialogCollection childDialogs = new SUIDialogCollection();
                foreach (SUIWindow win in Children)
                {
                    if (win.IsDialog)
                        childDialogs.Add(new SUIDialog(win));
                }
                
                return childDialogs;
            }
        }

        public SUIButton DefaultPushButton
        {
            get
            {
                SUIButton defaultBtn = null;
                foreach (SUIButton btn in Buttons)
                {
                    if (btn.IsDefaultPushButton)
                    {
                        defaultBtn = btn;
                        break;
                    }
                }
                return defaultBtn;
            }
        }
    }

    public class SUIDialogCollection : ReadOnlyCollectionBase
    {
        public void Add(SUIDialog dialog)
        {
            this.InnerList.Add(dialog);
        }

        public SUIDialog this[int index]
        {
            get
            {
                return (SUIDialog)this.InnerList[index];
            }
        }

        public SUIDialog this[string text]
        {
            get
            {
                return null;
            }
        }
    }
}
