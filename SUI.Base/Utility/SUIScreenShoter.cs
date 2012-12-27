using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using SUI.Base.Win;

namespace SUI.Base.Utility
{
    public class SUIScreenShoter
    {
        private static void Cleanup(IntPtr hBitmap, IntPtr hdcSrc, IntPtr hdcDest, IntPtr hwnd)
        {
            SUIWinAPIs.ReleaseDC(hwnd, hdcSrc);
            SUIWinAPIs.DeleteDC(hdcDest);
            SUIWinAPIs.DeleteObject(hBitmap);
        }

        private static void SaveImageAs(IntPtr hBitmap, string fileName, ImageFormat imageFormat)
        {
            Bitmap image =
            new Bitmap(Image.FromHbitmap(hBitmap),
                       Image.FromHbitmap(hBitmap).Width,
                       Image.FromHbitmap(hBitmap).Height);
            image.Save(fileName, imageFormat);

        }

        public static void CaptureDesktop(string fileName, ImageFormat imageFormat)
        {
            CaptureDialog(fileName, imageFormat, SUIWindow.DesktopWindow);
        }

        public static void CaptureDialog(string fileName, ImageFormat imageFormat, SUIWindow sui)
        {
            Rectangle rDialog = new Rectangle();
            SUIWinAPIs.GetWindowRect(sui.WindowHandle, out rDialog);

            IntPtr hdcSrc = SUIWinAPIs.GetWindowDC(sui.WindowHandle);
            IntPtr hdcDest = SUIWinAPIs.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = SUIWinAPIs.CreateCompatibleBitmap(hdcSrc, rDialog.Width-rDialog.X, rDialog.Height-rDialog.Y);

            SUIWinAPIs.SelectObject(hdcDest, hBitmap);

            SUIWinAPIs.BitBlt(hdcDest, 0, 0, rDialog.Width-rDialog.X,
                            rDialog.Height-rDialog.Y,
                            hdcSrc, 0, 0, 0x00CC0020);

            SaveImageAs(hBitmap, fileName, imageFormat);
            Cleanup(hBitmap, hdcSrc, hdcDest, sui.WindowHandle);
        }

#region setscreenresolution
        
        enum DMDO
        {
            DEFAULT = 0,
            D90 = 1,
            D180 = 2,
            D270 = 3
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct DEVMODE
        {
            public const int DM_DISPLAYFREQUENCY = 0x400000;
            public const int DM_PELSWIDTH = 0x80000;
            public const int DM_PELSHEIGHT = 0x100000;
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public DMDO dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        } 
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int ChangeDisplaySettings([In] ref DEVMODE lpDevMode, int dwFlags); 
        public static void SetScreenResolution(int width, int height)
        {
            long RetVal = 0;
            DEVMODE dm = new DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            dm.dmPelsWidth = width;
            dm.dmPelsHeight = height;
            dm.dmFields = DEVMODE.DM_PELSWIDTH | DEVMODE.DM_PELSHEIGHT;
            RetVal = ChangeDisplaySettings(ref dm, 0);
        }

#endregion setscreenresolution
    }
}
