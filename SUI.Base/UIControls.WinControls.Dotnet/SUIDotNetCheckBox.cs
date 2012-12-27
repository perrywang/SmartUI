using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.UIControls.WinControls.Win32;
using Microsoft.ManagedSpy;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetCheckBox : SUIDotNetButton
    {
        private SUICheckBox btn = null;
        private ControlProxy proxy = null;
        public  SUIDotNetCheckBox(IntPtr hWnd) 
            : base(hWnd)
        {
            btn = new SUICheckBox(hWnd);
            ControlProxy proxy = ControlProxy.FromHandle(WindowHandle);
        }
        
        public SUIDotNetCheckBox(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetCheckBox(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
        public bool IsChecked
        {
            get
            {
                if (proxy == null)
                    proxy = ControlProxy.FromHandle(WindowHandle);
                return (bool)proxy.GetValue("Checked");
            }
        }

        public void Check()
        {
            if (!IsChecked)
            {
                proxy.SetValue("Checked", true);
            }
        }

        public void Uncheck()
        {
            if (IsChecked)
            {
                proxy.SetValue("Checked", false);
            }
        }
    }
}
