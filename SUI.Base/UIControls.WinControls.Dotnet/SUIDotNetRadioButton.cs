using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.UIControls.WinControls.Win32;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetRadioButton : SUIDotNetButton
    {
        private SUIRadioButton btn = null;
        public  SUIDotNetRadioButton(IntPtr hWnd) 
            : base(hWnd)
        {
            btn = new SUIRadioButton(hWnd);
        }
        
        public SUIDotNetRadioButton(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetRadioButton(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
        
        public bool IsChecked
        {
            get
            {
                return btn.IsChecked;
            }
            
        }

        public void Check()
        {
            btn.Check();
        }

        public void Uncheck()
        {
            btn.Uncheck();
        }
    }
}
