using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.SUIAccessible;
using SUI.Base.Win;
using Accessibility;
using System.Drawing;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUITabControl : SUIWindow
    {
        private IAccessible IAcc; 
        public SUITabControl(IntPtr hWnd)
            : base(hWnd)
        {
            IntPtr Result = SUIWinAPIs.AccessibleObjectFromWindow(hWnd, (int)AccType.Client, ref SUIAccessibility.GuidOfIAcc, ref IAcc);
            if (!Result.Equals(IntPtr.Zero))
                throw new Exception("Object does not support IAccessible.");
        }
        public IAccessible Accessible
        {
            get
            {
                return IAcc;
            }
        }
        public SUITabControl(SUIWindow win)
            : this(win.WindowHandle)
        {
        }
        public int TabsCount
        {
            get
            {
                return IAcc.accChildCount;
            }
        }
        public void SelectTab(int index)
        {
            
            //IAcc.accSelect(3,index +1);
            //return SelectedTab;
            SUIMouse.MouseClick(TabItems[index].Rect.X+5,TabItems[index].Rect.Y+ 5);
        }
        public int SelectedIndex
        {
            get
            {
                return (int)IAcc.accSelection -1;
            }
        }
        public SUITabControlTab SelectedTab
        {
            get
            {
                return new SUITabControlTab(this, SelectedIndex +1);
            }
        }
        public SUITabControlTab[] TabItems
        {
            get
            {
                int count = TabsCount;
                //There are cases (ADAPT01073597) TabsCount is one bigger than the real tab count.
                //So we'll have a try to get the last tab's name.
                //If failed, we need decrease TabsCount by 1.
                try
                {
                    IAcc.get_accName(count);
                }
                catch
                {
                    count = TabsCount - 1;
                }

                SUITabControlTab[] tabs = new SUITabControlTab[count];
                for (int i = 0; i < count; i++)
                {
                    tabs[i] = new SUITabControlTab(this, i + 1);
                }
                return tabs;
            }
        }
    }
    public class SUITabControlTab
    {
        private SUITabControl tabControl;
        private int index;
        private string name;
        public SUITabControlTab(SUITabControl container, int index)
        {
            tabControl = container;
            this.index = index;
            name = container.Accessible.get_accName(index);
        }
        public int Index
        {
            get
            {
                return index;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }

        public Rectangle Rect
        {
            get
            {
                int x, y, width, height;
                tabControl.Accessible.accLocation(out x, out y, out width, out height, index);
                return new Rectangle(x, y, width, height);
            }
        }
    }
}
