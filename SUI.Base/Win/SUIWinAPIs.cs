using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using SUI.Base.UIControls.WinControls.Win32;
using Accessibility;

namespace SUI.Base.Win
{
    public class SUIWinAPIs
    {
        #region Process
        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int IsIconic(IntPtr hWnd);  
        [DllImport("user32.dll")]
        public static extern IntPtr AttachThreadInput(IntPtr th1, IntPtr th2, int attach);
        [DllImport("user32.dll")]
        public static extern bool IsWindowEnabled(IntPtr hWnd);  
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hwnd, ref int processID);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
      
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThreadId();
        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr HWnd);

        [DllImport("oleacc.dll")]
        public static extern IntPtr AccessibleObjectFromWindow(IntPtr hwnd, int dwId, ref Guid riid, ref IAccessible ppvObject);

        [DllImport("oleacc.dll")]
        public static extern IntPtr AccessibleObjectFromPoint(Point p,ref IAccessible ppvObject, out object pvarchild);


        #endregion

        #region Memory
        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int flFreeType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess( int dwDesiredAccess, int bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] IntPtr buffer, int size, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, int size, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        
        public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] IntPtr buffer, int size, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hObject);

        [DllImport("kernel32")]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappigAttributes, int flProtect, int dwMaximumSizeHigh, int dwMaximumSizeLow, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpName);

        [DllImport("kernel32")]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, int dwNumberOfBytesToMap);

        [DllImport("kernel32", EntryPoint="RtlMoveMemory")]
        public static extern void CopyMemoryRECT(ref RECT destRect, IntPtr sourceAddr, int numberOfBytes);

        [DllImport("kernel32")]
        public static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

        #endregion

        #region Window
        [DllImport("user32.dll")]
        public static extern int RegisterWindowMessage(string str);

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern IntPtr RealChildWindowFromPoint(IntPtr parent,Point pt);
        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr win,out RECT rect);
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int index, int value);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr wnd, uint cmdID);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32")]
        public extern static int EnumWindows(SUI.Base.SUIWindow.EnumSMARTUIWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32")]
        public extern static int EnumChildWindows(IntPtr hWndParent, SUI.Base.SUIWindow.EnumSMARTUIWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string lpClassName, string lpWindowName);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr GetParent(IntPtr HWnd);
        [DllImport("user32")]
        public static extern IntPtr GetAncestor(IntPtr hWnd, int nFlag);
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out Rectangle lpRect);

        [DllImport("user32.dll")]
        public static extern void AdjustWindowRectEx(ref Rectangle lpRect, int dwStyle, bool hasMenu, int dwExStyle);

        [DllImport("user32.dll")]
        public static extern IntPtr BeginDeferWindowPos(int nNumWindows);

        [DllImport("user32.dll")]
        public static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32")]
        public extern static int IsZoomed(IntPtr hwnd);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int PostMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int SendMessage(IntPtr hWnd, int wMsg, int wParam, IntPtr lParam);
        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int SendMessage(IntPtr hWnd, int wMsg, int nMaxCount, [Out]StringBuilder lpString);
        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, string lpstring);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDlgItem(IntPtr hWnd, int dlgItemID);
        [DllImport("user32.dll")]
        public static extern int GetDlgCtrlID(IntPtr hwndCtl);
        [DllImport("user32.dll")]
        public static extern int GetDlgItemText(IntPtr hDlg, int nIDDlgItem, [Out] StringBuilder lpString, int nMaxCount);
        [DllImport ("user32.dll")]
        public static extern bool SetWindowPos (IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        
        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, String lpString);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("User32.dll")]
        public static extern Int32 GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, out Point lpPoint);

        [DllImport("oleacc.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object ObjectFromLresult(IntPtr lResult, [MarshalAs(UnmanagedType.LPStruct)] Guid refiid, IntPtr wParam);
        #endregion

        #region Menu
        [DllImport("user32.dll")]
        public static extern bool IsMenu(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetMenu(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);
        [DllImport("user32.dll")]
        public static extern int GetMenuItemCount(IntPtr hMenu);
        [DllImport("user32.dll")]
        public static extern IntPtr GetMenuItemID(IntPtr hMenu, int nPos);
        [DllImport("user32.dll")]
        public static extern int SetMenuItemBitmaps(IntPtr hMenu, IntPtr nPosition, int wFlags, IntPtr hBitmapUnchecked, IntPtr hBitmapChecked);
        [DllImport("user32.dll")]
        static extern bool GetMenuItemInfo(IntPtr hMenu, int uItem, bool fByPosition, ref MENUITEMINFO lpmii);
        [DllImport("User32.dll")]
        public static extern int GetMenuString(IntPtr hMenu, int uIDItem, StringBuilder lpString, int nMaxCount, uint uFlag);
        #endregion

        #region Caret
        [DllImport("user32.dll")]
        public static extern uint GetCaretBlinkTime();

        [DllImport("user32.dll")]
        public static extern bool SetCaretBlinkTime(uint time);

        [DllImport("user32.dll")]
        public static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowCaret(IntPtr hWnd);
        #endregion

        #region GDI
        [DllImport("GDI32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("GDI32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);


        [DllImport("GDI32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
                                         int nWidth, int nHeight, IntPtr hdcSrc,
                                         int nXSrc, int nYSrc, int dwRop);

        [DllImport("GDI32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("User32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("GDI32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("GDI32.dll")]
        public static extern bool GetTextExtentPoint32(IntPtr hdc, string text, int count, out Size txtSize);
        
#endregion

        #region Keyboard
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void keybd_event(byte vk, ushort scan, int flag, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern byte VkKeyScan(char ch);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern short VkKeyScanEx(char ch, HandleRef keyboardLayout);

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendInput(int inputCount, INPUT[] inputs, int inputSize);

        #endregion

        #region Mouse
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        #endregion

        #region Resource
        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, int ResID, int lpType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, string ResID, int lpType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, int dwFlags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string dllName);

        [DllImport("kernel32.dll")]
        public static extern int FreeLibrary(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern int LoadString(IntPtr hInstance, int uID, StringBuilder lpBuffer, int nBufferMax);
        #endregion

        #region Cursor and Icon
        [DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
        public static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll", EntryPoint = "CopyIcon")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll", EntryPoint = "GetIconInfo")]
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);
        #endregion

#region String Format
        [DllImport("msvcrt40.dll")]
        public static extern int sprintf(StringBuilder sb, string format, __arglist);
#endregion

        [STAThread]
        public static Hashtable GetRunningObjectTable()
        {
            Hashtable result = new Hashtable();

            IntPtr numFetched = IntPtr.Zero;
            IRunningObjectTable runningObjectTable;
            IEnumMoniker monikerEnumerator;
            IMoniker[] monikers = new IMoniker[1];

            SUIWinAPIs.GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();

            while (monikerEnumerator.Next(1, monikers, numFetched) == 0)
            {
                IBindCtx ctx;
                SUIWinAPIs.CreateBindCtx(0, out ctx);

                string runningObjectName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectName);

                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);

                result[runningObjectName] = runningObjectVal;
            }

            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public int dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
        [FieldOffset(0)]
        public InputType type;
        [FieldOffset(4)] //*
        public MOUSEINPUT mi;
        [FieldOffset(4)] //*
        public KEYBDINPUT ki;
        [FieldOffset(4)] //*
        public HARDWAREINPUT hi;
    }

    public enum InputType
    {
        MOUSE,
        KEYBOARD,
        HARDWARE
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ICONINFO
    {
        public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies 
        public Int32 xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot 
        public Int32 yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot 
        public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon, 
        public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this 
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CURSORINFO
    {
        public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
        public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
        public IntPtr hCursor;          // Handle to the cursor. 
        public Point ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
    }
}
