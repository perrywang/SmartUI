using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.UIControls.WinControls.Win32;
using SUI.Base.Win;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetComboBox : SUIDotNetControl
    {
        SUIComboBox suiComboBox = null;
        public  SUIDotNetComboBox(IntPtr hWnd) 
            : base(hWnd)
        {
            suiComboBox = new SUIComboBox(hWnd);
        }
        
        public SUIDotNetComboBox(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetComboBox(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
        public int SelectedIndex
        {
            get
            {
                return suiComboBox.SelectedIndex;
            }
            
        }
        public string Text
        {
            get
            {
                return suiComboBox.Text;
            }
            set
            {
                suiComboBox.Text = value;
            }
        }
        public void Select(int index)
        {
            suiComboBox.Select(index);
        }

        public void Set_Text(string text)
        {
            Text = text;
        }

        public string GetDropdownListText(int index)
        {
            return suiComboBox.GetDropdownListText(index);
        }

        public int DropdownListTextCount
        {
            get
            {
                return suiComboBox.DropdownListTextCount;
            }
        }

        public void Select(string str)
        {
            int index = -1;
            for (int i = 0; i < DropdownListTextCount; i++)
            {
                if (GetDropdownListText(i).IndexOf(str) > -1)
                {
                    Select(i);
                    return;
                }
            }

            //Don't match. So we need specify this text if this is a dropdown combobox.
            throw new SUIException("The string you specified is not in dropdown list.");
        }
        
    }
}
