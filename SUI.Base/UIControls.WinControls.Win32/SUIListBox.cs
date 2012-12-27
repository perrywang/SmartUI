using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Runtime.InteropServices;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIListBox : SUIWindow
    {
        public const string className = "ListBox";
        private StringBuilder itemText = new StringBuilder();

        public SUIListBox(IntPtr hWnd)
            : base(hWnd)
        {
        }

        public SUIListBox(SUIWindow win)
            : base(win)
        {
        }

        public int Count
        {
            get
            {
                return SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETCOUNT, 0, 0).ToInt32();
            }
        }

        public bool IsMultipleSelect
        {
            get
            {
                return HasWindowStyle(SUIMessage.LBS_MULTIPLESEL) || HasWindowStyle(SUIMessage.LBS_EXTENDEDSEL);
            }
        }

        //Zero based
        public RECT GetItemRECT(int index)
        {

            if (index < 0 || index >= Count)
                throw new SUIException("Index is out of range!");
            
            RECT rec = new RECT();
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(RECT)));

                SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETITEMRECT, index, ptr);

                rec = (RECT)Marshal.PtrToStructure(ptr, typeof(RECT));
            }
            catch (Exception e)
            {
                throw new SUIException("Error getting item rectangle!", e);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }

            return rec;
            
        }

        public void ClickItem(int index)
        {
            if (index < 0 || index >= Count)
                throw new SUIException("Index is out of range!");

            RECT rect = GetItemRECT(index);
            SUIMouse.MouseClick(this, rect.left + 5, rect.top + 5);
        }

        public int GetItemHeight(int index)
        {
            if (index < 0 || index >= Count)
                throw new SUIException("Index is out of range!");

            return SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETITEMHEIGHT, 0, 0).ToInt32();
        }

        public bool IsOwnerDrawVariable
        {
            get
            {
                return HasWindowStyle(SUIMessage.LBS_OWNERDRAWVARIABLE);
            }
        }
        
        public int SelectedIndex
        {
            get
            {
                return SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public string GetSelectItem()
        {
            int selectedItem = SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
            SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETTEXT, selectedItem, itemText);
            return itemText.ToString();
        }

        public void Select(int index)
        {
            //The selection messages are different for single/multiple selection control.
            if (IsMultipleSelect)
            {
                UnselectAll();
                MultipleSelect(index);
            }
            else
                SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_SETCURSEL, index, IntPtr.Zero);
        }

        public void Select(string selectText)
        {
            IntPtr count = SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETCOUNT, 0, 0);
            StringBuilder itemText = new StringBuilder();
            for (int i = 0; i < count.ToInt32(); i++)
            {
                SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETTEXT, i, itemText);
                if (itemText.ToString() == selectText)
                {
                    Select(i);
                    break;
                }
            }
        }

        public void MultipleSelect(int index)
        {
            SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_SETSEL, 1, new IntPtr(index));
        }

        public void MultipleSelect(string selectText)
        {
            IntPtr count = SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETCOUNT, 0, 0);
            StringBuilder itemText = new StringBuilder();
            for (int i = 0; i < count.ToInt32(); i++)
            {
                SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETTEXT, i, itemText);
                if (itemText.ToString() == selectText)
                {
                    MultipleSelect(i);
                    break;
                }
            }
        }

        public void UnselectAll()
        {
            if(IsMultipleSelect)
                SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_SETSEL, 0, new IntPtr(-1));
            else
                SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_SETCURSEL, -1, IntPtr.Zero);
        }

        public void ClickToSelect(int index)
        {
            SetTopIndex(index);
            SUISleeper.Sleep(200);
            ClickItem(index);
        }

        public void ClickToSelect(string text)
        {
            IntPtr count = SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETCOUNT, 0, 0);
            StringBuilder itemText = new StringBuilder();
            for (int i = 0; i < count.ToInt32(); i++)
            {
                SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETTEXT, i, itemText);
                if (itemText.ToString() == text)
                {
                    ClickToSelect(i);
                    break;
                }
            }
        }

        public void SetTopIndex(int index)
        {
            SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_SETTOPINDEX, index, IntPtr.Zero);
        }

        public string GetTextByIndex(int index)
        {
            StringBuilder itemText = new StringBuilder();
            SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LB_GETTEXT, index, itemText);
            return itemText.ToString();
        }
    }
}
