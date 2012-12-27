using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SUI.Base.Win;
using System.Drawing.Imaging;
using Microsoft.VisualBasic.FileIO;
using SUI.Base.SUIExceptions;
using System.Runtime.InteropServices;
namespace SUI.Base.Utility
{
    public class SUIImage
    {
        private static string tmpFile = FileSystem.CombinePath(SpecialDirectories.Temp, "temp");
        private static string tmpFileSuffix = ".bmp";
        private static int index = 0;
        public static void FillSpecialArea(Bitmap bitmap, Rectangle specialArea)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            for (int i = 0; i < width; i++)
            {
                 for (int j = 0; j < height; j++)
                {
                    Point index = new Point(i, j);
                    if (specialArea.Contains(index))
                    {
                        bitmap.SetPixel(index.X, index.Y, Color.White);
                    }
                }
            }
        }
        public static SUIBitmap GetImageFromWindow(SUIWindow sui)
        {
            return GetImageFromWindow(sui, false);
        }
        public static SUIBitmap GetImageFromWindow(SUIWindow sui, bool withCursor)
        {
            if (null != sui)
            {
                SUISleeper.Sleep(2000);  //make sure paint compldte!
                //Rectangle rDialog = new Rectangle();
                try
                {
                    if (!sui.ClassName.Equals("#32768"))
                    {
                        sui.BringToTopMost();
                        sui.ShowShortCutIndicators();
                        if (!withCursor)
                            SUIWinAPIs.SetCursorPos(SUIWindow.DesktopWindow.Width / 2, 0);// Set Cursor at top mid of screen
                    }
                }
                catch (Exception e)
                {
                    throw new SUIGetImageException("Win32 SDK Platform Exception!", e);
                }
                IntPtr hdcSrc = IntPtr.Zero;
                IntPtr hdcDest = IntPtr.Zero;
                IntPtr hBitmap = IntPtr.Zero;
                Bitmap image = null;
                try
                {
                    //index to solve the problem of GDR+ when run IE 2 times in one case.
                    index = ((index % 10) + 1);
                    //SUIWinAPIs.GetWindowRect(sui.WindowHandle, out rDialog);
                    int buffer = 0;   
                    if ((SUIUtil.IsCurrentOSVista || SUIUtil.IsCurrentOSXP) && sui.IsWinForm&&sui.Maximized)
                    {
                        buffer = 8;     // under vista need cut a little height of window
                    }
                    hdcSrc = SUIWinAPIs.GetWindowDC(sui.WindowHandle);
                    hdcDest = SUIWinAPIs.CreateCompatibleDC(hdcSrc);
                    hBitmap = SUIWinAPIs.CreateCompatibleBitmap(hdcSrc, sui.Width - sui.X, sui.Height - sui.Y-buffer);
                    SUIWinAPIs.SelectObject(hdcDest, hBitmap);
                    SUIWinAPIs.BitBlt(hdcDest, 0, 0, sui.Width - sui.X,
                                    sui.Height - sui.Y-buffer,
                                    hdcSrc, 0, 0, 0x00CC0020);
                    image = new Bitmap(Image.FromHbitmap(hBitmap),
                                                                Image.FromHbitmap(hBitmap).Width,
                                                                Image.FromHbitmap(hBitmap).Height);

                    //Draw cursor image
                    if (withCursor)
                    {
                        int cursorX = 0, cursorY = 0;
                        SUIBitmap cursorBmp = GetCursorImage(ref cursorX, ref cursorY);
                        if (cursorBmp != null)
                        {
                            Bitmap cBmp = cursorBmp.Bitmap;
                            Rectangle r = new Rectangle(cursorX - sui.X, cursorY - sui.Y, cBmp.Width, cBmp.Height);
                            Graphics g = Graphics.FromImage(image);
                            g.DrawImage(cBmp, r);
                            g.Flush();
                            cBmp.Dispose();
                        }
                    }

                    image.Save(tmpFile + index + tmpFileSuffix);
                    image.Dispose();
                    //SaveImg(image,tmpFile,ImageFormat.Bmp);
                }catch (Exception e)
                 {
                    throw new SUIGetImageException("Win32 SDK Platform Exception!", e);  
                 }finally
                  {
                    
                        if (!hdcSrc.Equals(IntPtr.Zero))
                        {
                            SUIWinAPIs.ReleaseDC(sui.WindowHandle, hdcSrc);
                        }
                        if (!hdcDest.Equals(IntPtr.Zero))
                        {
                            SUIWinAPIs.DeleteDC(hdcDest);
                        }
                        if (!hBitmap.Equals(IntPtr.Zero))
                        {
                            SUIWinAPIs.DeleteObject(hBitmap);
                        }
                        if (image != null)
                        {
                            image.Dispose();
                        }
                  }
                  SUIBitmap ImageOfWindow = SUIBitmap.LoadSUIBitmap(tmpFile + index + tmpFileSuffix);
                
                return ImageOfWindow;
            }
            throw new SUIGetImageException("Parameter SUIWindow sui is Null");
        }
        public static SUIBitmap GetImageFromWindow(SUIWindow sui, Rectangle rectangle)
        {
            return GetImageFromWindow(sui, rectangle, false);
        }
        public static SUIBitmap GetImageFromWindow(SUIWindow sui,Rectangle rectangle, bool withCursor)
        {
            if (null != sui)
            {
                SUISleeper.Sleep(2000);  //make sure paint compldte!
                //Rectangle rDialog = new Rectangle();
                try
                {
                    sui.BringToTopMost();
                    sui.ShowShortCutIndicators();
                    if (!withCursor)
                        SUIWinAPIs.SetCursorPos(SUIWindow.DesktopWindow.Width / 2, 0);// Set Cursor at top mid of screen
                }
                catch (Exception e)
                {
                    throw new SUIGetImageException("Win32 SDK Platform Exception!", e);
                }
                IntPtr hdcSrc = IntPtr.Zero;
                IntPtr hdcDest = IntPtr.Zero;
                IntPtr hBitmap = IntPtr.Zero;
                Bitmap image = null;
                try
                {
                    //index to solve the problem of GDR+ when run IE 2 times in one case.
                    index = ((index % 10) + 1);
                    //SUIWinAPIs.GetWindowRect(sui.WindowHandle, out rDialog);
                   
                    hdcSrc = SUIWinAPIs.GetWindowDC(sui.WindowHandle);
                    hdcDest = SUIWinAPIs.CreateCompatibleDC(hdcSrc);
                    hBitmap = SUIWinAPIs.CreateCompatibleBitmap(hdcSrc, rectangle.Width, rectangle.Height);
                    try
                    {
                        SUIWinAPIs.SelectObject(hdcDest, hBitmap);
                        SUIWinAPIs.BitBlt(hdcDest, 0, 0, rectangle.Width, rectangle.Height, hdcSrc, rectangle.X, rectangle.Y, 0x00CC0020);
                    }
                    catch
                    {
                        throw new SUIException("parameters width or height execeed arrange!");
                    }
                    image = new Bitmap(Image.FromHbitmap(hBitmap),
                                                                Image.FromHbitmap(hBitmap).Width,
                                                                Image.FromHbitmap(hBitmap).Height);

                    //Draw cursor image
                    if (withCursor)
                    {
                        int cursorX = 0, cursorY = 0;
                        SUIBitmap cursorBmp = GetCursorImage(ref cursorX, ref cursorY);
                        if (cursorBmp != null && rectangle.Contains(cursorX - sui.X, cursorY - sui.Y))
                        {
                            Bitmap cBmp = cursorBmp.Bitmap;
                            Rectangle r = new Rectangle(cursorX - sui.X, cursorY - sui.Y, cBmp.Width, cBmp.Height);
                            Graphics g = Graphics.FromImage(image);
                            g.DrawImage(cBmp, r);
                            g.Flush();
                            cBmp.Dispose();
                        }
                    }

                    image.Save(tmpFile + index + tmpFileSuffix);
                    image.Dispose();
                    //SaveImg(image,tmpFile,ImageFormat.Bmp);
                }
                catch (Exception e)
                {
                    throw new SUIGetImageException("Win32 SDK Platform Exception!", e);
                }
                finally
                {

                    if (!hdcSrc.Equals(IntPtr.Zero))
                    {
                        SUIWinAPIs.ReleaseDC(sui.WindowHandle, hdcSrc);
                    }
                    if (!hdcDest.Equals(IntPtr.Zero))
                    {
                        SUIWinAPIs.DeleteDC(hdcDest);
                    }
                    if (!hBitmap.Equals(IntPtr.Zero))
                    {
                        SUIWinAPIs.DeleteObject(hBitmap);
                    }
                    if (image != null)
                    {
                        image.Dispose();
                    }
                }
                SUIBitmap ImageOfWindow = SUIBitmap.LoadSUIBitmap(tmpFile + index + tmpFileSuffix);

                return ImageOfWindow;
            }
            throw new SUIGetImageException("Parameter SUIWindow sui is Null");
        }

        public static SUIBitmap GetCursorImage(ref int x, ref int y)
        {
            Bitmap bmp;
            IntPtr hicon;
            CURSORINFO ci = new CURSORINFO();
            ICONINFO icInfo;
            ci.cbSize = Marshal.SizeOf(ci);
            if (SUIWinAPIs.GetCursorInfo(out ci))
            {
                if (ci.flags == SUIMessage.CURSOR_SHOWING)
                {
                    hicon = SUIWinAPIs.CopyIcon(ci.hCursor);
                    if (SUIWinAPIs.GetIconInfo(hicon, out icInfo))
                    {
                        x = ci.ptScreenPos.X - ((int)icInfo.xHotspot);
                        y = ci.ptScreenPos.Y - ((int)icInfo.yHotspot);
                        Icon ic = Icon.FromHandle(hicon);
                        bmp = ic.ToBitmap();

                        return new SUIBitmap(bmp, null);
                    }
                }
            }
            return null;

        }
    }
}
