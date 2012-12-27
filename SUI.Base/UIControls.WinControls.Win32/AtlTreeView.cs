using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Collections;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class AtlTreeView : SUIWindow
    {
        public AtlTreeView(IntPtr hWnd)
            : base(hWnd)
        { }

        public AtlTreeView(SUIWindow win)
            : this(win.WindowHandle)
        { }

        public void Select(string treeNodeLabel)
        {
            this.SelectText(treeNodeLabel);
        }

    }
}
