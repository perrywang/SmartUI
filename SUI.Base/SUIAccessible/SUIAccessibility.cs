using System;
using System.Collections.Generic;
using System.Text;
using Accessibility;
using SUI.Base.Win;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SUI.Base.Utility;

namespace SUI.Base.SUIAccessible
{
    public class SUIAccessibility
    {

        [DllImport("SUIGrabWord.dll")]
        private static extern Point GetPointFromTextIndex(IntPtr HWnd, [MarshalAs(UnmanagedType.LPWStr)]string text,int index);

        [DllImport("GetWord.dll")]
        private static extern IntPtr GetTextFromPoint(IntPtr handle, Point p);
        
        public static Point GetPositionFromTextIndex(IntPtr HWnd,string text, int index)
        {
            string processedText = text;
            if (SUIUtil.IsCurrentOSVista)
            {
                SUIWindow win = new SUIWindow(HWnd);
                if (!win.ClassName.Equals("SysListView32"))
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < text.Length; i++)
                    {
                        char temp = (char)(text[i] - 29);
                        builder.Append(temp);
                    }
                    processedText = builder.ToString();
                }
                
            }
            Point result = new Point(-1, -1);
            try
            {
                SUISleeper.Sleep(2000);
                result = GetPointFromTextIndex(HWnd, processedText, index);
            }
            catch
            {
                Debug.WriteLine("GetPositionFromText ERROR.");
            }

            return result;
        }

        public static string GetWord(SUIWindow win, Point p)
        {
            string result = string.Empty;
            try
            {
                IntPtr strPtr = GetTextFromPoint(win.WindowHandle, p);
                result = Marshal.PtrToStringAuto(strPtr);

            }
            catch
            {
                Debug.WriteLine("GetWord ERROR");
            }
            return result;
        }

        public static Point GetPositionFromTextIndex(SUIWindow win,string text, int index)
        {
            return GetPositionFromTextIndex(win.WindowHandle,text,index);
        }
        
        public static Point GetPositionFromText(IntPtr HWnd,string text)
        {
            return GetPositionFromTextIndex(HWnd, text,0);
        }
       

        public static Point GetPositionFromText(SUIWindow window,string text)
        {
            return GetPositionFromText(window.WindowHandle, text);
        }
        
        public static Guid GuidOfIAcc = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");
        private IAccessible IAcc = null;
        private int childID = -1;
        //private SUIWindow suiWindow = null;
        public SUIAccessibility(IAccessible acc)
        {
            IAcc = acc;
        }
        public SUIAccessibility(IntPtr HWnd, AccType type)
        {
            IntPtr Result = SUIWinAPIs.AccessibleObjectFromWindow(HWnd, (int)type, ref GuidOfIAcc, ref IAcc);
            if (!Result.Equals(IntPtr.Zero))
                throw new Exception("Object does not support IAccessible.");
        }
        
        public SUIAccessibility(SUIWindow accWin,AccType type):this(accWin.WindowHandle,type)
        { }
        
        public SUIAccessibility(IntPtr HWnd, int RoleId)
        {
            IntPtr Result = SUIWinAPIs.AccessibleObjectFromWindow(HWnd, RoleId, ref GuidOfIAcc, ref IAcc);
            if (!Result.Equals(IntPtr.Zero))
                throw new Exception("Object does not support IAccessible.");
        }

        public SUIAccessibility(Point p)
        {
            object o = new object();
            IntPtr Result = SUIWinAPIs.AccessibleObjectFromPoint(p, ref IAcc, out o);
            if (!Result.Equals(IntPtr.Zero))
                throw new Exception("Object does not support IAccessible.");
            childID = (int)o;
        }

        public int ChildID
        {
            get
            {
                return childID;
            }
        }
        public int ChildCount
        {
            get
            {
                return IAcc.accChildCount;
            }
        }
        //private SUIAccessibility[] children = null;
        public SUIAccessibility[] Children
        {
            get
            {
                int count = IAcc.accChildCount;
                SUIAccessibility[] children = new SUIAccessibility[count + 1];
                for (int i = 1; i <= count; i++)
                {
                    object child = IAcc.get_accChild(i);
                    if (child is IAccessible)
                    {
                        IAccessible temp = (IAccessible)child;
                        children[i] = new SUIAccessibility(temp);
                    }
                    else
                    {
                        children[i] = null;
                    }
                }
                return children;
            }
        }
        public string Name
        {
            get
            {
                return IAcc.get_accName(0);
            }
        }
        public IAccessible NativeIA
        {
            get
            {
                return IAcc;
            }
        }
        public void Click()
        {
            int x, y, width, height;
            IAcc.accLocation(out x,out y,out width,out height,0);
            SUIMouse.MouseClick(x,y);
        }

        public Rectangle RECT
        {
            get
            {
                int x, y, width, height;
                IAcc.accLocation(out x, out y, out width, out height, 0);
                return new Rectangle(x,y,width,height);
            }
        }

        public bool IsVisible
        {
            get
            {
                return !((((int)IAcc.get_accState(0))&0x8000) == 0x8000);
            }
        }

        private int left = 0;
        private int top = 0;
        private int width = 0;
        private int height = 0;
        public int Left
        {
            get
            {
                IAcc.accLocation(out left, out top, out width, out height, 0);
                return left;
            }
        }
        public int Top
        {
            get
            {
                IAcc.accLocation(out left, out top, out width, out height, 0);
                return top;
            }
        }
        public int Width
        {
            get
            {
                IAcc.accLocation(out left, out top, out width, out height, 0);
                return width;
            }
        }
        public int Height
        {
            get
            {
                IAcc.accLocation(out left, out top, out width, out height, 0);
                return height;
            }
        }
        public int RoleID
        {
            get
            {
                return (int)IAcc.get_accRole(0);
            }
        }
    }
    public enum AccType
    {

        Alert = -10,
        Caret = -8,
        Client = -4,
        Cursor = -9,
        HScroll = -6,
        Menu = -3,
        NativeOM = -16,
        SizeGrip = -7,
        Sound = -11,
        SysMenu = -1,
        TitleBar = -2,
        VScroll = -5,
        Window = 0

    }
}
