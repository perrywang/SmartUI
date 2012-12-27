using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SUI.Base.Win;
using System.Collections;
using SUI.Base.SUIAccessible;
using Accessibility;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace SUI.Base.UIControls.WinControls.Win32
{
    
    public class SUIMenu
    {
        private SUIWindow dockedMainFrame;
        public SUIWindow DockedMainFrame
        {
            get
            {
                return dockedMainFrame;
            }
        }
        public IAccessible IA
        {
            get
            {
                IAccessible iacc = null;
                SUIWinAPIs.AccessibleObjectFromWindow(dockedMainFrame.WindowHandle, (int)AccType.Menu, ref SUIAccessibility.GuidOfIAcc, ref iacc);
                return iacc;
            }
        }
        public int ItemCount
        {
            get
            {
                return IA.accChildCount;
            }
        }
        public SUIMenu(SUIWindow dockedMainFrame)
        {
            this.dockedMainFrame = dockedMainFrame;
        }
        public void Click(int nIndex)
        {
            MenuItems[nIndex].Click();
        }

        public SUIPopupMenu PopupMenu(int nIndex)
        {
            MenuItems[nIndex].Click();
            return SUIPopupMenu.FindPopupMenu();
        }

        public int GetItemIndexFromPoint(Point p)
        {
            SUIMenuItemList items = MenuItems;
            foreach (SUIMenuItem item in items)
            {
                if (item.RECT.Contains(p))
                    return item.Index;
            }
            return -1;
        }
        public SUIMenuItemList MenuItems
        {
            get
            {
                SUIMenuItemList list = new SUIMenuItemList();
                int count = IA.accChildCount;
                for (int i = 0; i < count; i++)
                {
                    SUIMenuItem item = new SUIMenuItem(this,i);
                    list.Add(item);
                }
                return list;
            }
        }
    }
    
    public class SUIMenuItemList : ReadOnlyCollectionBase
    {
        public void Add(SUIMenuItem menuItem)
        {
            this.InnerList.Add(menuItem);
        }

        public SUIMenuItem this[int index]
        {
            get
            {
                return (SUIMenuItem)this.InnerList[index];
            }
        }
    }
    public class SUIPopupMenu : SUIWindow
    {
       // private SUIWindow popupWindow;
        public SUIPopupMenu(IntPtr HWnd)
            : base(HWnd)
        {

        }
        public SUIPopupMenu(SUIWindow suiwin)
            : base(suiwin)
        {

        }
        //public SUIWindow PopupWindow
        //{
        //    get
        //    {
        //        return popupWindow;
        //    }
        //}
        public IAccessible IA
        {
            get
            {
                IAccessible iacc = null;
                SUIWinAPIs.AccessibleObjectFromWindow(this.WindowHandle, (int)AccType.Client, ref SUIAccessibility.GuidOfIAcc, ref iacc);
                return iacc;
            }
        }
        public static SUIPopupMenu FindPopupMenu() //for all the popup menu
        {
            try
            {
                int timeout = 10;
                int timeCounter = 0;
                while (true)
                {
                    SUISleeper.Sleep(500);
                    IntPtr popupWindowHandle = SUIWinAPIs.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "#32768", string.Empty);
                    if (!popupWindowHandle.Equals(IntPtr.Zero))
                    {
                        return new SUIPopupMenu(popupWindowHandle);
                    }
                    timeCounter++;
                    if (timeCounter > timeout)
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("No menu popup.");
            }
        }

        public void Click(int nIndex)
        {
            PopupMenuItems[nIndex].Click();
        }

        public int GetItemIndexFromPoint(Point p)
        {
            SUIPopupMenuItemList items = PopupMenuItems;
            foreach (SUIPopupMenuItem item in PopupMenuItems)
            {
                if (item.RECT.Contains(p))
                    return item.Index;
            }
            return -1;
        }

        public SUIPopupMenu PopupSubMenu(int index)
        {
            Click(index);
            return FindPopupMenu();
        }

        public SUIPopupMenuItemList PopupMenuItems
        {
            get
            {
                SUIPopupMenuItemList list = new SUIPopupMenuItemList();
                int count = IA.accChildCount;
                Trace.WriteLine("+++++++++++++++++++++++++++" + count + "*************************");
                for (int i = 0; i < count; i++)
                {
                    SUIPopupMenuItem item = new SUIPopupMenuItem(this, i);
                    list.Add(item);
                }
                return list;
            }
        }

        //Sometimes, it needs a few seconds to initialize context menu and show it.
        //So I add a timeout parameter for this operation.
        public static SUIPopupMenu MouseRightClick(SUIWindow window, string strWindowsText, int x, int y, int timeout)
        {
            SUIMouse.MouseRightClick(window, x, y);

            SUIWindow win = null;
            while (win == null && timeout > 0)
            {
                SUISleeper.Sleep(200);
                win = SUIWindow.DesktopWindow.FindChildWindow("#32768", strWindowsText);
                timeout--;
            }

            if (win == null)
                throw new SUIException("Fail to find context menus!");

            return new SUIPopupMenu(win);
        }

        public static SUIPopupMenu MouseRightClick(SUIWindow window, string strWindowsText, int x, int y)
        {
            //By default, I set timeout as 6 seconds!
            return MouseRightClick(window, strWindowsText, x, y, 30);
        }
    }
    
    public class SUIPopupMenuItemList : ReadOnlyCollectionBase
    {
        public void Add(SUIPopupMenuItem menuItem)
        {
            this.InnerList.Add(menuItem);
        }

        public SUIPopupMenuItem this[int index]
        {
            get
            {
                return (SUIPopupMenuItem)this.InnerList[index];
            }
        }
    }
}
