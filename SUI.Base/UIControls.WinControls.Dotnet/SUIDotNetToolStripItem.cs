using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SUI.Base.SUIAccessible;
using System.Windows.Forms;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetToolStripItem
    {
        protected SUIDotNetToolStrip container = null;
        protected int itemIndex = -1;
        protected SUIAccessibility accessibleObject = null;
        public SUIDotNetToolStripItem(SUIDotNetToolStrip container, int index)
        {
            this.container = container;
            itemIndex = index;
            accessibleObject = container.AccessibleObject.Children[index];
        }
        public Rectangle rect
        {
            get
            {
                return new Rectangle(accessibleObject.Left, accessibleObject.Top, accessibleObject.Width, accessibleObject.Height);
            }
        }
        public int Index
        {
            get
            {
                return itemIndex;
            }
        }
        public bool IsComboBox
        {
            get
            {
                return accessibleObject.RoleID == (int)AccessibleRole.ComboBox;
            }
        }
        public bool IsTextBox
        {
            get
            {
                return accessibleObject.RoleID == (int)AccessibleRole.Text;
            }
        }
        public bool IsMenuItem
        {
            get
            {
                return accessibleObject.RoleID == (int)AccessibleRole.MenuItem;
            }
        }
        public void Click()
        {
            accessibleObject.Click();
        }
    }
}
