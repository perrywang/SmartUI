using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using SUI.Base.SUIExceptions;
using System.Drawing.Imaging;
namespace SUI.Base.Utility
{
    public class SUIBitmap
    {
        public const string FileSuffix = ".bmp";
        private Bitmap bitmap;
        private string path;
        public string Path
        {
            get 
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }
        public Bitmap Bitmap
        {
            get
            {
                return this.bitmap;
            }
            set
            {
                this.bitmap = value;
            }
        }
        public bool IsNullBitmap
        {
            get
            {
                return (this.bitmap == null);
            }
        }
        public SUIBitmap(Bitmap bitmap,string path)
        {
            this.bitmap = bitmap;
            this.path = path;
        }
        public SUIBitmap(SUIBitmap suiBitmap)
        {
            this.bitmap = suiBitmap.bitmap;
            this.path = suiBitmap.path;
        }
        public void ReleaseMem()
        {
            if(bitmap != null)
                try
                {
                    bitmap.Dispose();
                }
                catch (Exception e)
                {
                    throw new SUIException("SUIBitmap release memory exception! ");
                }
        }
        public static SUIBitmap LoadSUIBitmap(string path)
        {
            try
            {
                Bitmap map = null;
                if (File.Exists(path))
                    map = new Bitmap(path);
                return new SUIBitmap(map, path);
            }
            catch (Exception e)
            {
                throw new SUILoadBitmapFailException("Load Bitmap Failed!",e);
            }
        }
        public static void SaveSUIBitmap(SUIBitmap suiBitmap)
        {
            suiBitmap.Save();
        }
        public void Save()
        {
            if(IsNullBitmap)
                throw new SUISaveBitmapFailException("Cannnot save null Bitmap!");
            string imageDirectory = FileSystem.GetParentPath(path);
            try
            {
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }
                Bitmap temp = new Bitmap(bitmap);
                bitmap.Dispose();
                temp.Save(path, ImageFormat);
            }
            catch (Exception e)
            {
                throw new SUISaveBitmapFailException("Save Bitmap Failed!",e);
            }
        }

        public void Resize(int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(bitmap, 0, 0, width, height);
            bitmap = result;
        }

        public void Cut(int x, int y, int width, int height)
        {
            if (x < 0 || y < 0 || (x + width) > bitmap.Width || (y + height) > bitmap.Height)
                throw new Exception("Fail to cut bitmap: invalid rectangle information.");

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(bitmap, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            bitmap = result;
        }

        public ImageFormat ImageFormat
        {
            get
            {
                ImageFormat format = ImageFormat.Bmp;
                if (path != null)
                {
                    if (path.ToLower().EndsWith(".bmp"))
                    {
                        format = ImageFormat.Bmp;
                    }
                    else if (path.ToLower().EndsWith(".jpeg") || path.ToLower().EndsWith(".jpg"))
                    {
                        format = ImageFormat.Jpeg;
                    }
                    else if (path.ToLower().EndsWith(".gif"))
                    {
                        format = ImageFormat.Gif;
                    }
                    else if (path.ToLower().EndsWith(".png"))
                    {
                        format = ImageFormat.Png;
                    }
                    else if (path.ToLower().EndsWith(".ico"))
                    {
                        format = ImageFormat.Icon;
                    }
                }
                return format;
            }
        }
    }
}
