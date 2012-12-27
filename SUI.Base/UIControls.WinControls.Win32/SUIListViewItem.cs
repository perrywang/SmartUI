using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SUI.Base.Win;
using System.Drawing;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIListViewItem
    {
        private SUIListView listView;
        protected LVITEM lvItem;
        public SUIListViewItem(SUIListView lv)
        {
            listView = lv;
            lvItem = new LVITEM();
        }
        public SUIListViewItem(SUIListView lv, LVITEM item)
        {
            listView = lv;
            lvItem = item;
        }

        public SUIListView ListView
        {
            get
            {
                return listView;
            }
        }

        public LVITEM LVITEM
        {
            get
            {
                return lvItem;
            }
        }

        public bool IsInvalidItem
        {
            get
            {
                return lvItem.iItem == -1;
            }
        }

        //Zero based index.
        public int Index
        {
            get
            {
                return lvItem.iItem;
            }
        }

        //If this item is invisible, we need to scroll to it to show it.
        public void EnsureVisible()
        {
            SUIWinAPIs.SendMessage(ListView.WindowHandle, SUIMessage.LVM_ENSUREVISIBLE, Index, 0);
        }

        public void Select()
        {
            SetItemState(SUIMessage.LVIS_SELECTED, SUIMessage.LVIS_SELECTED);
        }

        public bool IsSelected
        {
            get
            {
                int flag = SUIMessage.LVIS_SELECTED;
                return (GetItemState(flag) & flag ) == flag;
            }
        }

        //One based index.
        public SUIListViewSubitem GetSubitem(int subitemIndex)
        {
            if (IsInvalidItem)
                throw new SUIException("Cannot get subitem of an invalid item!");

            if (subitemIndex < 0 || (ListView.ColumnCount > 0 && subitemIndex >= ListView.ColumnCount))
                throw new SUIException("Index is out of range!");

            LVITEM item = new LVITEM();
            item.iItem = Index;
            item.iSubItem = subitemIndex;
            return new SUIListViewSubitem(ListView, item, this);
        }

        public void SetItemState(int value, int flag)
        {
            if (IsInvalidItem)
                throw new SUIException("Cannot set state to an invalid item!");

            int processId = 0;
            SUIWinAPIs.GetWindowThreadProcessId(ListView.WindowHandle, ref processId);
            IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

            IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(lvItem), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
            IntPtr ptrLvi = IntPtr.Zero;
            try
            {
                lvItem.mask = SUIMessage.LVIF_STATE;
                lvItem.state = value;
                lvItem.stateMask = flag;

                ptrLvi = Marshal.AllocHGlobal(Marshal.SizeOf(lvItem));
                Marshal.StructureToPtr(lvItem, ptrLvi, false);
                IntPtr numReaded = new IntPtr();
                SUIWinAPIs.WriteProcessMemory(processHandle, data, ptrLvi, Marshal.SizeOf(lvItem), out numReaded);

                SUIWinAPIs.SendMessage(ListView.WindowHandle, SUIMessage.LVM_SETITEMSTATE, new IntPtr(Index), data);
            }
            catch (Exception e)
            {
                throw new SUIException("Error set state to ListView item!", e);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrLvi);

                SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(lvItem), SUIMessage.MEM_RESERVE);
                SUIWinAPIs.CloseHandle(processHandle);
            }
        }

        public int GetItemState(int flag)
        {
            if (IsInvalidItem)
                throw new SUIException("Cannot get state of an invalid item!");

            int rv = SUIWinAPIs.SendMessage(ListView.WindowHandle, SUIMessage.LVM_GETITEMSTATE, Index, flag).ToInt32();
            return rv;
        }

        public string Text
        {
            get
            {
                if (IsInvalidItem)
                    throw new SUIException("Cannot get text of an invalid item!");

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
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct LVITEM
    {
        public int mask;
        public int iItem;
        public int iSubItem;
        public int state;
        public int stateMask;
        public IntPtr pszText;
        public int cchTextMax;
        public int iImage;
        public int lParam;
        public int iIndent;
    }
}
