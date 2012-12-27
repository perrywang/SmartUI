using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIRichEdit : SUIWindow
    {
        private SUITextBox suiTextBox = null;
        public SUIRichEdit(SUIWindow sw)
            : base(sw)
        {
            suiTextBox = new SUITextBox(sw);
        }
        public SUIRichEdit(IntPtr winHandle)
            : base(winHandle)
        {
            suiTextBox = new SUITextBox(winHandle);
        }
        public string Text
        {
            get
            {
                return suiTextBox.Text;
            }
            set
            {
                suiTextBox.Text = value;
            }
        }
    }
}
