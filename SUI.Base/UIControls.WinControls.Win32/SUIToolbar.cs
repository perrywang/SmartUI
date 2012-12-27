using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using SUI.Base.Win;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIToolbar : SUIWindow
    {
        public SUIToolbar(IntPtr hWnd)
            : base(hWnd)
        { }

        public SUIToolbar(SUIWindow win)
            : base(win)
        { }

        public SUIToolbarButtonCollection ButtonList
        {
            get
            {
                RetrieveButtonInfo();
                return buttonList;
            }
        }

        private bool isDocking = false;
        public bool IsDocking
        {
            get
            {
                return isDocking;
            }
            set
            {
                isDocking = value;
            }
        }
        public void Click(int nIndex)
        {
            if (buttonList == null)
            {
                RetrieveButtonInfo();
            }
            buttonList[nIndex].Click();
        }

        public void MouseMoveTo(int nIndex)
        {
            if (buttonList == null)
            {
                RetrieveButtonInfo();
            }
            buttonList[nIndex].MouseMoveTo();
        }

        protected SUIToolbarButtonCollection buttonList; 
        protected virtual void RetrieveButtonInfo()
        {
            if (buttonList == null)
            {
                buttonList = new SUIToolbarButtonCollection();

                int processId = 0;
                SUIWinAPIs.GetWindowThreadProcessId(WindowHandle, ref processId);
                IntPtr processHandle = SUIWinAPIs.OpenProcess(SUIMessage.PROCESS_ALL_ACCESS, false, processId);

                IntPtr data = SUIWinAPIs.VirtualAllocEx(processHandle, IntPtr.Zero, Marshal.SizeOf(typeof(TBBUTTON)), SUIMessage.MEM_COMMIT, SUIMessage.PAGE_READWRITE);
                buttonCount = SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.TB_BUTTONCOUNT, IntPtr.Zero, IntPtr.Zero);
                for (int i = 0; i < buttonCount; i++)
                {
                    TBBUTTON btn = new TBBUTTON();
                    IntPtr ptr = IntPtr.Zero;
                    try
                    {
                        ptr = Marshal.AllocHGlobal(Marshal.SizeOf(btn));

                        SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.TB_GETBUTTON, new IntPtr(i), data);
                        IntPtr numReaded = new IntPtr();
                        SUIWinAPIs.ReadProcessMemory(processHandle, data, ptr, Marshal.SizeOf(typeof(TBBUTTON)), out numReaded);

                        btn = (TBBUTTON)Marshal.PtrToStructure(ptr, btn.GetType());

                        buttonList.Add(new SUIToolbarButton(this, btn, i));
                    }
                    catch (Exception e)
                    {
                        throw new SUIException("Error getting toolbar button information!", e);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }

                SUIWinAPIs.VirtualFreeEx(processHandle, data, Marshal.SizeOf(typeof(TBBUTTON)), SUIMessage.MEM_RESERVE);
                SUIWinAPIs.CloseHandle(processHandle);
            }
        }

        private int buttonCount;
        public int ButtonCount
        {
            get
            {
                RetrieveButtonInfo();
                return buttonCount;
            }
        }
        public int GetItemIndexFromPoint(Point point) //point is relative coordination
        {
            for (int itemIndex=0; itemIndex < ButtonCount; itemIndex++)
            {
                Rectangle rect = ButtonList[itemIndex].Rectangle;
                rect.Width = rect.Width - rect.X;
                rect.Height = rect.Height - rect.Y;
                if (rect.Contains(point))
                {
                    return itemIndex;
                }
            }
            return -1;
        }

        public virtual SUIToolbarButton GetItemByIndex(int index)
        {
            return ButtonList[index];
        }

        public SUIPopupMenu PopupMenu(int index)  //for CRW OLAP member selector dialog
        {
            try
            {
                SUIToolbarButton btnWithCtxMenu = ButtonList[index];
                ClickWindow(btnWithCtxMenu.Rectangle.X + btnWithCtxMenu.Rectangle.Width - 5, btnWithCtxMenu.Rectangle.Height / 2);
                return SUIPopupMenu.FindPopupMenu();
            }
            catch
            {
                return null;
            }
        }
    }

    public class SUIToolbarButtonCollection : ReadOnlyCollectionBase
    {
        public void Add(SUIToolbarButton button)
        {
            this.InnerList.Add(button);
        }

        public SUIToolbarButton this[int index]
        {
            get
            {
                return (SUIToolbarButton)this.InnerList[index];
            }
        }

        public SUIToolbarButton this[string text]
        {
            get
            {
                return null;
            }
        }
    }
}
