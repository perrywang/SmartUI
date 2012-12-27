using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Runtime.InteropServices;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Threading;
namespace SUI.Base.Resource
{
    public abstract class SUIStringResourceHandler
    {
        private StringResourceType m_stringType;
        public abstract string ExtractString(SUIStringResource resourceToken);
        public SUIStringResourceHandler(StringResourceType stringType)
        {
            m_stringType = stringType;
        }
        public StringResourceType StringType
        {
            get
            {
                return m_stringType;
            }
        }
    }
    
#region SUIWin32DialogStringHandler
    public class SUIWin32DialogStringHandler : SUIStringResourceHandler
    {
        //two inner class 
        [StructLayout(LayoutKind.Sequential, Pack = 2)]//extend dlg memory structure,2 byte align
        private class DialogTemplateEx                          //no use only sure memory structure
        {
            public short wDlgVer;
            public short wSignature;
            public int dwHelpID;
            public int dwExStyle;
            public int dwStyle;
            public short cDlgItems;
            public short x;
            public short y;
            public short cx;
            public short cy;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 2)]//standard dlg memory structure,2 byte align
        class DialogTemplate
        {
            public int style;
            public int dwExtendedStyle;
            public short cdit;
            public short x;
            public short y;
            public short cx;
            public short cy;
        }
        public SUIWin32DialogStringHandler()
            : base(StringResourceType.Win32DialogString)
        { }
        internal static void Register(SUIResourceManager manager)
        {
            manager.RegisterResourceHandler(new SUIWin32DialogStringHandler() );
        }
        public override string ExtractString(SUIStringResource resourceToken)
        {
            IntPtr hInstance = IntPtr.Zero;
            IntPtr hModule = IntPtr.Zero;
            try
            {
                hInstance = SUIWinAPIs.LoadLibraryEx(resourceToken.ResourceDll, IntPtr.Zero, 2);
               
                if(resourceToken.StringID is int)
                {
                    hModule = SUIWinAPIs.FindResource(hInstance, (int)resourceToken.StringID, (int)resourceToken.StringType);
                }
                else
                {
                    hModule = SUIWinAPIs.FindResource(hInstance, (string)resourceToken.StringID,(int)resourceToken.StringType);
                }
                IntPtr hResData = SUIWinAPIs.LoadResource(hInstance, hModule);
                IntPtr ptr = SUIWinAPIs.LockResource(hResData);
                DialogTemplateEx dlgtemplateex = new DialogTemplateEx();
                DialogTemplate dlgtemplate = new DialogTemplate();
                Marshal.PtrToStructure(ptr, dlgtemplateex);
                Marshal.PtrToStructure(ptr, dlgtemplate);
                if ((dlgtemplateex.wSignature & 0xffff) == 0xffff) //if dlg is extend dlg
                {
                    ptr = (IntPtr)(ptr.ToInt64() + Marshal.SizeOf(dlgtemplateex)); //directly operation memory address
                }
                else
                {
                    ptr = (IntPtr)(ptr.ToInt64() + 0x12);
                }
                if (Marshal.ReadInt16(ptr) == -1)
                {
                    ptr = (IntPtr)(ptr.ToInt64() + 4);
                }
                else
                {
                    while (Marshal.ReadInt16(ptr) != 0)
                    {
                        ptr = (IntPtr)(ptr.ToInt64() + 2);
                    }
                    ptr = (IntPtr)(ptr.ToInt64() + 2);
                }
                if (Marshal.ReadInt16(ptr) == -1)
                {
                    ptr = (IntPtr)(ptr.ToInt64() + 4);
                }
                else
                {
                    while (Marshal.ReadInt16(ptr) != 0)
                    {
                        ptr = (IntPtr)(ptr.ToInt64() + 2);
                    }
                    ptr = (IntPtr)(ptr.ToInt64() + 2);
                }
                return Marshal.PtrToStringAuto(ptr);
            }
            catch (Exception e)
            {
                throw new SUIException("ExactString Error",e);
            }
            finally
            {
                if (!hInstance.Equals(IntPtr.Zero))
                    SUIWinAPIs.FreeLibrary(hInstance);
            }
        }
    }
#endregion
#region SUIWin32StringHandler
    public class SUIWin32StringHandler : SUIStringResourceHandler
    {
         public SUIWin32StringHandler()
            : base(StringResourceType.Win32String)
        { }
        internal static void Register(SUIResourceManager manager)
        {
            manager.RegisterResourceHandler(new SUIWin32StringHandler());
        }
        public override string ExtractString(SUIStringResource resourceToken)
        {
            IntPtr hInstance = IntPtr.Zero;
            try
            {
                hInstance = SUIWinAPIs.LoadLibraryEx(resourceToken.ResourceDll, IntPtr.Zero, 2);
                StringBuilder result = new StringBuilder(255);
                SUIWinAPIs.LoadString(hInstance, (int)resourceToken.StringID, result, result.Capacity);
                return result.ToString();
            }
            catch (Exception e)
            {
                throw new SUIException("ExactString Error!",e);
            }
            finally
            {
                if (!hInstance.Equals(IntPtr.Zero))
                    SUIWinAPIs.FreeLibrary(hInstance);
            }
        }
    }
#endregion
#region ManagedString
    public class SUIManagedStringHandler : SUIStringResourceHandler
    {
        public SUIManagedStringHandler()
            : base(StringResourceType.ManagedString)
        { }
        internal static void Register(SUIResourceManager manager)
        {
            manager.RegisterResourceHandler(new SUIManagedStringHandler());
        }
        public override string ExtractString(SUIStringResource resourceToken)
        {
            string baseName = (string)resourceToken.ManagedAssembly[resourceToken.ResourceDll];
            Assembly assembly = Assembly.LoadFrom(resourceToken.ResourceDll);
            ResourceManager manager = new ResourceManager(baseName, assembly);
            return manager.GetString((string)resourceToken.StringID);
        }
    }
    
#endregion
}
