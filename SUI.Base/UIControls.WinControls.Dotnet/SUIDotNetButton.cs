using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using SUI.Base.Win;
using SUI.Base.UIControls.WinControls.Win32;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetButton : SUIDotNetControl
    {
        SUIButton suiBtn = null;
        public  SUIDotNetButton(IntPtr hWnd) 
            : base(hWnd)
        {
            suiBtn = new SUIButton(hWnd);
        }
        
        public SUIDotNetButton(SUIWindow sui)
            : this(sui.WindowHandle)
        {
            
        }
        
        public SUIDotNetButton(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }

        public string Text
        {
            get
            {
                return suiBtn.Text;
            }
        }

        public void Click()
        {
            SUIMouse.MouseClick(this, (Width - X) / 2, (Height - Y) / 2);
        }

        public bool IsCheckBox
        {
            get
            {
                return suiBtn.IsCheckBox;
            }
        }

        public bool IsRadioButton
        {
            get
            {
                return suiBtn.IsRadioButton;
            }
        }
        
    }
}
