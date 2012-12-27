using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Runtime.InteropServices;
using SUI.Base.Utility;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIComboBox : SUIWindow
    {
        public SUIComboBox(IntPtr hWnd)
            : base(hWnd)
        { }

        public SUIComboBox(SUIWindow win)
            : base(win)
        { }

        public const int MaxLength = 256;
        StringBuilder text = null;
        public string Text
        {
            get
            {
                text = new StringBuilder(MaxLength);
                SUIWinAPIs.SendMessage(this.WindowHandle, SUIMessage.WM_GETTEXT, text.Capacity, text);
                return text.ToString();
            }
            set
            {
                if (IsDropDown)
                {                                    
                    SUIWinAPIs.SendMessage(this.WindowHandle, SUIMessage.WM_SETTEXT, 0, value);
                    IntPtr wParam = SUIUtil.MakeWParam(ControlID, SUIMessage.CBN_EDITCHANGE);
                    SUIWinAPIs.SendMessage(Parent.WindowHandle, SUIMessage.WM_COMMAND, wParam, WindowHandle);                                   
                }
                else
                {
                    throw new SUIException("You cannot edit text in this combobox!");
                }
            }
        }

        public int DropdownListTextCount
        {
            get
            {
                return SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.CB_GETCOUNT, 0, 0).ToInt32();
            }
        }

        public string GetDropdownListText(int index)
        {
            if (index < 0 || index >= DropdownListTextCount)
            {
                throw new SUIException("Index is out of range!");
            }

            text = new StringBuilder(MaxLength);
            SUIWinAPIs.SendMessage(this.WindowHandle, SUIMessage.CB_GETLBTEXT, index, text);
            return text.ToString();
        }

        public bool IsDropDownList
        {
            get
            {
                return (WindowStyle & 3)== SUIMessage.CBS_DROPDOWNLIST;
            }
        }

        public bool IsDropDown
        {
            get
            {
                return (WindowStyle & 3)== SUIMessage.CBS_DROPDOWN;
            }
        }

        public bool IsSimple
        {
            get
            {
                return (WindowStyle & 3)== SUIMessage.CBS_SIMPLE;
            }
        }

        public int SelectedIndex
        {
            get
            {
                if (IsDropDown || IsDropDownList || IsSimple)
                {
                    return SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.CB_GETCURSEL, 0, 0).ToInt32();
                }
                else
                {
                    throw new SUIException("You cannot try to get selected index for this combobox!");
                }
            }
        }

        private void CommitSelectionChange()
        {
            IntPtr wParam = SUIUtil.MakeWParam(ControlID, SUIMessage.CBN_SELCHANGE);
            SUIWinAPIs.SendMessage(Parent.WindowHandle, SUIMessage.WM_COMMAND, wParam, WindowHandle);
        }

        public void Select(int index)
        {
            if (index < 0 || index >= DropdownListTextCount)
            {
                throw new SUIException("Index is out of range!");
            }
            if (this.SelectedIndex != index)
            {
                IntPtr wParam = SUIUtil.MakeWParam(this.ControlID, 3);
                int num3 = SUIWinAPIs.PostMessage(this.Parent.WindowHandle, 0x111, wParam, this.WindowHandle);
                wParam = SUIUtil.MakeWParam(this.ControlID, 7);
                num3 = SUIWinAPIs.PostMessage(this.Parent.WindowHandle, 0x111, wParam, this.WindowHandle);
                wParam = new IntPtr(index);
                SUIWinAPIs.SendMessage(this.WindowHandle, 0x14e, wParam, IntPtr.Zero);
                wParam = SUIUtil.MakeWParam(this.ControlID, 1);
                num3 = SUIWinAPIs.PostMessage(this.Parent.WindowHandle, 0x111, wParam, this.WindowHandle);
                wParam = SUIUtil.MakeWParam(this.ControlID, 9);
                num3 = SUIWinAPIs.PostMessage(this.Parent.WindowHandle, 0x111, wParam, this.WindowHandle);
                wParam = SUIUtil.MakeWParam(this.ControlID, 8);
                num3 = SUIWinAPIs.PostMessage(this.Parent.WindowHandle, 0x111, wParam, this.WindowHandle);
            }
        }

        public void ClickSelect(int index)
        {
            if (this.SelectedIndex != index)
            {
                if (IsDropDown)
                    SUIMouse.MouseClick(this, this.Width - this.X - 5, (this.Height - this.Y) / 2);
                else
                    this.Focus();
                int offset = index - this.SelectedIndex;
                if (offset > 0)
                {
                    for (int i = 0; i < offset; i++)        
                        SUIKeyboard.Type(SUI.Base.Win.SUIKeyboard.VK.DOWN);
                }
                else
                {
                    offset = Math.Abs(offset);
                    for (int j = 0; j < offset; j++)
                        SUIKeyboard.Type(SUI.Base.Win.SUIKeyboard.VK.UP);
                }
                SUISleeper.Sleep(500);
                if(IsDropDown)
                    SUIKeyboard.Type(SUI.Base.Win.SUIKeyboard.VK.RETURN);
            }
        }

        public void Select(string str)
        {
            if (IsDropDown)
            {
                Text = str;
            }
            else
            {
                //First of all, search that whether parameter string is one of dropdown list strings.
                int index = -1;
                for (int i = 0; i < DropdownListTextCount; i++)
                {
                    if (GetDropdownListText(i).Equals(str))
                    {
                        Select(i);
                        return;
                    }
                }

                //Don't match. So we need specify this text if this is a dropdown combobox.
                throw new SUIException("The string you specified is not in dropdown list.");
            }
        }

        public void Set_Text(string text)
        {
            Text = text;
        }
    }
}
