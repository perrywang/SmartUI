using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SUI.Base.Win;
using System.Drawing;
using SUI.Base.Utility;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIToolbarButton
    {
        private int index;
        public int Index
        {
            get
            {
                return index;
            }
        }

        public SUIToolbarButton(SUIToolbar _toolbar, TBBUTTON _tbButton, int _index)
        {
            toolbar = _toolbar;
            tbButton = _tbButton;
            index = _index;
        }

        private SUIToolbar toolbar;
        public SUIToolbar Toolbar
        {
            get
            {
                return toolbar;
            }
        }       

        private TBBUTTON tbButton;
        public TBBUTTON TBBUTTON
        {
            get
            {
                return tbButton;
            }
        }

        private string text;
        public string Text
        {
            get
            {
                if (text == null)
                {
                    //int processId = 0;
                    //SUIWinAPIs.GetWindowThreadProcessId(toolbar.WindowHandle, ref processId);
                    //IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

                    //IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(typeof(Rectangle)), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);

                    //Rectangle rec = new Rectangle();
                    //IntPtr ptr = IntPtr.Zero;
                    //try
                    //{
                    //    ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Rectangle)));
                    //    int length = SUIWinAPIs.SendMessage(toolbar.WindowHandle, SUIMessage.TB_GETITEMRECT, index, IntPtr.Zero);
                    //    System.Windows.Forms.MessageBox.Show(length.ToString());

                    //    IntPtr numReaded = new IntPtr();
                    //    SUIWinAPIs.ReadProcessMemory(processHandle, data, ptr, Marshal.SizeOf(typeof(Rectangle)), out numReaded);

                    //    rec = (Rectangle)Marshal.PtrToStructure(ptr, typeof(Rectangle));
                    //}
                    //catch (Exception e)
                    //{
                    //    throw new SUIException("Error getting toolbar button information!", e);
                    //}
                    //finally
                    //{
                    //    Marshal.FreeHGlobal(ptr);
                    //}
                    //SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(typeof(Rectangle)), SUIMessage.MEM_RESERVE);
                    //SUIWinAPIs.CloseHandle(processHandle);
                    //StringBuilder sb = new StringBuilder(256);
                    
                    
                    
                    //int length = SUIWinAPIs.SendMessage(toolbar.WindowHandle, SUIMessage.TB_GETBUTTONTEXT, tbButton.idCommand, IntPtr.Zero);
                    //System.Windows.Forms.MessageBox.Show(length.ToString());
                    text = "";
                }
                return text;
            }
        }

        protected bool HasStyle(int styleID)
        {
            return (TBBUTTON.fsStyle & styleID) == styleID;
        }

        protected bool HasState(int stateID)
        {
            return (TBBUTTON.fsState & stateID) == stateID;
        }

        public bool IsSeparator
        {
            get
            {
                return HasStyle(SUIMessage.TBSTYLE_SEP);
            }
        }

        public bool IsDropDownList
        {
            get
            {
                return HasStyle(SUIMessage.TBSTYLE_DROPDOWN);
            }
        }

        public bool IsCheckBox
        {
            get
            {
                return HasStyle(SUIMessage.TBSTYLE_CHECK);
            }
        }

        public bool IsChecked
        {
            get
            {
                return HasState(SUIMessage.TBSTATE_CHECKED);
            }
        }

        //There are 10 pixels X_offset if the toolbar is docking.
        public static int X_OFFSET = 10;
        // I use TB_GETITEMRECT to get rectangle of toolbar button,
        public virtual Rectangle Rectangle
        {
            get
            {
                int processId = 0;
                SUIWinAPIs.GetWindowThreadProcessId(toolbar.WindowHandle, ref processId);
                IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

                IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(typeof(Rectangle)), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);

                Rectangle rec = new Rectangle();
                IntPtr ptr = IntPtr.Zero;
                try
                {
                    ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Rectangle)));
                    SUIWinAPIs.SendMessage(toolbar.WindowHandle, SUIMessage.TB_GETITEMRECT, index, data);

                    IntPtr numReaded = new IntPtr();
                    SUIWinAPIs.ReadProcessMemory(processHandle, data, ptr, Marshal.SizeOf(typeof(Rectangle)), out numReaded);

                    rec = (Rectangle)Marshal.PtrToStructure(ptr, typeof(Rectangle));
                }
                catch (Exception e)
                {
                    throw new SUIException("Error getting toolbar button information!", e);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
                SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(typeof(Rectangle)), SUIMessage.MEM_RESERVE);
                SUIWinAPIs.CloseHandle(processHandle);

                if (Toolbar.IsDocking)
                {
                    rec.X += X_OFFSET;
                    rec.Width += X_OFFSET;
                }

                return rec;
            }
        }

        // I use TB_GETRECT to get rectangle of toolbar button,
        //public virtual Rectangle Rect
        //{
        //    get
        //    {
        //        int processId = 0;
        //        SUIWinAPIs.GetWindowThreadProcessId(toolbar.WindowHandle, ref processId);
        //        IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

        //        IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(typeof(Rectangle)), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);

        //        Rectangle rec = new Rectangle();
        //        IntPtr ptr = IntPtr.Zero;
        //        try
        //        {
        //            ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Rectangle)));
        //            SUIWinAPIs.SendMessage(toolbar.WindowHandle, SUIMessage.TB_GETRECT, TBBUTTON.idCommand, data);

        //            IntPtr numReaded = new IntPtr();
        //            SUIWinAPIs.ReadProcessMemory(processHandle, data, ptr, Marshal.SizeOf(typeof(Rectangle)), out numReaded);

        //            rec = (Rectangle)Marshal.PtrToStructure(ptr, typeof(Rectangle));
        //        }
        //        catch (Exception e)
        //        {
        //            throw new SUIException("Error getting toolbar button information!", e);
        //        }
        //        finally
        //        {
        //            Marshal.FreeHGlobal(ptr);
        //        }
        //        SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(typeof(Rectangle)), SUIMessage.MEM_RESERVE);
        //        SUIWinAPIs.CloseHandle(processHandle);

        //        return rec;
        //    }
        //}

        // UI Click
        public void Click()
        {
            SUIMouse.MouseClick(new SUIWindow(toolbar), (int)((Rectangle.X + Rectangle.Width) / 2), (int)((Rectangle.Y + Rectangle.Height) / 2));
            SUISleeper.Sleep(1000);
        }

        public void RClick()
        {
            SUIMouse.MouseRightClick(new SUIWindow(toolbar), (int)((Rectangle.X + Rectangle.Width) / 2), (int)((Rectangle.Y + Rectangle.Height) / 2));
            SUISleeper.Sleep(1000);
        }

        public void MouseMoveTo()
        {
            SUIMouse.MouseMove(new SUIWindow(toolbar), (int)((Rectangle.X + Rectangle.Width) / 2), (int)((Rectangle.Y + Rectangle.Height) / 2));
            SUISleeper.Sleep(1000);
        }

        //Send Command
        public void Execute()
        {
            //Here, we use PostMessage instead of SendMessage to avoid current process blocked.
            SUIWinAPIs.PostMessage(toolbar.WindowHandle, SUIMessage.WM_COMMAND, TBBUTTON.idCommand, 0);
            SUISleeper.Sleep(1000);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    public struct TBBUTTON
    {
        public int iBitmap;
        public int idCommand;
        public byte fsState;
        public byte fsStyle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] bReserved;
        public UInt32 dwData;
        public Int32 iString;
    } 
}
