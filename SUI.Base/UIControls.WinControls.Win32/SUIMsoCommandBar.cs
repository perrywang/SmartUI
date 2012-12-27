using System;
using System.Collections.Generic;
using System.Text;
using Accessibility;
using SUI.Base.Win;
using SUI.Base.SUIAccessible;
using System.Drawing;
namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIMsoCommandBar : SUIWindow
    {
        public const string WindowClass = "MsoCommandBar";
        private SUIAccessibility CurAccessibile = null;
        private SUIMsoCommandBarPopup popupedMenu = null;
        public SUIMsoCommandBar(IntPtr HWnd)
            : base(HWnd)
        {   
        }
        public SUIMsoCommandBar(SUIWindow suiwin)
            : this(suiwin.WindowHandle)
        {
        }

        public SUIMsoCommandBarPopup PopupMenu(int index)
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

        public SUIMsoCommandBarPopup PopupMenu(string itemText)
        {
            MsoCommandBarItem target = null;
            foreach (MsoCommandBarItem item in ItemList)
            {
                if (item != null && item.Name.Equals(itemText))
                {
                    target = item;
                    break;
                }
            }
            if(target != null)
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
            MsoCommandBarItem target = null;
            foreach (MsoCommandBarItem item in ItemList)
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
        public MsoCommandBarItem[] ItemList
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
        private MsoCommandBarItem[] itemList = null;
       
        private void InitToolBar()
        {
            if(this.WindowHandle != null)
            {
                CurAccessibile = new SUIAccessibility(WindowHandle,AccType.NativeOM);
                int count = CurAccessibile.ChildCount;
                itemList = new MsoCommandBarItem[count];
                int index = 0;
                for (int i = 1; i < count; i++)
                {
                    if (CurAccessibile.Children[i].IsVisible)
                    {
                        itemList[index] = new MsoCommandBarItem(CurAccessibile.Children[i]);
                        index++;
                    }
                }
            }
        }

    }
    public class MsoCommandBarItem
    {
        public MsoCommandBarItem()
        { }
        private SUIAccessibility activeaccessibility = null;
        public MsoCommandBarItem(SUIAccessibility acc)
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

