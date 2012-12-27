using System;
using System.Collections.Generic;
using System.Text;
using Accessibility;
using SUI.Base.Win;
using SUI.Base.SUIAccessible;
using System.Drawing;
namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIMsoCommandBarPopup : SUIWindow
    {
        public const string WindowClass = "MsoCommandBarPopup";
        private SUIAccessibility CurAccessibile = null;
        private SUIMsoCommandBarPopup popupedMenu = null;
        public SUIMsoCommandBarPopup(IntPtr HWnd)
            : base(HWnd)
        {
            
        }
        public SUIMsoCommandBarPopup(SUIWindow suiwin)
            : base(suiwin)
        {
            
        }
        public SUIMsoCommandBarPopup PopupSubMenu(int index)
        {
            if (ItemList[index] != null)
            {
                ItemList[index].Click();
                SUISleeper.Sleep(500);
                SUIWinAPIs.EnumWindows(new EnumSMARTUIWindowsProc(this.FindPopupMenu), 0);
                return popupedMenu;
            }
            return null;
        }

        public SUIMsoCommandBarPopup PopupSubMenu(string itemText)
        {
            MsoCommandBarItemPopup target = null;
            foreach (MsoCommandBarItemPopup item in ItemList)
            {
                if (item != null && item.Name.Equals(itemText))
                {
                    target = item;
                    break;
                }
            }
            if (target != null)
            {
                target.Click();
                SUISleeper.Sleep(500);
                SUIWinAPIs.EnumWindows(new EnumSMARTUIWindowsProc(this.FindPopupMenu), 0);
                return popupedMenu;
            }
            return null;
        }

        public int FindPopupMenu(IntPtr hWnd,int lParam)
        {
            SUIWindow sui = new SUIWindow(hWnd);
            if ((sui != null) && (sui.ClassName.Equals("MsoCommandBarPopup")))
            {
                popupedMenu = new SUIMsoCommandBarPopup(hWnd);
                return 0;
            }
            return 1;
        }

        public void Click(int index)
        {
            if (ItemList[index] != null)
            {
                ItemList[index].Click();
            }
        }

        public void Click(string itemText)
        {
            MsoCommandBarItemPopup target = null;
            foreach (MsoCommandBarItemPopup item in ItemList)
            {
                if (item != null && item.Name.Equals(itemText))
                {
                    target = item;
                    break;
                }
            }
            if (target != null)
            {
                target.Click();
            }
        }

        public int GetItemIndexFromPoint(Point p)
        {
            //do something get index fron point
            if (itemList == null)
            {
                InitToolBar();
            }
            int length = itemList.Length;
            System.Diagnostics.Debug.WriteLine("Item length:" + length.ToString());
            System.Diagnostics.Debug.WriteLine("Point:" + p.ToString());
            for (int i = 0; i < length; i++)
            {
                if (itemList[i] != null)
                    System.Diagnostics.Debug.WriteLine("itemList[i]:" + itemList[i].RECT.ToString());
                if (itemList[i] != null && itemList[i].RECT.Contains(p))
                {
                    return i;
                }
            }
            System.Diagnostics.Debug.WriteLine("return -1");
            return -1;
        }
        public MsoCommandBarItemPopup[] ItemList
        {
            get
            {
                if (itemList == null)
                {
                    InitToolBar();
                }
                return itemList;
            }
        }
        private MsoCommandBarItemPopup[] itemList = null;
       
        private void InitToolBar()
        {
            if (this.WindowHandle != null)
            {
                CurAccessibile = new SUIAccessibility(WindowHandle, AccType.NativeOM);
                int count = CurAccessibile.ChildCount;
                itemList = new MsoCommandBarItemPopup[count];
                int index = 0;
                for (int i = 1; i <= count; i++)
                {
                    if (CurAccessibile.Children[i].IsVisible || CurAccessibile.Children[i].Height > 0)//Judge whether it is visible not only by property IsVisible but also Height
                    {
                        itemList[index] = new MsoCommandBarItemPopup(CurAccessibile.Children[i]);
                        index++;
                    }
                }
            }
        }

        //Sometimes, it needs a few seconds to initialize context menu and show it.
        //So I add a timeout parameter for this operation.
        public static SUIMsoCommandBarPopup MouseRightClick(SUIWindow window, string strWindowsText, int x, int y, int timeout)
        {
            SUIMouse.MouseRightClick(window, x, y);

            SUIWindow win = null;
            while (win == null && timeout > 0)
            {
                SUISleeper.Sleep(200);
                win = SUIWindow.DesktopWindow.FindChildWindow(WindowClass, strWindowsText);
                timeout--;
            }

            if (win == null)
                return null;

            return new SUIMsoCommandBarPopup(win);
        }

        public static SUIMsoCommandBarPopup MouseRightClick(SUIWindow window, string strWindowsText, int x, int y)
        {
            //By default, I set timeout as 6 seconds!
            return MouseRightClick(window, strWindowsText, x, y, 30);
        }

    }
    public class MsoCommandBarItemPopup
    {
        public MsoCommandBarItemPopup()
        { }
        private SUIAccessibility activeaccessibility = null;
        public MsoCommandBarItemPopup(SUIAccessibility acc)
        {
            activeaccessibility = acc;
        }
        public void Click()
        {
            if (activeaccessibility != null)
            {
                activeaccessibility.Click();
            }
        }
        public string Name
        {
            get
            {
                return activeaccessibility.Name;
            }
        }
        public int Top
        {
            get
            {
                return activeaccessibility.Top;
            }
        }
        public int Left
        {
            get
            {
                return activeaccessibility.Left;
            }
        }

        public Rectangle RECT
        {
            get
            {
                return activeaccessibility.RECT;
            }
        }

        public bool IsVisilbe
        {
            get
            {
                return activeaccessibility.IsVisible;
            }
        }      

    }

}

