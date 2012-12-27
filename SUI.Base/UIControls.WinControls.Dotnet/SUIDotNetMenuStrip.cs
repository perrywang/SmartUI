using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetMenuStrip : SUIDotNetToolStrip
    {
        public  SUIDotNetMenuStrip(IntPtr hWnd) 
            : base(hWnd)
        {
        }
        public SUIDotNetMenuStrip(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetMenuStrip(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
        public SUIDotNetContextMenuStrip PopupSubMenu(int clickedIndex)
        {
            this.Click(clickedIndex);
            SUISleeper.Sleep(500);
            return FindContextMenu();
        }
        private SUIDotNetContextMenuStrip FindContextMenu()
        {
            IntPtr topContextMenu = SUIWinAPIs.GetTopWindow(IntPtr.Zero);
            return new SUIDotNetContextMenuStrip(topContextMenu);
        }
    }
}
