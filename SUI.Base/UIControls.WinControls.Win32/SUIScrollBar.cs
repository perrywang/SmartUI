using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUIScrollBar : SUIWindow
    {
        private bool IsStandlong = false; //flase means the scrollbar is not standalong control and belong to the parent control.
        private int VerticalOrHorizontal = 0; //default is 0 stand for Vertical ScrollBar,1 stand for Horizontal Scrollbar
        public SUIScrollBar(IntPtr handle, bool IsStandlong,int scrollbarType)
            :base(handle)
        {
            this.IsStandlong = IsStandlong;
            this.VerticalOrHorizontal = scrollbarType;
        }

        public SUIScrollBar(IntPtr handle,int scrollbarType)
            : this(handle, false, scrollbarType)
        { }

        public SUIScrollBar(SUIWindow win, int scrollbarType)
            :this(win.WindowHandle,scrollbarType)
        { }

        public SUIScrollBar(IntPtr handle)
            : this(handle, false,0)
        { }

        public SUIScrollBar(SUIWindow win)
            : this(win.WindowHandle)
        { }

        public SUIScrollBar(IntPtr handle, bool IsStandlong)
            : this(handle, IsStandlong, 0)
        { }
        public SUIScrollBar(SUIWindow win, bool IsStandlong)
            :this(win.WindowHandle,IsStandlong)
        { }

        public SUIScrollBar(SUIWindow win, bool IsStandlong, int scrollType)
            : this(win.WindowHandle, IsStandlong, scrollType)
        { }

        public void Scroll(ScrollBarAction ActionType)
        {
            if (!IsStandlong)
            {
                if (VerticalOrHorizontal == 0)  //scrolbar is vertical scrollbar
                {
                    SUIWinAPIs.SendMessage(this.WindowHandle,0x115,(int)ActionType,0);
                    SUIWinAPIs.SendMessage(this.WindowHandle, 0x115,8, 0);
                }
                else  //scrollbar is horizontal scroll bar
                {
                    SUIWinAPIs.SendMessage(this.WindowHandle, 0x114, (int)ActionType, 0);
                    SUIWinAPIs.SendMessage(this.WindowHandle, 0x114, 8, 0);
                }
            }
        }

        public void ScrollUpPage()
        {
            Scroll(ScrollBarAction.PageUpLeft);
        }

        public void ScrollUpLine()
        {
            Scroll(ScrollBarAction.LineUpLeft);
        }

        public void ScrollDownPage()
        {
            Scroll(ScrollBarAction.PageDownRight);
        }

        public void ScrollDownLine()
        {
            Scroll(ScrollBarAction.LineDownRight);
        }

        public void ScrollRightLine()
        {
            Scroll(ScrollBarAction.LineDownRight);
        }

        public void ScrollRightPage()
        {
            Scroll(ScrollBarAction.PageDownRight);
        }

        public void ScrollLeftPage()
        {
            Scroll(ScrollBarAction.PageUpLeft);
        }

        public void ScrollLeftMost()
        {
            Scroll(ScrollBarAction.TopLeft);
        }

        public void ScrollRightMost()
        {
            Scroll(ScrollBarAction.BottomRight);
        }

        public void ScrollDownMost()
        {
            Scroll(ScrollBarAction.BottomRight);
        }

        public void ScrollUpMost()
        {
            Scroll(ScrollBarAction.TopLeft);
        }

        public enum ScrollBarAction
        {
            BottomRight = 7,
            LineDownRight = 1,
            LineUpLeft = 0,
            PageDownRight = 3,
            PageUpLeft = 2,
            TopLeft = 6
        }

    }

    
}
