using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIListViewSubitem : SUIListViewItem
    {
        private SUIListViewItem parentItem;
        public SUIListViewSubitem(SUIListView lv, SUIListViewItem _parentItem)
            : base(lv)
        {
            parentItem = _parentItem;
        }

        public SUIListViewSubitem(SUIListView lv, LVITEM item, SUIListViewItem _parentItem)
            : base(lv, item)
        {
            parentItem = _parentItem;
        }

        public SUIListViewItem ParentItem
        {
            get
            {
                return parentItem;
            }
        }

        //One based.
        public int SubitemIndex
        {
            get
            {
                return LVITEM.iSubItem;
            }
        }

        public void Click()
        {
            RECT rect = Rect;
            SUIMouse.MouseClick(ListView, (int)((Rect.left + Rect.right) / 2), (int)((Rect.top + Rect.bottom) / 2));
            
            //some listview require must click on the Text,and in general,text is left align,so add 30 offset as a tempory solution at present.
            SUIMouse.MouseClick(ListView, (int)(Rect.left + 30), (int)((Rect.top + Rect.bottom) / 2));
        }

        public SUIWindow ClickToGetEditControl()
        {
            Click();
            SUISleeper.Sleep(2000);

            RECT rect = Rect;
            int x = ListView.X + rect.left + 5;
            int y = ListView.Y + rect.top + 5;
            IntPtr handle = SUIWinAPIs.WindowFromPoint(new Point(x, y));

            if (handle == null || handle.Equals(IntPtr.Zero) || handle.Equals(ListView.WindowHandle))
                throw new SUIException("No edit control found!");

            SUIWindow window = new SUIWindow(handle);
            while (true)
            {
                if (window.Parent == null)
                    throw new SUIException("No edit control found!");

                if(window.Parent.WindowHandle.Equals(ListView.WindowHandle))
                    break;

                window = window.Parent;
            }

            return window;
        }

        public RECT Rect
        {
            get
            {
                int i = SubitemIndex;
                RECT rect = new RECT();
                rect.top = i;
                rect.left = SUIMessage.LVIR_BOUNDS;
                int processId = 0;
                SUIWinAPIs.GetWindowThreadProcessId(ListView.WindowHandle, ref processId);
                IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

                IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(rect), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
                IntPtr ptrLvi = IntPtr.Zero;
                try
                {
                    ptrLvi = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                    Marshal.StructureToPtr(rect, ptrLvi, false);

                    IntPtr numReaded = new IntPtr();
                    SUIWinAPIs.WriteProcessMemory(processHandle, data, ptrLvi, Marshal.SizeOf(rect), out numReaded);

                    SUIWinAPIs.SendMessage(ListView.WindowHandle, SUIMessage.LVM_GETSUBITEMRECT, new IntPtr(Index), data);

                    SUIWinAPIs.ReadProcessMemory(processHandle, data, ptrLvi, Marshal.SizeOf(rect), out numReaded);

                    rect = (RECT)Marshal.PtrToStructure(ptrLvi, rect.GetType());
                }
                catch (Exception e)
                {
                    throw new SUIException("Error getting rectangle of ListView item!", e);
                }
                finally
                {
                    SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(rect), SUIMessage.MEM_RESERVE);
                    Marshal.FreeHGlobal(ptrLvi);

                    SUIWinAPIs.CloseHandle(processHandle);
                }

                //If this subitem is not the last column.
                if (SubitemIndex != ListView.ColumnCount - 1 && ListView.ColumnCount != 0)
                {
                    RECT nextItemRect = ParentItem.GetSubitem(SubitemIndex + 1).Rect;
                    rect.right = nextItemRect.left;
                }

                return rect;
            }
        }

        public string Text
        {
            get
            {
                int processId = 0;
                SUIWinAPIs.GetWindowThreadProcessId(ListView.WindowHandle, ref processId);
                IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

                IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(lvItem), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
                IntPtr ptrLvi = IntPtr.Zero;
                IntPtr str = IntPtr.Zero;
                const int bufferSize = 512;
                try
                {
                    lvItem.cchTextMax = bufferSize;
                    lvItem.mask = SUIMessage.LVIF_TEXT;
                    lvItem.pszText = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, bufferSize, SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
                    str = Marshal.AllocHGlobal(bufferSize);

                    ptrLvi = Marshal.AllocHGlobal(Marshal.SizeOf(lvItem));
                    Marshal.StructureToPtr(lvItem, ptrLvi, false);
                    IntPtr numReaded = new IntPtr();
                    SUIWinAPIs.WriteProcessMemory(processHandle, data, ptrLvi, Marshal.SizeOf(lvItem), out numReaded);

                    SUIWinAPIs.SendMessage(ListView.WindowHandle, SUIMessage.LVM_GETITEM, IntPtr.Zero, data);

                    SUIWinAPIs.ReadProcessMemory(processHandle, lvItem.pszText, str, bufferSize, out numReaded);

                    string text = Marshal.PtrToStringAuto(str);

                    return text;
                }
                catch (Exception e)
                {
                    throw new SUIException("Error getting text of ListView item!", e);
                }
                finally
                {
                    SUIWinAPIs.VirtualFreeEx(processHandle, lvItem.pszText, bufferSize, SUIMessage.MEM_RESERVE);
                    Marshal.FreeHGlobal(str);
                    Marshal.FreeHGlobal(ptrLvi);

                    SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(lvItem), SUIMessage.MEM_RESERVE);
                    SUIWinAPIs.CloseHandle(processHandle);
                }
            }
            set
            {
                if (value == null)
                    throw new SUIException("Null string object.");

                int processId = 0;
                SUIWinAPIs.GetWindowThreadProcessId(ListView.WindowHandle, ref processId);
                IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

                IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(lvItem), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
                IntPtr ptrLvi = IntPtr.Zero;
                IntPtr str = IntPtr.Zero;
                const int bufferSize = 512;
                try
                {
                    lvItem.cchTextMax = bufferSize;
                    lvItem.mask = SUIMessage.LVIF_TEXT;
                    lvItem.pszText = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, bufferSize, SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
                    str = Marshal.AllocHGlobal(bufferSize);
                    str = Marshal.StringToHGlobalAuto(value);
                    IntPtr numReaded = new IntPtr();
                    SUIWinAPIs.WriteProcessMemory(processHandle, lvItem.pszText, str, bufferSize, out numReaded);

                    ptrLvi = Marshal.AllocHGlobal(Marshal.SizeOf(lvItem));
                    Marshal.StructureToPtr(lvItem, ptrLvi, false);
                    SUIWinAPIs.WriteProcessMemory(processHandle, data, ptrLvi, Marshal.SizeOf(lvItem), out numReaded);

                    SUIWinAPIs.SendMessage(ListView.WindowHandle, SUIMessage.LVM_SETITEMTEXT, new IntPtr(lvItem.iItem), data);
                }
                catch (Exception e)
                {
                    throw new SUIException("Error setting text of ListView item!", e);
                }
                finally
                {
                    SUIWinAPIs.VirtualFreeEx(processHandle, lvItem.pszText, bufferSize, SUIMessage.MEM_RESERVE);
                    Marshal.FreeHGlobal(str);
                    Marshal.FreeHGlobal(ptrLvi);

                    SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(lvItem), SUIMessage.MEM_RESERVE);
                    SUIWinAPIs.CloseHandle(processHandle);
                }
            }
        }
    }
}
