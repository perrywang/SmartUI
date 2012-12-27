using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Accessibility;
using SUI.Base.SUIAccessible;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetToolStrip : SUIDotNetControl
    {
        protected SUIAccessibility accessibleObject = null;
        public  SUIDotNetToolStrip(IntPtr hWnd) 
            : base(hWnd)
        {
            accessibleObject = new SUIAccessibility(hWnd, AccType.Client);
        }
        
        public SUIDotNetToolStrip(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetToolStrip(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
        public void Click(int itemIndex)
        {
            Items[itemIndex].Click();
        }
        public int GetItemIndexFromPoint(Point point)
        {
            int indexOfToolItem = 0;
            foreach (SUIDotNetToolStripItem item in Items)
            {
                if (item.rect.Contains(point))
                {
                    return indexOfToolItem;
                }
                else
                    indexOfToolItem++;
            }
            return -1;
        }

        public SUIDotNetToolStripItemList Items
        {
            get
            {
                SUIDotNetToolStripItemList itemList = new SUIDotNetToolStripItemList();
                int itemCount = accessibleObject.ChildCount;
                for(int i=1; i <= itemCount; i++)
                {
                    int roleID = accessibleObject.Children[i].RoleID;
                    if (roleID == (int)AccessibleRole.PushButton
                        || roleID == (int)AccessibleRole.ComboBox 
                        || roleID == (int)AccessibleRole.Text
                        || roleID == (int)AccessibleRole.MenuItem)
                    {
                        SUIDotNetToolStripItem item = new SUIDotNetToolStripItem(this, i);
                        itemList.Add(item);
                    }
                }
                return itemList;
            }
        }
        public SUIAccessibility AccessibleObject
        {
            get
            {
                return accessibleObject;
            }
        }
        
    }
    public class SUIDotNetToolStripItemList : ReadOnlyCollectionBase
    {
        public void Add(SUIDotNetToolStripItem toolStripItem)
        {
            this.InnerList.Add(toolStripItem);
        }

        public SUIDotNetToolStripItem this[int index]
        {
            get
            {
                return (SUIDotNetToolStripItem)this.InnerList[index];
            }
        }
    }

    
}
