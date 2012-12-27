using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUITextBox : SUIWindow
    {
        public const string ClassName = "Edit";
        public SUITextBox(SUIWindow sw)
            : base(sw)
        { }
        public SUITextBox(IntPtr winHandle)
            : base(winHandle)
        { }
        public const int MaxLength = 256;
       
        public string Text
        {
            get
            {
                StringBuilder text = new StringBuilder(MaxLength);
                int ch = 0;
                bool bPassword = false;
                if ((this.WindowStyle & 0x0020L) == 0x0020)//ES_PASSWORD==0x0020
                {
                    bPassword = true;
                    ch = SUIWinAPIs.SendMessage(this.WindowHandle, SUIMessage.EM_GETPASSWORDCHAR, IntPtr.Zero, IntPtr.Zero);
                    SUIWinAPIs.PostMessage(this.WindowHandle, SUIMessage.EM_SETPASSWORDCHAR, IntPtr.Zero, IntPtr.Zero);
                    SUISleeper.Sleep(100);
                }
                SUIWinAPIs.SendMessage(this.WindowHandle, SUIMessage.WM_GETTEXT, text.Capacity, text);
                if (bPassword)
                {
                    SUIWinAPIs.PostMessage(this.WindowHandle, SUIMessage.EM_SETPASSWORDCHAR, new IntPtr(ch), IntPtr.Zero);
                }
                return text.ToString();
            }
            set
            {
                if (SUIWinAPIs.IsWindowEnabled(this.WindowHandle))
                {
                    this.Activate();
                    SUIWinAPIs.SendMessage(this.WindowHandle, SUIMessage.WM_SETTEXT, 0, string.Empty);
                    SUIKeyboard.Type(value);
                    //SUIWinAPIs.SendMessage(this.WindowHandle,SUIMessage.WM_SETTEXT,0, value);
                    this.Activate();
                }
                else
                {
                    throw new SUIException("Edit Control is not enable!");
                }
            }
        }

        public void TypeText(string str)
        {
            SUIMouse.MouseClick(this, (int)((Width - X) / 2), (int)((Height - Y) / 2));

            //Clear exsting text.
            SUIKeyboard.Press(SUIKeyboard.VK.CONTROL);
            SUIKeyboard.Type(SUIKeyboard.VK.HOME);
            SUIKeyboard.Release(SUIKeyboard.VK.CONTROL);

            int textLeng = Text.Length;
            for (int i = 0; i < textLeng; i++ )
                SUIKeyboard.Type(SUIKeyboard.VK.DELETE);
            
            SUIKeyboard.Type(str);
        }
    }
}
