using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Drawing;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIListView : SUIWindow
    {
        public SUIListView(IntPtr hWnd)
            : base(hWnd)
        { }

        public SUIListView(SUIWindow win)
            : base(win)
        { }

        public SUIListViewHeader Header
        {
            get
            {
                IntPtr headerHandle = SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LVM_GETHEADER, 0, 0);
                if (headerHandle.Equals(IntPtr.Zero))
                    throw new SUIException("Cannot get header on this listview!");
                return new SUIListViewHeader(headerHandle);
            }
        }

        public void ClickHeaderItem(int index)
        {
            Header.ClickItem(index);
        }

        public int ColumnCount
        {
            get
            {
                int ret = -1;
                try
                {
                    ret = Header.Count;
                }
                catch (SUIException e)
                {
                    Console.WriteLine("Exception in listview:{0}", e.Message);
                    ret = 0;
                }
                return ret;
            }
        }

        public int Count
        {
            get
            {
                return SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LVM_GETITEMCOUNT, 0, 0).ToInt32();
            }
        }

        public int SelectedIndex
        {
            get
            {
                SUIListViewItem item = null;
                item = GetNextItem(-1, SUIMessage.LVNI_ALL);

                while (!item.IsInvalidItem)
                {
                    if (item.IsSelected)
                    {
                        return item.Index;
                    }
                    item = GetNextItem(item.LVITEM.iItem, SUIMessage.LVNI_ALL);
                }

                return -1;
            }
        }

        //Index of the item to begin the search with, 
        //or -1 to find the first item that matches the specified flags. 
        //The specified item itself is excluded from the search. 
        internal SUIListViewItem GetNextItem(int startItemIndex, int flag)
        {
            LVITEM item = new LVITEM();
            item.iItem = SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LVM_GETNEXTITEM, startItemIndex, flag).ToInt32();
            return new SUIListViewItem(this, item);
        }

        public SUIListViewItem GetItemByName(string name)
        {
            SUIListViewItem item = null;
            item = GetNextItem(-1, SUIMessage.LVNI_ALL);

            while (!item.IsInvalidItem)
            {
                if (name.Equals(item.Text))
                {
                    return item;
                }
                item = GetNextItem(item.LVITEM.iItem, SUIMessage.LVNI_ALL);
            }

            return null;
        }

        public SUIListViewItem GetItemByIndex(int index)
        {
            if (index < 0 || index + 1 > Count)
                throw new SUIException("Index is out of item range!");

            LVITEM i = new LVITEM();
            i.iItem = index;
            SUIListViewItem item = new SUIListViewItem(this, i);
            
            return item;
        }

        //Send LVM_GETEDITCONTROL to retrieve edit control.
        public SUIWindow EditControl
        {
            get
            {
                IntPtr handle = SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.LVM_GETEDITCONTROL, 0, 0);
                if (handle == null || handle.Equals(IntPtr.Zero))
                    throw new SUIException("No edit control found!");

                return new SUIWindow(handle);
            }
        }

        public void SetSubItemText(int rowIndex, int columnIndex,string subItemText)
        {
            SUIListViewItem slvitem = GetItemByIndex(rowIndex);
            SUIListViewSubitem subItem = slvitem.GetSubitem(columnIndex);
            subItem.Text = subItemText;
            
        }
        public void Click(int rowIndex, int columnIndex)
        {
            SUIListViewItem slvitem = GetItemByIndex(rowIndex);
            SUIListViewSubitem subItem = slvitem.GetSubitem(columnIndex);
            subItem.Click();

        }

        public void Click(int rowIndex, int columnIndex, int nFlags)
        {
            SUIListViewItem slvitem = GetItemByIndex(rowIndex);
            SUIListViewSubitem subItem = slvitem.GetSubitem(columnIndex);
            if (nFlags == 0) //Shift down
            {
                SUIKeyboard.Press(SUI.Base.Win.SUIKeyboard.VK.SHIFT);
                subItem.Click();
                SUIKeyboard.Release(SUI.Base.Win.SUIKeyboard.VK.SHIFT);
            }
            else if (nFlags == 1) //Control down
            {
                SUIKeyboard.Press(SUI.Base.Win.SUIKeyboard.VK.CONTROL);
                subItem.Click();
                SUIKeyboard.Release(SUI.Base.Win.SUIKeyboard.VK.CONTROL);
            }
            else
            {
                subItem.Click();
            }

        }
        
    }
}
