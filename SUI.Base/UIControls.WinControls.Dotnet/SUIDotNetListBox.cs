using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.UIControls.WinControls.Win32;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetListBox : SUIDotNetControl
    {
        private SUIListBox suiListBox = null;
        public  SUIDotNetListBox(IntPtr hWnd) 
            : base(hWnd)
        {
            suiListBox = new SUIListBox(hWnd);
        }
        
        public SUIDotNetListBox(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetListBox(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }

        public void ClickToSelect(int index)
        {
            suiListBox.ClickToSelect(index);
        }

        public void Select(int index)
        {
            suiListBox.Select(index);
        }

        public int SelectedIndex
        {
            get
            {
                return suiListBox.SelectedIndex;
            }
        }
    }
}
