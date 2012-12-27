using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SUI.Base.Win;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUITreeViewNode
    {
        private SUITreeView treeView;
        private TVITEM tvItem;
        public SUITreeViewNode(SUITreeView tv)
        {
            treeView = tv;
            tvItem = new TVITEM();
        }
        public SUITreeViewNode(SUITreeView tv, TVITEM item)
        {
            treeView = tv;
            tvItem = item;
        }

        public bool IsNullNode
        {
            get
            {
                return tvItem.hItem == IntPtr.Zero;
            }
        }

        public SUITreeView TreeView
        {
            get
            {
                return treeView;
            }
        }

        public TVITEM TVITEM
        {
            get
            {
                return tvItem;
            }
        }

        public IntPtr hItem
        {
            get
            {
                return tvItem.hItem;
            }
            set
            {
                tvItem.hItem = value;
            }
        }

        public int GetNodeState(int flag)
        {
            if (IsNullNode)
                throw new SUIException("Cannot get state of NULL TreeView node!");

            int rv = SUIWinAPIs.SendMessage(TreeView.WindowHandle, SUIMessage.TVM_GETITEMSTATE, hItem, new IntPtr(flag));
            return rv;
        }

        public bool IsSelected
        {
            get
            {
                int flag = SUIMessage.TVIS_SELECTED;
                return (GetNodeState(flag) & flag) == flag;
            }
        }

        public string Text
        {
            get
            {
                if (IsNullNode)
                    throw new SUIException("Cannot get text of NULL TreeView node!");

                int processId = 0;
                SUIWinAPIs.GetWindowThreadProcessId(TreeView.WindowHandle, ref processId);
                IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

                IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(tvItem), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
                IntPtr ptrTvi = IntPtr.Zero;
                IntPtr str = IntPtr.Zero;
                const int bufferSize = 512;
                try
                {
                    tvItem.cchTextMax = bufferSize;
                    tvItem.mask = SUIMessage.TVIF_TEXT;
                    tvItem.pszText = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, bufferSize, SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
                    str = Marshal.AllocHGlobal(bufferSize);

                    ptrTvi = Marshal.AllocHGlobal(Marshal.SizeOf(tvItem));
                    Marshal.StructureToPtr(tvItem, ptrTvi, false);
                    IntPtr numReaded = new IntPtr();
                    SUIWinAPIs.WriteProcessMemory(processHandle, data, ptrTvi, Marshal.SizeOf(tvItem), out numReaded);

                    SUIWinAPIs.SendMessage(TreeView.WindowHandle, SUIMessage.TVM_GETITEM, IntPtr.Zero, data);

                    SUIWinAPIs.ReadProcessMemory(processHandle, tvItem.pszText, str, bufferSize, out numReaded);

                    string text = Marshal.PtrToStringAuto(str);

                    return text;
                }
                catch (Exception e)
                {
                    throw new SUIException("Error getting text of TreeView node!", e);
                }
                finally
                {
                    SUIWinAPIs.VirtualFreeEx(processHandle, tvItem.pszText, bufferSize, SUIMessage.MEM_RESERVE);
                    Marshal.FreeHGlobal(str);
                    Marshal.FreeHGlobal(ptrTvi);

                    SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(tvItem), SUIMessage.MEM_RESERVE);
                    SUIWinAPIs.CloseHandle(processHandle);
                }
            }
        }

        public SUITreeViewNode ParentNode
        {
            get
            {
                if (IsNullNode)
                    throw new SUIException("Cannot get NextSiblingNode of NULL TreeView node!");

                return TreeView.GetNextItem(tvItem.hItem, SUIMessage.TVGN_PARENT);
            }
        }

        public SUITreeViewNode NextSiblingNode
        {
            get
            {
                if (IsNullNode)
                    throw new SUIException("Cannot get NextSiblingNode of NULL TreeView node!");

                return TreeView.GetNextItem(tvItem.hItem, SUIMessage.TVGN_NEXT);
            }
        }

        public SUITreeViewNode FirstChildNode
        {
            get
            {
                if (IsNullNode)
                    throw new SUIException("Cannot get child node of NULL TreeView node!");
                return TreeView.GetNextItem(tvItem.hItem, SUIMessage.TVGN_CHILD);
            }
        }

        public SUITreeViewNode FindChildNode(string text)
        {
            return FindChildNode(text, false);
        }

        public SUITreeViewNode FindChildNode(string text, bool isRegex)
        {
            SUITreeViewNode node = FirstChildNode;
            Regex reg = null;

            if (isRegex)
                reg = new Regex(text);

            while(!node.IsNullNode)
            {
                if (isRegex)
                {
                    if (reg.IsMatch(node.Text))
                    {
                        break;
                    }
                }
                else
                {
                    if (node.Text.Equals(text))
                    {
                        break;
                    }
                }

                node = node.NextSiblingNode;
            }

            return node;
        }

        public SUITreeViewNode FindChildNode(int index)
        {
            SUITreeViewNode node = FirstChildNode;

            while (!node.IsNullNode)
            {
                if (index == 0)
                {
                    break;
                }

                node = node.NextSiblingNode;
                index--;
            }

            return node;
        }

        public void RClick()
        {
            SUIMouse.MouseRightClick(treeView, (int)((Rectangle.X + Rectangle.Width) / 2), (int)((Rectangle.Y + Rectangle.Height) / 2));
        }

        public Rectangle Rectangle
        {
            get
            {
                Rectangle rect = new Rectangle();
                int processId = 0;
                SUIWinAPIs.GetWindowThreadProcessId(TreeView.WindowHandle, ref processId);
                IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

                IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(rect), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
                IntPtr ptrTvi = IntPtr.Zero;
                IntPtr intPtr = IntPtr.Zero;
                try
                {
                    ptrTvi = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                    intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int32)));
                    Marshal.WriteInt32(intPtr, hItem.ToInt32());

                    IntPtr numReaded = new IntPtr();
                    SUIWinAPIs.WriteProcessMemory(processHandle, data, intPtr, Marshal.SizeOf(typeof(Int32)), out numReaded);

                    SUIWinAPIs.SendMessage(TreeView.WindowHandle, SUIMessage.TVM_GETITEMRECT, new IntPtr(1), data);

                    SUIWinAPIs.ReadProcessMemory(processHandle, data, ptrTvi, Marshal.SizeOf(rect), out numReaded);

                    rect = (Rectangle)Marshal.PtrToStructure(ptrTvi, rect.GetType());
                }
                catch (Exception e)
                {
                    throw new SUIException("Error getting rectangle of TreeView node!", e);
                }
                finally
                {
                    SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(rect), SUIMessage.MEM_RESERVE);
                    Marshal.FreeHGlobal(ptrTvi);
                    Marshal.FreeHGlobal(intPtr);

                    SUIWinAPIs.CloseHandle(processHandle);
                }

                return rect;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
    public struct TVITEM
    {
        public int mask;
        public IntPtr hItem;
        public int state;
        public int stateMask;
        public IntPtr pszText;
        public int cchTextMax;
        public int iImage;
        public int iSelectedImage;
        public int cChildren;
        public IntPtr lParam;
    }
}
