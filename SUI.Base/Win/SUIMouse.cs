using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using SUI.Base.SUIAccessible;

namespace SUI.Base.Win
{
    public class SUIMouse
    {
        public static void MouseClick(SUIWindow sui, int x, int y)
        {
            x += sui.X; y += sui.Y;
            MouseMove(x, y);
            MouseClick(x, y, false);
        }

        public static void MouseClickText(SUIWindow sui, string text)
        {
            Point p = SUIAccessibility.GetPositionFromText(sui.WindowHandle,text);
            MouseClick(sui, p.X, p.Y);
        }
        public static void MouseRightClick(SUIWindow sui, int x, int y)
        {
            x += sui.X; y += sui.Y;
            MouseMove(x, y);
            MouseRightClick(x, y, false);
        }

        public static void ClickButton(SUIWindow win)
        {
            MouseClick(win, 1, 1);
        }

        public static void MouseDrag(SUIWindow sui, int xStart, int yStart, int xEnd, int yEnd)
        {
            xStart += sui.X; yStart += sui.Y;
            xEnd += sui.X; yEnd += sui.Y;
            MouseMove(xStart, yStart);
            SUISleeper.Sleep(500);
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTDOWN), (uint)xStart, (uint)yStart, 0, 0);
            SUISleeper.Sleep(500);
            MouseMove(xEnd, yEnd);
            SUISleeper.Sleep(500);
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTUP), (uint)xEnd, (uint)yEnd, 0, 0);
        }
        public static void MouseDrag(SUIWindow startWindow, int xStart, int yStart, SUIWindow endWindow, int xEnd, int yEnd)
        {
            xStart += startWindow.X; yStart += startWindow.Y;
            xEnd += endWindow.X; yEnd += endWindow.Y;
            MouseMove(xStart, yStart);
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTDOWN), (uint)xStart, (uint)yStart, 0, 0);
            MouseMove(xEnd, yEnd);
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTUP), (uint)xEnd, (uint)yEnd, 0, 0);
        }

        public static void MouseDrag(int xStart, int yStart, int xEnd, int yEnd)
        {
            MouseMove(xStart, yStart);
            SUISleeper.Sleep(500);
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTDOWN), (uint)xStart, (uint)yStart, 0, 0);
            SUISleeper.Sleep(500);
            MouseMove(xEnd, yEnd);
            SUISleeper.Sleep(500);
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTUP), (uint)xEnd, (uint)yEnd, 0, 0);
            SUISleeper.Sleep(500);
        }
        public static void MouseMove(SUIWindow sui, int x, int y)
        {
            x += sui.X; y += sui.Y;
            MouseMove(x, y);
        }

        public static void MouseDoubleClick(SUIWindow sui, int x, int y)
        {
            x += sui.X; y += sui.Y;
            MouseMove(x, y);
            MouseDoubleClick(x, y, false);
        }

        private static void PixelsToAbsNormalCoors(int x, int y, ref int xOut, ref int YOut)
        {
            xOut = (int)Math.Round((double)((((double)(x * 0x10000)) / ((double)Screen.PrimaryScreen.Bounds.Width)) + 0.5));
            YOut = (int)Math.Round((double)((((double)(y * 0x10000)) / ((double)Screen.PrimaryScreen.Bounds.Height)) + 0.5));
        }

        public static void MouseClick(int x, int y) //In general coordinate need transfered
        {
            MouseMove(x,y,true);
            MouseClick(x, y, false);
        }
        public static void MouseClick(int x, int y, bool NeedTransfer)
        {
            if (NeedTransfer)
            {
                PixelsToAbsNormalCoors(x, y, ref x, ref y);
            }
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTDOWN), (uint)x, (uint)y, 0, 0);
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTUP), (uint)x, (uint)y, 0, 0);
        }
        public static void MouseMove(int x, int y) //In general coordinate need transfered
        {
            MouseMove(x, y, true);
        }
        public static void MouseMove(int x, int y, bool NeedTransfer)
        {
            if (NeedTransfer)
            {
                PixelsToAbsNormalCoors(x, y, ref x, ref y);
            }
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.MOVE), (uint)x, (uint)y, 0, 0);
        }
        public static void MouseRightClick(int x, int y) //In general coordinate need transfered
        {
            MouseRightClick(x, y, true);
        }
        public static void MouseRightClick(int x, int y, bool NeedTransfer)
        {
            if (NeedTransfer)
            {
                PixelsToAbsNormalCoors(x, y, ref x, ref y);
            }
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.RIGHTDOWN), (uint)x, (uint)y, 0, 0);
            SUIWinAPIs.mouse_event((uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.RIGHTUP), (uint)x, (uint)y, 0, 0);
        }

        public static void MouseDoubleClick(int x, int y, bool NeedTransfer)
        {
            if (NeedTransfer)
            {
                PixelsToAbsNormalCoors(x, y, ref x, ref y);
            }
            MouseClick(x, y, false);
            int interval=(int)Math.Round((double)(((double)SystemInformation.DoubleClickTime) / 3));
            interval = interval < 50 ? interval : 50;
            SUISleeper.Sleep(interval);
            MouseClick(x, y, false);
        }
        public static void MouseClick(SUIWindow win, int x, int y, int nFlags)
        {
            x += win.X;
            y += win.Y;
            MouseClick(x, y, nFlags);
        }


        public static void MouseClick(int x, int y, int nFlags) // nFlag hint whether 'Shift' 'Control' pressed
        {
            if (nFlags == 0) //'Shift' been pressed
            {
                SUIKeyboard.Press(SUI.Base.Win.SUIKeyboard.VK.SHIFT);
                MouseClick(x, y);
                SUIKeyboard.Release(SUI.Base.Win.SUIKeyboard.VK.SHIFT);
            }
            else if (nFlags == 1) // 'Control' been pressed
            {
                SUIKeyboard.Press(SUI.Base.Win.SUIKeyboard.VK.CONTROL);
                MouseClick(x, y);
                SUIKeyboard.Release(SUI.Base.Win.SUIKeyboard.VK.CONTROL);
            }
            else if (nFlags == 2) // 'Alt' been pressed
            {
                SUIKeyboard.Press(SUI.Base.Win.SUIKeyboard.VK.MENU);
                MouseClick(x, y);
                SUIKeyboard.Release(SUI.Base.Win.SUIKeyboard.VK.MENU);
            }
            else if (nFlags == 3) //double click action
            {
                MouseClick(x, y);
                SUISleeper.Sleep(50);
                MouseClick(x, y);
            }
            else
            {
                MouseClick(x, y);
            }
        }

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
    }
}
