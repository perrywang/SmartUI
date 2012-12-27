using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagedSpy;
using System.Collections;
using System.Diagnostics;
using SUI.Base.Win;
using System.Runtime.InteropServices;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetControl : SUIWindow
    {
        protected static string lpString = "WM_GETCONTROLNAME";
        protected static int msg = SUIWinAPIs.RegisterWindowMessage(lpString);
        protected string SearchTarget = string.Empty;
        protected SUIDotNetControl targetChildCtrl = null;
        private IList<SUIDotNetControl> children = null;
        
        public SUIDotNetControl(IntPtr hWnd)
            :base(hWnd)
        {
           
        }
        
        public SUIDotNetControl(SUIWindow sui):this(sui.WindowHandle)
        {
        }

        private SUIDotNetControl(SUIDotNetControl ctrlProxy):this(ctrlProxy.WindowHandle)
        {
            
        }
        public static SUIDotNetControl FromHandle(IntPtr hWnd)
        {
            return new SUIDotNetControl(hWnd);
        }
        public static SUIDotNetControl FromSUIWindow(SUIWindow sui)
        {
            return FromHandle(sui.WindowHandle);
        }
        public SUIDotNetControl FindControl(string ctrID)
        {
            return SUIDotNetControl.FindControl(this, ctrID);
        }
        
        protected static SUIDotNetControl FindControl(SUIDotNetControl root, string ctrlID)
        {
            SUIDotNetControl rootCtrl = new SUIDotNetControl(root);
            return rootCtrl.FindChidDotNetControlByID(ctrlID);
        }

        public SUIDotNetControl FindChidDotNetControlByID(string ctrlID)
        {
            SearchTarget = ctrlID;
            SUIWinAPIs.EnumChildWindows(this.WindowHandle, new EnumSMARTUIWindowsProc(this.SearchCtrl), 0);
            return targetChildCtrl;
        }

        public SUIDotNetControl FindChildDotNetControlByCaption(string caption)
        {
            SearchTarget = caption;
            SUIWinAPIs.EnumChildWindows(this.WindowHandle, new EnumSMARTUIWindowsProc(this.SearchCtrl), 1);
            return targetChildCtrl;
        }

        public SUIDotNetControl FindChildDotNetControlByClassName(string className)
        {
            SearchTarget = className;
            SUIWinAPIs.EnumChildWindows(this.WindowHandle, new EnumSMARTUIWindowsProc(this.SearchCtrl), 2);
            return targetChildCtrl;
        }

        private int SearchCtrl(IntPtr hWnd, int lParam)
        {
            if (lParam == 0)
            {
                SUIDotNetControl temp = new SUIDotNetControl(hWnd);
                string id = temp.ID;
                
                //if (temp.ControlTypeName.IndexOf("ToolStrip") > -1)
                //{
                //    SUIDotNetToolStrip toolStrip = new SUIDotNetToolStrip(temp);
                //    id = toolStrip.ID;
                //}
                //else
                //{
                //    id = temp.ID;
                //}
                if (id.Equals(SearchTarget))
                {
                    targetChildCtrl = new SUIDotNetControl(hWnd);
                    return 0;
                }
                return 1;
            }
            else if (lParam == 1)
            {
                SUIDotNetControl temp = new SUIDotNetControl(hWnd);
                if (temp.WindowText.IndexOf(SearchTarget) > -1)
                {
                    targetChildCtrl = new SUIDotNetControl(hWnd);
                    return 0;
                }
                return 1;
            }
            else
            {
                SUIDotNetControl temp = new SUIDotNetControl(hWnd);
                if (temp.ClassName.IndexOf(SearchTarget) > -1)
                {
                    targetChildCtrl = new SUIDotNetControl(hWnd);
                    return 0;
                }
                return 1;
            }
        }

        public string ID
        {
            get
            {
               string id = GetDotNetControlID();
               if (id.Equals(string.Empty))
               {
                   try
                   {
                       ControlProxy proxy = ControlProxy.FromHandle(WindowHandle);
                       id = proxy.GetComponentName();
                   }
                   catch
                   {
                       
                   }

               }
               return id;
            }
        }

        public string ControlTypeName
        {
            get
            {
                try
                {
                    ControlProxy proxy = ControlProxy.FromHandle(WindowHandle);
                    return proxy.ComponentType.FullName;
                }
                catch
                {
                    return string.Empty;
                }

            }
        }

        private string GetDotNetControlID()
        {
            
            IntPtr ptr = IntPtr.Zero;
            IntPtr ptr4;
            IntPtr ptr6;
            int threadID = this.ThreadId.ToInt32();
            byte[] lpBuffer = new byte[0x10000];
            int num2 = 0x10000;
           
            IntPtr ptr3 = IntPtr.Zero;
            try
            {
                IntPtr ptr5 = IntPtr.Zero;
                ptr3 = SUIWinAPIs.OpenProcess(0x38, 0, this.OwningProcess.Id);
                ptr6 = new IntPtr(num2);
                ptr = SUIWinAPIs.VirtualAllocEx(ptr3, IntPtr.Zero, ptr6.ToInt32(), 0x3000, 4);
                ptr6 = new IntPtr(num2);
                ptr4 = new IntPtr(SUIWinAPIs.SendMessage(this.WindowHandle, msg, ptr6, ptr));
                ptr6 = new IntPtr(num2);
                if (SUIWinAPIs.ReadProcessMemory(ptr3, ptr, lpBuffer, ptr6.ToInt32(), out ptr5) == 0)
                {
                    throw new SUIException("Get DotNetControl ID failed.");
                }
            }
            catch (Exception exception1)
            {

            }
            finally
            {
                ptr6 = new IntPtr(0);
                try
                {
                    SUIWinAPIs.VirtualFreeEx(ptr3, ptr, ptr6.ToInt32(), 0x8000);
                }
                catch
                { }
                //if (!SUIWinAPIs.VirtualFreeEx(ptr3, ptr, ptr6.ToInt32(), 0x8000))
                //{
                //    throw new SUIException("Get DotNetControl ID failed.");
                //}
                SUIWinAPIs.CloseHandle(ptr3);
            }
            return Encoding.Unicode.GetString(lpBuffer).TrimEnd(new char[] { '\x00fd' }).TrimEnd(new char[] { '\xfdfd' }).TrimEnd(new char[] { '\xfffd' }).TrimEnd(new char[] { '\0' });
        }
   
       
        
     
        
       
       
        
      
       
      
    }
    
    
}
