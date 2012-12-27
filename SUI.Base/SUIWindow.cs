using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using SUI.Base.Win;
using System.Collections;
using System.Diagnostics;
using SUI.Base.SUIExceptions;
using SUI.Base.UIControls.WinControls.Win32;
using SUI.Base.UIControls.WinControls.Dotnet;
using SUI.Base.Utility;
using System.Text.RegularExpressions;
using SUI.Base.SUIAccessible;

namespace SUI.Base
{

    public class SUIWindow
    {
        #region private

        // used to enumerate child objects or top windows
        public delegate int EnumSMARTUIWindowsProc(IntPtr hwnd, int lParam);

        // collection windows
        private SUIWindowCollection items = null;

        // pointer to actual window
        private IntPtr hwnd = IntPtr.Zero;

        // max size for string builder for unmanaged calls
        private int sbmax = 256;

        private Process owningProcess = null;

        private IntPtr threadId = IntPtr.Zero;
        #endregion

        #region public
        public bool NullWindow
        {
            get
            {
                return WindowHandle == IntPtr.Zero;
            }
        }

        // maximize window
        public bool Maximized
        {
            get
            {
                return (SUIWinAPIs.IsZoomed(this.WindowHandle) == 1);
            }
            set
            {
                if (!Maximized)
                    SUIWinAPIs.SendMessage(this.hwnd, SUIMessage.WM_SYSCOMMAND, (IntPtr)SUIMessage.SC_MAXIMIZE, IntPtr.Zero);
            }
        }
        public void ClickWindow(int x, int y)
        {
            SUIMouse.MouseClick(this, x, y);
        }
        // minimize window

        public void DoubleClick(int x, int y)
        {
            SUIMouse.MouseDoubleClick(this,x,y);
        }
        public bool Minimized
        {
            get
            {
                return (SUIWinAPIs.IsIconic(this.WindowHandle) == 1);
            }
            set
            {
                if (!Minimized)
                    SUIWinAPIs.SendMessage(this.hwnd, SUIMessage.WM_SYSCOMMAND, (IntPtr)SUIMessage.SC_MINIMIZE, IntPtr.Zero);
            }
        }
        public bool IsManaged
        {
            get
            {
                return SUIUtil.IsManagedProcess(OwningProcess);
            }
        }
        public void SelectText(string text, int index)
        {
            Graphics g = Graphics.FromHwnd(WindowHandle);
            IntPtr hdc = g.GetHdc();
            Size size = new Size();
            SUIWinAPIs.GetTextExtentPoint32(hdc,text,text.Length,out size);
            g.ReleaseHdc();
            Point p = SUIAccessibility.GetPositionFromTextIndex(this.WindowHandle, text, index);
            if (p.X < 0 || p.Y < 0)
            {
                SUISleeper.Sleep(2000);
                p = SUIAccessibility.GetPositionFromTextIndex(this.WindowHandle, text, index);
            }
            if (p.X > 0 && p.Y > 0)
                SUIMouse.MouseClick(this, p.X + size.Width / 2, p.Y + size.Height / 2);
            else
                throw new Exception("Can not locate expected string " +text + ".");
        }

        public bool ExistText(string text)
        {
            try
            {
                SelectText(text);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void DoubleClick(string text)
        {
            Graphics g = Graphics.FromHwnd(WindowHandle);
            IntPtr hdc = g.GetHdc();
            Size size = new Size();
            SUIWinAPIs.GetTextExtentPoint32(hdc, text, text.Length, out size);
            g.ReleaseHdc();
            Point p = SUIAccessibility.GetPositionFromTextIndex(this.WindowHandle, text, 0);
            if (p.X < 0 || p.Y < 0)
            {
                SUISleeper.Sleep(2000);
                p = SUIAccessibility.GetPositionFromTextIndex(this.WindowHandle, text, 0);
            }
            if (p.X > 0 && p.Y > 0)
                ClickWindow(p.X+size.Width/2,p.Y + 2,3);
            else
                throw new Exception("Can not locate expected string " + text + ".");
        }

        public Point GetTextPosition(string text)
        {
            return GetTextPosition(text,0);
        }

        public Point GetTextPosition(string text,int index)
        {
            Graphics g = Graphics.FromHwnd(WindowHandle);
            IntPtr hdc = g.GetHdc();
            Size size = new Size();
            SUIWinAPIs.GetTextExtentPoint32(hdc, text, text.Length, out size);
            g.ReleaseHdc();
            Point p = SUIAccessibility.GetPositionFromTextIndex(this.WindowHandle, text, index);
            if (p.X < 0 || p.Y < 0)
            {
                SUISleeper.Sleep(2000);
                p = SUIAccessibility.GetPositionFromTextIndex(this.WindowHandle, text, index);
            }
            if (p.X > 0 && p.Y > 0)
                return p;
            else
                throw new Exception("Can not locate expected string " + text + ".");
        }

        public void SelectText(string text)
        {
            SelectText(text, 0);
        }


        public void ClickWindow(int x, int y, int nFlags)
        {
            this.Focus();
            SUISleeper.Sleep(2000);
            SUIMouse.MouseClick(this, x, y, nFlags);
        }

        public void Focus()
        {
            //SUIWindow.SetForegroundWindow(this);
            IntPtr currentId = SUIWinAPIs.GetCurrentThreadId();
            int processid = 0;
            IntPtr attachId = SUIWinAPIs.GetWindowThreadProcessId(WindowHandle, ref processid);
            SUIWinAPIs.AttachThreadInput(currentId, attachId, 1);
            SUIWinAPIs.SetFocus(WindowHandle);
        }

        public IntPtr ThreadId
        {
            get
            {
                if (threadId.Equals(IntPtr.Zero))
                {
                    int PID = 0;
                    threadId = SUIWinAPIs.GetWindowThreadProcessId(hwnd, ref PID);
                    owningProcess = Process.GetProcessById(PID);
                }
                return threadId;
            }
        }

        public Process OwningProcess
        {
            get
            {
                if (owningProcess == null)
                {
                    int PID = 0;
                    this.threadId = SUIWinAPIs.GetWindowThreadProcessId(hwnd, ref PID);
                    owningProcess = Process.GetProcessById(PID);
                }
                return owningProcess;
            }
        }
        
        public int WindowStyle
        {
            get
            {
                return SUIWinAPIs.GetWindowLong(this.WindowHandle, SUIMessage.GWL_STYLE);
            }
        }

        public int WindowExStyle
        {
            get
            {
                return SUIWinAPIs.GetWindowLong(this.WindowHandle, SUIMessage.GWL_EXSTYLE);
            }
        }

        public int WindowID
        {
            get
            {
                return SUIWinAPIs.GetWindowLong(this.WindowHandle, SUIMessage.GWL_ID);
            }
        }
        public SUIWindow NextSibling
        {
            get
            {
                IntPtr handle = SUIWinAPIs.GetWindow(this.WindowHandle,2);
                if (handle.Equals(IntPtr.Zero))
                {
                    return null;
                }
                SUIWindow win = new SUIWindow(handle);
                // If invisible, we will ignor it.
                if (!SUIWinAPIs.IsWindowVisible(handle))
                {
                    win = win.NextSibling;
                }
                return win;
            }
        }

        public SUIWindow PreviousSibling
        {
            get
            {
                IntPtr window = SUIWinAPIs.GetWindow(this.WindowHandle, 3);
                if (window.ToInt64() == 0L)
                {
                    return null;
                }
                return new SUIWindow(window);
            }
        }

        public bool HasWindowStyle(int styleID)
        {
            return (WindowStyle & styleID) == styleID;
        }

        public bool Enabled
        {
            get
            {
                return SUIWinAPIs.IsWindowEnabled(WindowHandle);
            }
        }

        public bool IsDialog
        {
            get
            {
                bool retval = false;

                if (this.ClassName.Equals(SUIDialog.DialogClassName))
                    retval = true;
                if (this.ClassName.Equals("Internet Explorer_TridentDlgFrame"))
                    retval = true;

                return retval;
            }
        }

        public bool IsWinForm
        {
            get
            {
                return this.ClassName.StartsWith("WindowsForms");
            }
        }

        public bool IsButton
        {
            get
            {
                // I find that the GroupBox controls is also with "Button" class name.
                // So here we need to filter our those GroupBox controls.
                return (this.ClassName.Equals(SUIButton.ButtonClassName) && !new SUIButton(this).IsGroupBox);
            }
        }
        public bool IsFormButton
        {
            get
            {
                return false;
            }
        }
        

        public bool IsCenterOfBackground(SUIWindow backGroundWin)
        {
            int backGroundLeft = backGroundWin.X;
            int backGroundTop = backGroundWin.Y;
            int backGroundRight = backGroundWin.Width;
            int backGroundBottom = backGroundWin.Height;
            if (
                     Math.Abs(((backGroundLeft + backGroundRight) / 2 - (this.X + this.Width) / 2)) < 10 &&
                     Math.Abs(((backGroundTop + backGroundBottom) / 2 - (this.Y + this.Height) / 2)) < 10
               )
            {
                return true;
            }
            return false;
        }

        public void Close()
        {
            SUIWinAPIs.SendMessage(this.hwnd, SUIMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            SUIKeyboard.Type(SUIKeyboard.VK.CONTROL);   //control key
        }
        public bool BringToTopMost()
        {
            Rectangle rec = new Rectangle();
            SUIWinAPIs.GetWindowRect(this.hwnd, out rec);
            return SUIWinAPIs.SetWindowPos(this.hwnd, SUIMessage.HWND_TOPMOST, rec.X, rec.Y, rec.Width - rec.X, rec.Height - rec.Y, SUIMessage.SWP_NOMOVE);
        }

        public void ShowShortCutIndicators()
        {
            //Only if we are working on a dialog, indicators are supposed to display.
            if (IsDialog)
            {
                SUIKeyboard.Type(SUIKeyboard.VK.MENU);
                //Type "Alt" key for twice to ensure that we could see indicators.
                SUIKeyboard.Type(SUIKeyboard.VK.MENU);

                SUISleeper.Sleep(1000);
            }
        }

        // set SMARTUIWindow in foreground
        public static void SetForegroundWindow(SUIWindow sui)
        {
            SUIWinAPIs.SetForegroundWindow(sui.WindowHandle);
        }

        public void Activate()
        {
            SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.WM_ACTIVATE, 1, 0);
            //SUIWindow.SetForegroundWindow(this);
        }

        // return all top windows
        public SUIWindowCollection Items
        {
            get
            {
                this.items = new SUIWindowCollection();
                SUIWinAPIs.EnumWindows(new EnumSMARTUIWindowsProc(this.WindowEnum), 0);
                return this.items;
            }
        }

        public static void MinimizeAllWindow(SUIWindow exception)
        {
            SUIWindowCollection windows = SUIWindow.DesktopWindow.Items;
            foreach (SUIWindow win in windows)
            {
                if (exception != null)
                {
                    // If current window is the exception window or child window of the exception window.
                    SUIWindow parent = win.Parent;
                    if (win.WindowHandle.Equals(exception.WindowHandle) || (parent != null && parent.WindowHandle.Equals(exception.WindowHandle)))
                    {
                        continue;
                    }
                }
                win.Minimized = true;
            }
        }

        // return  children
        public SUIWindowCollection Children
        {
            get
            {
                this.items = new SUIWindowCollection();
                SUIWinAPIs.EnumChildWindows(this.hwnd, new EnumSMARTUIWindowsProc(this.WindowEnum), 0);
                return this.items;
            }
        }

        public SUIWindow Parent
        {
            get
            {
                SUIWindow sui = null;
                IntPtr ptr = SUIWinAPIs.GetParent(this.hwnd);
                if (ptr.Equals(IntPtr.Zero))
                {
                    return null;
                }
                sui = new SUIWindow(ptr);
                return sui;
            }
        }

        public SUIWindow Ancestor
        {
            get
            {
                SUIWindow sui = null;
                IntPtr ptr = SUIWinAPIs.GetAncestor(this.hwnd,1);
                if (ptr.Equals(IntPtr.Zero))
                {
                    return null;
                }
                sui = new SUIWindow(ptr);
                return sui;
            }
        }

        public void CloseChildModalDialog()
        {
            SUIWinAPIs.EnumWindows(new EnumSMARTUIWindowsProc(this.CloseDialog), 0);
        }

        private int CloseDialog(IntPtr hwnd, int lParam)
        {
            try
            {
                if (hwnd != null)
                {
                    SUIWindow child = new SUIWindow(hwnd);
                    SUIWindow parent = child.Parent;
                    if (parent != null)
                    {
                        if ((child.IsDialog || child.IsWinForm) && (parent.WindowHandle.Equals(this.hwnd)))
                        {
                            child.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return 1;
        }

		public SUIScrollBar VScrollBar
		{
			get
			{
				return new SUIScrollBar(WindowHandle);
			}
		}

		public SUIScrollBar HScrollBar
		{
			get
			{
				return new SUIScrollBar(WindowHandle, 1);
			}
		}

        public Point GetPositionFromText(string text)
        {
            return SUIAccessibility.GetPositionFromText(WindowHandle,text);
        }
        
        public SUIWindow FindChildWindowByText(string winText)
        {
            return SUIWindow.FindSMARTUIWindow(this, null, winText);
        }

        public void Maxmize()
        {
            if (!this.Maximized)
                Maximized = true;
        }

        public void Click(string rowName, string columnName)
        {
            Point p1 = SUIAccessibility.GetPositionFromText(WindowHandle, rowName);
            SUISleeper.Sleep(2000);
            if (p1.X < 0 || p1.Y < 0)
            {
                throw new Exception("can not locate string" + rowName);
            }
            Point p2 = SUIAccessibility.GetPositionFromText(WindowHandle, columnName);
            if (p2.X < 0 || p2.Y < 0)
            {
                throw new Exception("can not locate string" + columnName);
            }
            ClickWindow(p2.X,p1.Y+3);
        }

        public void DoubleClick(string rowName, string columnName)
        {
            Point p1 = SUIAccessibility.GetPositionFromText(WindowHandle, rowName);
            SUISleeper.Sleep(2000);
            if (p1.X < 0 || p1.Y < 0)
            {
                throw new Exception("can not locate string" + rowName);
            }
            Point p2 = SUIAccessibility.GetPositionFromText(WindowHandle, columnName);
            if (p2.X < 0 || p2.Y < 0)
            {
                throw new Exception("can not locate string" + columnName);
            }
            ClickWindow(p2.X, p1.Y + 3,3);
        }



        public void Resize(int x, int y, int width, int height)
        {
            Rectangle rect = new Rectangle(x, y, x + width, y + height);
            SUIWinAPIs.AdjustWindowRectEx(ref rect, WindowStyle, (SUIWinAPIs.GetMenu(WindowHandle) != null), WindowExStyle);

            IntPtr hDWP = SUIWinAPIs.BeginDeferWindowPos(1);
            SUIWinAPIs.DeferWindowPos(hDWP, WindowHandle, SUIMessage.HWND_TOP, x, y, width, height, SUIMessage.SWP_SHOWWINDOW);
            SUIWinAPIs.EndDeferWindowPos(hDWP);
        }

        

        public void Resize(int width, int height)
        {
            Rectangle r = new Rectangle();
            SUIWinAPIs.GetWindowRect(this.hwnd, out r);
            Resize(r.X, r.Y, width, height);
        }

        public void MoveWindow(int x, int y)
        {
            Rectangle r = new Rectangle();
            SUIWinAPIs.GetWindowRect(this.hwnd, out r);
            Resize(x, y, r.Width - r.X, r.Height - r.Y);
        }

        public SUIWindow FindChildWindow(string className, string winText)
        {
            try
            {
                return SUIWindow.FindSMARTUIWindow(this, className, winText);
            }
            catch
            {
                if (className.StartsWith("BCGP"))
                {
                    string optionname = className.Replace("BCGP", "BCG");
                    return SUIWindow.FindSMARTUIWindow(this, optionname, winText);
                }
                else
                    return null;
            }
            
        }

        public SUIWindow FindChildWindow(SUIWindow childAfter, string className, string winText)
        {
            return SUIWindow.FindSMARTUIWindow(this, childAfter, className, winText);
        }

        public static int DefaultTimeout = 30; //unit is second
        public static SUIWindow WaitingForWindow(string caption)
        {
            return WaitingForWindow(caption, DefaultTimeout);
        }

        // Add this method for those situation where we need longer timeout to wait for window!
        public static SUIWindow WaitingForWindow(string caption, int timeout)
        {
            SUIWindow waitingForwin = null;
            try
            {
                while (waitingForwin == null && timeout-- > 0)
                {
                    SUISleeper.Sleep(1000);
                    waitingForwin = DesktopWindow.FindChildWindowByText(caption);
                }
            }
            catch (Exception e)
            {
                throw new SUIGetWindowException(e);
            }

            return waitingForwin;
        }

        public static SUIWindow WaitingForWindow(string matchCaption, bool isRegularExpression)
        {
            return WaitingForWindow(matchCaption, isRegularExpression, DefaultTimeout);
        }

        public static SUIWindow WaitingForWindow(string matchCaption, bool isRegularExpression, int timeout)
        {
            if (!isRegularExpression)
                return WaitingForWindow(matchCaption, timeout);

            SUIWindow waitingForwin = null;
            Regex reg = new Regex(matchCaption);
            while (waitingForwin == null && timeout-- > 0)
            {
                SUISleeper.Sleep(1000);
                if(matchCaption.Equals(string.Empty))
				{
				     waitingForwin = DesktopWindow.FindChildWindowByText(matchCaption);
					 SUIWindow temp = SUIWindow.GetForegroundWindow();
					 if(!waitingForwin.WindowHandle.Equals(temp.WindowHandle))
					 {
					     waitingForwin = temp;
					 }
				}
                   
                else
                {
                    foreach (SUIWindow win in DesktopWindow.Items)
                    {
                        if (SUIWinAPIs.IsWindowVisible(win.WindowHandle) && reg.IsMatch(win.WindowText))
                        {
                            waitingForwin = win;
                            break;
                        }
                    }
                }
            }
            return waitingForwin;
        }
        
        //#warning may change to static
        /*   // get all childwindows of a window 
           public static SMARTUIWindowCollection GetChildWindows(SMARTUIWindow sui)
           {
              this.items = new SMARTUIWindowCollection();
              SUIWinAPIs.EnumChildWindows(sui.hwnd, new EnumSMARTUIWindowsProc(this.WindowEnum), 0);
           }*/

        #region EnumWindows callback

        // The enum Windows callback.
        private int WindowEnum(IntPtr hWnd, int lParam)
        {
            if (this.OnWindowEnum(hWnd))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        // used to continue enumeration
        protected virtual bool OnWindowEnum(IntPtr hWnd)
        {
            if (SUIWinAPIs.IsWindowVisible(hWnd))
            {
                items.Add(hWnd);
            }
            return true;
        }

        // get desktop SMARTUIWindow
        public static SUIWindow DesktopWindow
        {
            get
            {
                SUIWindow sui = null;
                IntPtr ptr = SUIWinAPIs.GetDesktopWindow();
                if (ptr.Equals(IntPtr.Zero))
                {
                    return null;
                }
                sui = new SUIWindow(ptr);
                return sui;
            }
        }
        public bool IsCenterOfScreen
        {
            get
            {
                int x1 = this.X;
                int y1 = this.Y;
                int x2 = this.Width;
                int y2 = this.Height;
                if (
                         Math.Abs(((x1 + x2) / 2 - (SUIWindow.DesktopWindow.X + SUIWindow.DesktopWindow.Width) / 2)) < 50 &&
                         Math.Abs(((y1 + y2) / 2 - (SUIWindow.DesktopWindow.Y + SUIWindow.DesktopWindow.Height) / 2)) < 50
                   )
                {
                    return true;
                }
                return false;
            }
        }
        public static SUIWindow GetForegroundWindow()
        {
            SUIWindow sui = null;
            IntPtr ptr = SUIWinAPIs.GetForegroundWindow();
            if (ptr.Equals(IntPtr.Zero))
            {
                return null;
            }
            sui = new SUIWindow(ptr);
            return sui;
        }

        // find window Note: took out sibling, may need to add a function for this.
        public static SUIWindow FindSMARTUIWindow(SUIWindow parent, string classname, string windowtext)
        {
            SUIWindow sui = null;
            IntPtr ptr = SUIWinAPIs.FindWindowEx(parent.hwnd, IntPtr.Zero, classname, windowtext);
            if (!ptr.Equals(IntPtr.Zero))
            {
                sui = new SUIWindow(ptr);
                //With below statement, we could avoid the invisible windows.
                if (!SUIWinAPIs.IsWindowVisible(ptr))
                {
                    sui = FindSMARTUIWindow(parent, sui, classname, windowtext);
                }
            }
            return sui;
        }

        public static SUIWindow FindSMARTUIWindow(SUIWindow parent, SUIWindow childAfter, string classname, string windowtext)
        {
            if (childAfter == null)
                return FindSMARTUIWindow(parent, classname, windowtext);

            SUIWindow sui = null;
            IntPtr ptr = SUIWinAPIs.FindWindowEx(parent.hwnd, childAfter.hwnd, classname, windowtext);
            if (!ptr.Equals(IntPtr.Zero))
            {
                sui = new SUIWindow(ptr);
                //With below statement, we could avoid the invisible windows.
                if (!SUIWinAPIs.IsWindowVisible(ptr))
                {
                    sui = FindSMARTUIWindow(parent, sui, classname, windowtext);
                }
            }
            return sui;
        }

        public SUIWindow(SUIWindow win)
        {
            if (win == null || win.WindowHandle == null || win.WindowHandle.Equals(IntPtr.Zero))
                throw new SUINullWindowException();//Exception(SUIBaseErrorMessages.Window_CannotInitializeWindow);
            this.hwnd = win.WindowHandle;
        }

        public SUIWindow(IntPtr hWnd)
        {
            if (hWnd == null || hWnd.Equals(IntPtr.Zero))
                throw new SUINullWindowException();//Exception(SUIBaseErrorMessages.Window_CannotInitializeWindow);
            this.hwnd = hWnd;
        }

        public void Bind(string xmlWindowName)
        {
            SUIUtil.RuntimeCache.Add(xmlWindowName,this.WindowText);
        }

        public SUIWindow FirstChild
        {
            get
            {
                SUIWindow sui = null;
                IntPtr ptr = SUIWinAPIs.GetWindow(WindowHandle, 5);
                if (IntPtr.Zero.Equals(ptr))
                    return null;
                
                sui = new SUIWindow(ptr);
                // If invisible, we will ignor it.
                if (!SUIWinAPIs.IsWindowVisible(ptr))
                    sui = sui.NextSibling;
                return sui;
            }
        }

        // width
        public int Width
        {
            get
            {
                Rectangle r = new Rectangle();
                SUIWinAPIs.GetWindowRect(this.hwnd, out r);
                return r.Width;
            }
        }

        // height
        public int Height
        {
            get
            {
                Rectangle r = new Rectangle();
                SUIWinAPIs.GetWindowRect(this.hwnd, out r);
                return r.Height;
            }
        }

        // x cord
        public int X
        {
            get
            {
                Rectangle r = new Rectangle();
                SUIWinAPIs.GetWindowRect(this.hwnd, out r);
                return r.X;
            }
        }

        // y cord
        public int Y
        {
            get
            {
                Rectangle r = new Rectangle();
                SUIWinAPIs.GetWindowRect(this.hwnd, out r);
                return r.Y;
            }
        }

        // identifier for window
        public override int GetHashCode()
        {
            return (int)this.hwnd;
        }

        // window handle
        public IntPtr WindowHandle
        {
            get
            {
                return this.hwnd;
            }
        }

        // control id
        public int ControlID
        {
            get
            {
                return SUIWinAPIs.GetDlgCtrlID(WindowHandle);
            }
        }

        //window text
        public string WindowText
        {
            get
            {
                StringBuilder text = new StringBuilder(sbmax);
                SUIWinAPIs.SendMessage(this.WindowHandle, SUIMessage.WM_GETTEXT, text.Capacity, text);
                return text.ToString();
            }
        }

        // window class name
        public string ClassName
        {
            get
            {
                StringBuilder sb = new StringBuilder(sbmax);
                SUIWinAPIs.GetClassName(this.hwnd, sb, sb.Capacity);
                return sb.ToString();
            }
        }
        #endregion
    }

    public class SUIWindowCollection : ReadOnlyCollectionBase
    {
        // add smart ui to collection
        public void Add(IntPtr hwnd)
        {
            SUIWindow sui = new SUIWindow(hwnd);
            this.InnerList.Add(sui);
        }

        public SUIWindow this[int index]
        {
            get
            {
                return (SUIWindow)this.InnerList[index];
            }
        }
    }
}
