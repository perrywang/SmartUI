using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using SUI.Base.SUIExceptions;
using SUI.Base.Utility;
using SUI.Base.UIControls.WinControls.Win32;

namespace SUI.Base.Utility
{
    public abstract class SUIComparer
    {
        public abstract bool Compare(Object obj1, Object obj2);

        public abstract bool Compare(Object obj1, Object obj2, CompareMode compareMode);

        public static SUIComparer GetComparer(ComparerType type)
        {
            switch (type)
            {
                case ComparerType.File:
                    return new SUIFileComparer();
                case ComparerType.Image:
                    return new SUIImageComparer();
                case ComparerType.Color:
                    return new SUIColorComparer();
                case ComparerType.WindowImage:
                    return new SUIWindowImageComparer();
                default:
                    return null;
            }
        }
    }

    public enum ComparerType :int
    {
        File,
        WindowImage,
        Image,
        Color
    }

    public enum CompareMode : int
    {
        Binary,
        Text,
        HtmlVSFile,
        HtmlVSImg
    }

    internal class SUIFileComparer : SUIComparer
    {
        public override bool Compare(Object obj1, Object obj2)
        {
            if (obj1 != null && obj2 != null)
            {
                if (obj1 is File && obj2 is File)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Compare(Object file1, Object file2, CompareMode compareMode)
        {
            if (file1 != null && file2 != null)
            {
                if (File.Exists((String)file1) && File.Exists((String)file2))
                {
                    switch (compareMode)
                    {
                        case CompareMode.Binary:
                            return compareBin((String)file1,(String)file2);
                        case CompareMode.Text:
                            return compareTxt((String)file1, (String)file2);
                    }
                }
            }

            return false;
        }

        private bool compareBin(string file1,string file2)
        {
            int i = 0, j = 0;
            FileStream f1;
            FileStream f2;
            f1 = new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.Read);
            f2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read);
            do
            {
                i = f1.ReadByte();
                j = f2.ReadByte();
                if (i != j) break;
            } while (i != -1 && j != -1);
            if (i != j)
            {
                f1.Close();
                f2.Close(); 
                return false;
            }
            else
            {
                f1.Close();
                f2.Close(); 
                return true;
            }
        }

        private bool compareTxt(string file1, string file2)
        {
            StreamReader f1 = new StreamReader(file1);
            StreamReader f2 = new StreamReader(file2);
            string line1;
            string line2;
            try
            {
                while ((line1 = f1.ReadLine()) != null)
                {
                    if ((line2 = f2.ReadLine()) != null)
                    {
                        if (!line1.Equals(line2))
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                if (f2.ReadLine() != null)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                f1.Close();
                f2.Close();
            }
        } 
    }

    public class SUIImageComparer : SUIComparer
    {
        private SUIComparer colorComparer = SUIComparer.GetComparer(ComparerType.Color);
        public override bool Compare(Object obj1, Object obj2)
        {
            if (obj1 != null && obj2 != null)
            {
                if (obj1 is SUIBitmap && obj2 is SUIBitmap)
                {
                    bool bFlag = OptionCompare(obj1 as SUIBitmap , obj2 as SUIBitmap , Rectangle.Empty);
                    return bFlag;
                }
            }
            if (obj1 == null)
            {
                SUIBitmap temp = obj2 as SUIBitmap;
                if (!temp.IsNullBitmap)
                {
                    temp.Save();
                    return true;
                }
            }
            return false;
           
        }
       /*do not compare specialArea*/
        public bool OptionCompare(SUIBitmap SUIbmBase, SUIBitmap SUIbmTest, Rectangle specialArea)
        {
            if (SUIbmBase.IsNullBitmap)
            {
                SUIBitmap temp = SUIbmTest as SUIBitmap;
                if (!temp.IsNullBitmap)
                {
                    temp.Path = SUIbmBase.Path;
                    temp.Save();
                    temp.ReleaseMem();
                    return true;
                }
            }
            bool bFlag = false;
            Bitmap bmBase = SUIbmBase.Bitmap;
            Bitmap bmTest = SUIbmTest.Bitmap;
            int base_width = bmBase.Width;
            int base_height = bmBase.Height;

            int test_width = bmTest.Width;
            int test_height = bmTest.Height;

            if ((base_width != test_width) || (base_height != test_height))
            {
                SUIbmTest.Path = SUIbmBase.Path.Replace(".bmp", "_error.bmp");
                SUIbmTest.Save();
                SUIbmTest.ReleaseMem();
                //SUIbmBase.ReleaseMem();
                return bFlag;
            }
           
            int i, j;
            Color pixBase;
            Color pixTest;
            Bitmap difference = new Bitmap(base_width, base_height);
            bool bChange = false;
            for (i = 0; i < base_width; i++)
            {
                for (j = 0; j < base_height; j++)
                {
                   
                    pixBase = bmBase.GetPixel(i, j);
                    pixTest = bmTest.GetPixel(i, j);
                    Point temp = new Point(i, j);
                    if ((! specialArea.IsEmpty ) && specialArea.Contains(temp))
                    {
                        //difference.SetPixel(i,j,Color.White);
                        continue;
                    }
                    if (!colorComparer.Compare(pixBase, pixTest))
                    {
                        //SUIbmTest.Path = SUIbmTest.Path.Replace(".bmp", "_error.bmp");
                        //SUIbmTest.Save();
                        bChange = true;
                        difference.SetPixel(i, j, Color.Black);
                        //return bFlag;
                    }
                    else
                    {
                        difference.SetPixel(i, j, Color.White);
                    }
                }
            }
            if (bChange)
            {
                SUIbmTest.Path = SUIbmBase.Path.Replace(".bmp", "_error.bmp");
                SUIbmTest.Save();
                SUIbmTest.ReleaseMem();
                string path = SUIbmBase.Path.Replace(".bmp", "_error_difference.bmp"); ;
                difference.Save(path);
                difference.Dispose();
                //SUIbmBase.ReleaseMem();
                return bFlag;
            }
            else
            {
                SUIbmTest.ReleaseMem();
                difference.Dispose();
                bFlag = true;
                return bFlag;
            }
 
        }
       
        /*only compare specialArea*/
        public bool OptionCompare2(SUIBitmap SUIbmBase, SUIBitmap SUIbmTest, Rectangle specialArea)
        {
            if (SUIbmBase.IsNullBitmap)
            {
                SUIBitmap temp = SUIbmTest as SUIBitmap;
                if (!temp.IsNullBitmap)
                {
                    temp.Save();
                    return true;
                }
            } 
            bool bFlag = false;
            Bitmap bmBase = SUIbmBase.Bitmap;
            Bitmap bmTest = SUIbmTest.Bitmap;
            int base_width = bmBase.Width;
            int base_height = bmBase.Height;

            int test_width = bmTest.Width;
            int test_height = bmTest.Height;

            if ((base_width != test_width) || (base_height != test_height))
            {
                SUIbmTest.Path = SUIbmTest.Path.Replace(".bmp", "_error.bmp");
                SUIbmTest.Save();
                return bFlag;
            }

            int i, j;
            Color pixBase;
            Color pixTest;
            Bitmap difference = new Bitmap(base_width, base_height);
            bool bChange = false;
            for (i = 0; i < base_width; i++)
            {
                for (j = 0; j < base_height; j++)
                {
                    pixBase = bmBase.GetPixel(i, j);
                    pixTest = bmTest.GetPixel(i, j);
                    Point temp = new Point(i, j);
                    if (specialArea.Contains(temp))
                    {
                        if (!colorComparer.Compare(pixBase, pixTest))
                        {
                            difference.SetPixel(i, j, Color.Black);
                            bChange = true;
                        }
                        else
                        {
                            difference.SetPixel(i, j, Color.White);
                        }
                    }
                }
            }

            if (bChange)
            {
                SUIbmTest.Path = SUIbmBase.Path.Replace(".bmp", "_error.bmp");
                SUIbmTest.Save();
                SUIbmTest.ReleaseMem();
                string path = SUIbmTest.Path.Replace(".bmp", "_error_difference.bmp"); ;
                difference.Save(path);
                difference.Dispose();
            }
            else
            {
                SUIbmTest.ReleaseMem();
                difference.Dispose();
                bFlag = true;
            }
            return bFlag;
        }
        /*do not compare some areas*/
        public bool OptionCompare3(SUIBitmap SUIbmBase, SUIBitmap SUIbmTest, Rectangle[] specialAreas)
        {
            if (SUIbmBase.IsNullBitmap)
            {
                SUIBitmap temp = SUIbmTest as SUIBitmap;
                if (!temp.IsNullBitmap)
                {
                    temp.Path = SUIbmBase.Path;
                    temp.Save();
                    temp.ReleaseMem();
                    return true;
                }
            }
            bool bFlag = false;
            Bitmap bmBase = SUIbmBase.Bitmap;
            Bitmap bmTest = SUIbmTest.Bitmap;
            int base_width = bmBase.Width;
            int base_height = bmBase.Height;

            int test_width = bmTest.Width;
            int test_height = bmTest.Height;

            if ((base_width != test_width) || (base_height != test_height))
            {
                SUIbmTest.Path = SUIbmBase.Path.Replace(".bmp", "_error.bmp");
                SUIbmTest.Save();
                SUIbmTest.ReleaseMem();
                return bFlag;
            }

            int i, j;
            Color pixBase;
            Color pixTest;
            Bitmap difference = new Bitmap(base_width, base_height);
            bool bChange = false;
            for (i = 0; i < base_width; i++)
            {
                for (j = 0; j < base_height; j++)
                {
                    pixBase = bmBase.GetPixel(i, j);
                    pixTest = bmTest.GetPixel(i, j);
                    Point temp = new Point(i, j);
                    if (!IsContainsPoint(specialAreas,temp))
                    {
                        if (!colorComparer.Compare(pixBase, pixTest))
                        {
                            difference.SetPixel(i, j, Color.Black);
                            bChange = true;
                        }
                        else
                        {
                            difference.SetPixel(i,j,Color.White);
                        }
                    }
                    else
                    {
                        //difference.SetPixel(i,j,Color.White);
                    }
                 }
             }
            if (bChange)
            {
                SUIbmTest.Path = SUIbmBase.Path.Replace(".bmp", "_error.bmp");
                SUIbmTest.Save();
                SUIbmTest.ReleaseMem();
                string path = SUIbmTest.Path.Replace(".bmp", "_error_difference.bmp"); ;
                difference.Save(path);
                return bFlag;
            }
            bFlag = true;
            return bFlag;
            
        }
        private bool IsContainsPoint(Rectangle[] areas, Point point)
        {
            foreach (Rectangle rectangle in areas)
            {
                if (rectangle.Contains(point))
                    return true;
            }
            return false;
        }

        public override bool Compare(object obj1, object obj2, CompareMode compareMode)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class SUIWindowImageComparer : SUIComparer
    {
        private SUIComparer colorComparer = SUIComparer.GetComparer(ComparerType.Color);
        //The first parameter is an object of SUIWindow and the second one is an object of SUIBitmap
        public override bool Compare(object obj1, object obj2)
        {
            if (obj1 != null && obj2 != null)
            {
                if (obj1 is SUIWindow && obj2 is SUIBitmap)
                {
                    bool bFlag = CompareWindowImage(obj1 as SUIWindow, obj2 as SUIBitmap);
                    return bFlag;
                }
            }
            return false;
        }
        private bool IsContainsPoint(Rectangle[] areas, Point point)
        {
            foreach (Rectangle rectangle in areas)
            {
                if (rectangle.Contains(point))
                    return true;
            }
            return false;
        }
        public bool CompareWindowImage(SUIWindow window, SUIBitmap baseImage,Rectangle[] NotCompare)
        {
            SUIBitmap windowImage = SUIImage.GetImageFromWindow(window);
            windowImage.Path = baseImage.Path; //2007 5-24 fhu add this line.
            if (baseImage.IsNullBitmap)
            {
                if (!windowImage.IsNullBitmap)
                {
                    windowImage.Path = baseImage.Path;
                    windowImage.Save();
                    windowImage.ReleaseMem();
                    return true;
                }
            }
            //Here's the Vista specific image comparison.
            if ((SUIUtil.IsCurrentOSVista || SUIUtil.IsCurrentOSXP) && (window.IsDialog || window.IsWinForm))
            {
                SUIDialog dialog = new SUIDialog(window);
                SUIButton defaultButton = dialog.DefaultPushButton;
                if (defaultButton == null)
                    return SUIComparer.GetComparer(ComparerType.Image).Compare(baseImage, windowImage);
                else
                {
                    Bitmap bmBase = baseImage.Bitmap;
                    Bitmap bmTest = windowImage.Bitmap;
                    int base_width = bmBase.Width;
                    int base_height = bmBase.Height;

                    int test_width = bmTest.Width;
                    int test_height = bmTest.Height;

                    //First of all, compare the size of images.
                    if ((base_width != test_width) || (base_height != test_height))
                    {
                        windowImage.Path = baseImage.Path.Replace(".bmp", "_error.bmp");
                        windowImage.Save();
                        windowImage.ReleaseMem();
                        return false;
                    }

                    //Retrieve button area.
                    Rectangle defaultButtonRectangle = new Rectangle();
                    defaultButtonRectangle.X = defaultButton.X - dialog.X;
                    defaultButtonRectangle.Y = defaultButton.Y - dialog.Y;
                    defaultButtonRectangle.Width = defaultButton.Width - defaultButton.X;
                    defaultButtonRectangle.Height = defaultButton.Height - defaultButton.Y;

                    // Ignor the comparison of a 4-pixel width border so that we could
                    // get rid of the side effect of dash-line rectangle on the button.
                    Rectangle defaultButtonRectangleEx = new Rectangle();
                    defaultButtonRectangleEx.X = defaultButtonRectangle.X + 4;
                    defaultButtonRectangleEx.Y = defaultButtonRectangle.Y + 4;
                    defaultButtonRectangleEx.Width = defaultButtonRectangle.Width - 8;
                    defaultButtonRectangleEx.Height = defaultButtonRectangle.Height - 8;
                    //Ignor the effect of flatness under vista
                    Rectangle flatnessRectangle1 = new Rectangle();
                    flatnessRectangle1.X = 0;
                    flatnessRectangle1.Y = 0;
                    flatnessRectangle1.Width = 6;
                    flatnessRectangle1.Height = 6;
                    Rectangle flatnessRectangle2 = new Rectangle();
                    flatnessRectangle2.X = dialog.Width -dialog.X- 6;
                    flatnessRectangle2.Y = 0;
                    flatnessRectangle2.Width = 6;
                    flatnessRectangle2.Height = 6;
                    Rectangle[] FlatnessAreas = new Rectangle[2];
                    FlatnessAreas[0] = flatnessRectangle1;
                    FlatnessAreas[1] = flatnessRectangle2;
                    Bitmap difference = new Bitmap(base_width, base_height);
                    bool bChange = false;
                    int i, j;
                    Color pixBase;
                    Color pixTest;
                    for (i = 0; i < base_width; i++)
                    {
                        for (j = 0; j < base_height; j++)
                        {
                            pixBase = bmBase.GetPixel(i, j);
                            pixTest = bmTest.GetPixel(i, j);
                            Point temp = new Point(i, j);
                            if (IsContainsPoint(FlatnessAreas,temp))
                            {
                                //difference.SetPixel(i, j, Color.White);
                                continue;
                            }
                            if (IsContainsPoint(NotCompare,temp))
                            {
                                //difference.SetPixel(i, j, Color.White);
                                continue;
                            }
                            if (!defaultButtonRectangle.Contains(temp))
                            {
                                if (!colorComparer.Compare(pixBase, pixTest))
                                {
                                    bChange = true;
                                    difference.SetPixel(i, j, Color.Black);
                                }
                                else
                                    difference.SetPixel(i, j, Color.White);
                            }
                            else if (!defaultButtonRectangleEx.Contains(temp))
                            {
                                // Ignor the pixels in this border.
                                //difference.SetPixel(i, j, Color.Orange);
                            }
                            else //Process the pixels in OK button rectangle
                            {
                                if (!colorComparer.Compare(pixBase, pixTest))
                                {
                                    int defaultBuffer = SUIColorComparer.buffer;
                                    //Temproraly set buffer to 150 since in this special area we need a larger buffer.
                                    SUIColorComparer.buffer = 150;
                                    if (colorComparer.Compare(pixBase, SystemColors.ControlText) || colorComparer.Compare(pixTest, SystemColors.ControlText))
                                    {
                                        bChange = true;
                                        difference.SetPixel(i, j, Color.Black);
                                    }
                                    //else
                                    //    difference.SetPixel(i, j, Color.Blue);
                                    // Set the default buffer back.
                                    SUIColorComparer.buffer = defaultBuffer;
                                }
                                //else
                                //    difference.SetPixel(i, j, Color.Red);
                            }
                        }
                    }

                    if (bChange)
                    {
                        windowImage.Path = baseImage.Path.Replace(".bmp", "_error.bmp");
                        windowImage.Save();
                        windowImage.ReleaseMem();
                        string path = baseImage.Path.Replace(".bmp", "_error_difference.bmp");
                        difference.Save(path);
                        return false;
                    }
                    return true;
                }
            }
            else
            {
                SUIImageComparer imgComparer = new SUIImageComparer();
                return imgComparer.OptionCompare3(baseImage,windowImage,NotCompare);
            }
        }
        public bool CompareWindowImage(SUIWindow window, SUIBitmap baseImage)
        {
            SUIBitmap windowImage = SUIImage.GetImageFromWindow(window);
            windowImage.Path = baseImage.Path; //2007 5-24 fhu add this line.
            if (baseImage.IsNullBitmap)
            {
                if (!windowImage.IsNullBitmap)
                {
                    windowImage.Path = baseImage.Path;
                    windowImage.Save();
                    windowImage.ReleaseMem();
                    return true;
                }
            }
            //Here's the Vista specific image comparison.
            // Both dialog and winform need to apply this algorithm.
            if ((SUIUtil.IsCurrentOSVista || SUIUtil.IsCurrentOSXP) && (window.IsDialog || window.IsWinForm))
            {
                SUIDialog dialog = new SUIDialog(window);
                SUIButton defaultButton = dialog.DefaultPushButton;
                if (defaultButton == null)
                    return SUIComparer.GetComparer(ComparerType.Image).Compare(baseImage,windowImage);
                else
                {
                    Bitmap bmBase = baseImage.Bitmap;
                    Bitmap bmTest = windowImage.Bitmap;
                    int base_width = bmBase.Width;
                    int base_height = bmBase.Height;

                    int test_width = bmTest.Width;
                    int test_height = bmTest.Height;

                    //First of all, compare the size of images.
                    if ((base_width != test_width) || (base_height != test_height))
                    {
                        windowImage.Path = baseImage.Path.Replace(".bmp", "_error.bmp");
                        windowImage.Save();
                        windowImage.ReleaseMem();
                        return false;
                    }

                    //Retrieve button area.
                    Rectangle defaultButtonRectangle = new Rectangle();
                    defaultButtonRectangle.X = defaultButton.X - dialog.X;
                    defaultButtonRectangle.Y = defaultButton.Y - dialog.Y;
                    defaultButtonRectangle.Width = defaultButton.Width - defaultButton.X;
                    defaultButtonRectangle.Height = defaultButton.Height - defaultButton.Y;

                    // Ignor the comparison of a 4-pixel width border so that we could
                    // get rid of the side effect of dash-line rectangle on the button.
                    Rectangle defaultButtonRectangleEx = new Rectangle();
                    defaultButtonRectangleEx.X = defaultButtonRectangle.X + 4;
                    defaultButtonRectangleEx.Y = defaultButtonRectangle.Y + 4;
                    defaultButtonRectangleEx.Width = defaultButtonRectangle.Width - 8;
                    defaultButtonRectangleEx.Height = defaultButtonRectangle.Height - 8;
                    //Ignor the effect of flatness under vista
                    Rectangle flatnessRectangle1 = new Rectangle();
                    flatnessRectangle1.X = 0;
                    flatnessRectangle1.Y = 0;
                    flatnessRectangle1.Width = 6;
                    flatnessRectangle1.Height = 6;
                    Rectangle flatnessRectangle2 = new Rectangle();
                    flatnessRectangle2.X = dialog.Width - dialog.X - 6;
                    flatnessRectangle2.Y = 0;
                    flatnessRectangle2.Width = 6;
                    flatnessRectangle2.Height = 6;
                    Rectangle[] FlatnessAreas = new Rectangle[2];
                    FlatnessAreas[0] = flatnessRectangle1;
                    FlatnessAreas[1] = flatnessRectangle2;

                    Bitmap difference = new Bitmap(base_width, base_height);
                    bool bChange = false;
                    int i, j;
                    Color pixBase;
                    Color pixTest;
                    for (i = 0; i < base_width; i++)
                    {
                        for (j = 0; j < base_height; j++)
                        {
                            pixBase = bmBase.GetPixel(i, j);
                            pixTest = bmTest.GetPixel(i, j);
                            Point temp = new Point(i, j);
                            if (IsContainsPoint(FlatnessAreas,temp))
                            {
                                //difference.SetPixel(i, j, Color.White);
                                continue;
                            }
                            if (!defaultButtonRectangle.Contains(temp))
                            {
                                if (!colorComparer.Compare(pixBase, pixTest))
                                {
                                    bChange = true;
                                    difference.SetPixel(i, j, Color.Black);
                                }
                                else
                                    difference.SetPixel(i, j, Color.White);
                            }
                            else if (!defaultButtonRectangleEx.Contains(temp))
                            {
                                // Ignor the pixels in this border.
                                //difference.SetPixel(i, j, Color.Orange);
                            }
                            else //Process the pixels in OK button rectangle
                            {
                                if (!colorComparer.Compare(pixBase, pixTest))
                                {
                                    int defaultBuffer = SUIColorComparer.buffer;
                                    //Temproraly set buffer to 150 since in this special area we need a larger buffer.
                                    SUIColorComparer.buffer = 150;
                                    if (colorComparer.Compare(pixBase, SystemColors.ControlText) || colorComparer.Compare(pixTest, SystemColors.ControlText))
                                    {
                                        bChange = true;
                                        difference.SetPixel(i, j, Color.Black);
                                    }
                                    //else
                                    //    difference.SetPixel(i, j, Color.Blue);
                                    // Set the default buffer back.
                                    SUIColorComparer.buffer = defaultBuffer;
                                }
                                //else
                                //    difference.SetPixel(i, j, Color.Red);
                            }
                        }
                    }

                    if (bChange)
                    {
                        windowImage.Path = baseImage.Path.Replace(".bmp", "_error.bmp");
                        windowImage.Save();
                        windowImage.ReleaseMem();
                        baseImage.ReleaseMem();
                        string path = baseImage.Path.Replace(".bmp", "_error_difference.bmp");
                        difference.Save(path);
                        difference.Dispose();
                        return false;
                    }
                    else
                    {
                        windowImage.ReleaseMem();
                        baseImage.ReleaseMem();
                        difference.Dispose();
                        return true;
                    }
                }
            }
            else
                return SUIComparer.GetComparer(ComparerType.Image).Compare(baseImage, windowImage);
        }

        public override bool Compare(object obj1, object obj2, CompareMode compareMode)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    internal class SUIColorComparer : SUIComparer
    {
        // We set 10 as default buffer to resolve errors.
        public static int buffer = 10;
        public override bool Compare(object obj1, object obj2)
        {
            bool result = false;
            if (obj1 != null && obj2 != null)
            {
                if (obj1 is Color && obj2 is Color)
                {
                    result = CompareColor((Color)obj1, (Color)obj2);
                }
            }
            return result;
        }

        protected bool CompareColor(Color c1, Color c2)
        {
            int A_C1 = c1.A, R_C1 = c1.R, G_C1 = c1.G, B_C1 = c1.B;

            int A_C2 = c2.A, R_C2 = c2.R, G_C2 = c2.G, B_C2 = c2.B;

            int A = Math.Abs(A_C1 - A_C2), R = Math.Abs(R_C1 - R_C2), G = Math.Abs(G_C1 - G_C2), B = Math.Abs(B_C1 - B_C2);
            int max = Math.Max(Math.Max(A, R), Math.Max(G, B));

            return (max <= buffer);
        }

        public override bool Compare(object obj1, object obj2, CompareMode compareMode)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class SUIHtmlComparer : SUIComparer
    {
        public override bool Compare(object obj1, object obj2,CompareMode compareMode)
        {
            if (obj1 == null || obj2 == null)
            {
                return false;
            }
            else
            {
                switch (compareMode)
                {
                    case CompareMode.HtmlVSFile:
                        return HtmlCompareFile(obj1.ToString(), obj2.ToString());
                    case CompareMode.HtmlVSImg:
                        return HtmlImgCompare(obj1.ToString(), obj2.ToString());
                    default:
                        return false;
                }
            }
        }

        public override bool Compare(object obj1, object obj2)
        {
            return HtmlCompareFile(obj1.ToString(), obj2.ToString());
        }

        private bool HtmlCompareFile(string html1, string filePath)
        {
            string html2 = SUIUtil.getStringFromTxtWithSave(filePath,html1);
            return PureHtmlCompare(html1, html2);
        }

        private bool PureHtmlCompare(string html1, string html2)
        {
            string filteredHtml1 = SUIUtil.convertHtml(html1);
            string filterenHtml2 = SUIUtil.convertHtml(html2);
            return (filteredHtml1.Equals(filterenHtml2));
        }

        private bool HtmlImgCompare(string html,string imgPath)
        {
            string temphtmlPath = SpecialDirectories.Temp+"\\SUITemp.html";
            if (File.Exists(imgPath))
            {
                SUIBitmap standardImg = SUIBitmap.LoadSUIBitmap(imgPath);
                if (standardImg == null)
                {
                    throw (new SUIHtmlCompareException());
                }
                try
                {
                    SUIUtil.OutputFileFromString(html, temphtmlPath);
                    SUIIE ie = SUIIE.CreateIEWithURL(temphtmlPath);
                    SUIBitmap tempImg = ie.GetClientArea();
                    // Fix a bug: we need to assign image path to the captured temp SUIBitmap object.
                    tempImg.Path = imgPath;
                    //SUIIE.CleanIEProcess();
                    SUIImageComparer Comparer = new SUIImageComparer();
                    return (Comparer.Compare(standardImg, tempImg));
                }catch(SUIException e)
                {
                    throw(new SUIHtmlCompareException(e));
                }
            }
            else
            {
                try
                {
                    SUIUtil.OutputFileFromString(html, temphtmlPath);
                    SUIIE ie = SUIIE.CreateIEWithURL(temphtmlPath);
                    SUIBitmap tempImg = ie.GetClientArea();
                    //SUIIE.CleanIEProcess();
                    tempImg.Path = imgPath;
                    tempImg.Save();
                    return true;
                }
                catch (SUIException e)
                {
                    throw (new SUIHtmlCompareException(e));
                }
            }
        }
    }
}
