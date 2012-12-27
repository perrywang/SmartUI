using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Runtime.InteropServices;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIListViewHeader : SUIWindow
    {
        public SUIListViewHeader(IntPtr hWnd)
            : base(hWnd)
        { }

        public SUIListViewHeader(SUIWindow win)
            : base(win)
        { }

        public int Count
        {
            get
            {
                return SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.HDM_GETITEMCOUNT, 0, 0).ToInt32();
            }
        }

        //Zero based index
        public RECT GetItemRect(int index)
        {
            RECT rect = new RECT();
            int processId = 0;
            SUIWinAPIs.GetWindowThreadProcessId(WindowHandle, ref processId);
            IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

            IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(rect), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
            IntPtr ptrLvi = IntPtr.Zero;
            try
            {
                ptrLvi = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                Marshal.StructureToPtr(rect, ptrLvi, false);

                IntPtr numReaded = new IntPtr();
                SUIWinAPIs.WriteProcessMemory(processHandle, data, ptrLvi, Marshal.SizeOf(rect), out numReaded);

                SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.HDM_GETITEMRECT, new IntPtr(index), data);

                SUIWinAPIs.ReadProcessMemory(processHandle, data, ptrLvi, Marshal.SizeOf(rect), out numReaded);

                rect = (RECT)Marshal.PtrToStructure(ptrLvi, rect.GetType());
            }
            catch (Exception e)
            {
                throw new SUIException("Error getting rectangle of header item!", e);
            }
            finally
            {
                SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(rect), SUIMessage.MEM_RESERVE);
                Marshal.FreeHGlobal(ptrLvi);

                SUIWinAPIs.CloseHandle(processHandle);
            }
            return rect;
        }

        public void ClickItem(int index)
        {
            RECT itemRect = GetItemRect(index);
            SUIMouse.MouseClick(this, (int)((itemRect.left + itemRect.right) / 2), (int)((itemRect.top + itemRect.bottom) / 2));
        }
    }
}
