using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SUI.Base.Win;
using Accessibility;
using System.Drawing;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIMenuItem
    {
        private SUIMenu container;
        private int nIndex;
        public SUIMenu ItemInThat
        {
            get
            {
                return container;
            }
        }
        public IAccessible IA
        {
            get
            {
                return (IAccessible)container.IA.get_accChild(nIndex + 1);
            }
        }
        public SUIMenuItem(SUIMenu container, int nIndex)
        {
            this.container = container;
            this.nIndex = nIndex;
        }
        public void Click()
        {
            int x, y, width, height;
            IA.accLocation(out x, out y, out width, out height, 0);
            SUIMouse.MouseClick(x, y);
        }

        public Rectangle RECT
        {
            get
            {
                int x, y, width, height;
                IA.accLocation(out x, out y, out width, out height, 0);
                return new Rectangle(x, y, width, height);
            } 
        }

        public int Index
        {
            get
            {
                return nIndex;
            }
        }

    }
    public class SUIPopupMenuItem
    {
        private SUIPopupMenu itemInThat;
        private int index;
        //public SUIWindow MenuWindow
        //{
        //    get
        //    {
        //        return itemInThat.PopupWindow;
        //    }
        //}
        public SUIPopupMenuItem(SUIPopupMenu itemInThat, int nIndex)
        {
            this.index = nIndex;
            this.itemInThat = itemInThat;
        }
        public IAccessible IA
        {
            get
            {
                return (IAccessible)itemInThat.IA;
            }
        }
        public void Click()
        {
            int x, y, width, height;
            IA.accLocation(out x, out y, out width, out height, index + 1);
            SUIMouse.MouseClick(x, y);
        }

        public Rectangle RECT
        {
            get
            {
                int x, y, width, height;
                IA.accLocation(out x, out y, out width, out height, index + 1);
                return new Rectangle(x, y, width, height);
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MENUITEMINFO
    {
        public uint cbSize;
        public uint fMask;
        public uint fType;
        public uint fState;
        public int wID;
        public int hSubMenu;
        public int hbmpChecked;
        public int hbmpUnchecked;
        public int dwItemData;
        public string dwTypeData;
        public uint cch;
        public int hbmpItem;
    }
}
