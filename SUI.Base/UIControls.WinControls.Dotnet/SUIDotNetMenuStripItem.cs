using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetMenuStripItem : SUIDotNetToolStripItem
    {
        public SUIDotNetMenuStripItem(SUIDotNetMenuStrip container, int itemIndex)
            : base(container, itemIndex)
        { }
        public string DisplayName
        {
            get
            {
                return accessibleObject.Name;
            }
        }

    }
}
