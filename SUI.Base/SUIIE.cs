using System;
using System.Collections.Generic;
using System.Text;
using SHDocVw;
using Shell32;
using Microsoft.VisualBasic;
using SUI.Base.Win;
using SUI.Base.Utility;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Drawing;
using mshtml;
namespace SUI.Base
{
    public class SUIIE
    {
        public static string DefaultCaption = "Untitled Page";
        public static string WebServerProcessName = "WebDev.WebServer";
        public static string IEProcessName = "iexplore";
        public static string IEProcess = "iexplore.exe";
        public static int Version = RetrieveVersion();
        private static int RetrieveVersion()
        {
            RegistryKey pregkey;
            pregkey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer", false);
            string version = (string)pregkey.GetValue("Version");
            pregkey.Close();
            string[] ver = version.Split('.');
            return int.Parse(ver[0]);
        }
        public static string IETitleSuffix = Version == 6 ? " - Microsoft Internet Explorer" : " - Windows Internet Explorer";

        private IWebBrowser2 ie;
        private SUIWindow ieWin;
        private SUIIE(IWebBrowser2 instance)
        {
            this.ie = instance;
        }
        public SUIWindow IEWin
        {
            get
            {
                return this.ieWin;
            }
            set
            {
                this.ieWin = value;
            }
        }

        private bool IsLoadCompleted
        {
            get
            {
                if (ie.ReadyState == tagREADYSTATE.READYSTATE_COMPLETE)
                {
                    return true;
                }
                return false;
            }
        }

        // Comment out this method and we have an alternative solution.
        //private static SUIIE GetIE()
        //{
        //    Object o = Interaction.CreateObject("Shell.Application.1", null);
        //    IShellDispatch4 shell = (IShellDispatch4)o;
        //    IShellWindows windows = (IShellWindows)shell.Windows();
        //    for (int i = 0; i < windows.Count; i++)
        //    {
        //        Object obj = windows.Item(i);

        //        if (Information.TypeName(obj).Equals("IWebBrowser2"))
        //        {
        //            IWebBrowser2 browser = (IWebBrowser2)obj;
        //            SUIWindow ieWin = new SUIWindow(new IntPtr(browser.HWND));
        //            if (ieWin.WindowText.Contains(IETitleSuffix))
        //            {
        //                SUIIE instance = new SUIIE(browser);
        //                instance.ieWin = ieWin;
        //                return instance;
        //            }
        //        }
        //    }
        //    return null;
        //}  

        public static SUIIE WaitingForIEWindow(string IETitle)
        {
            // Interestingly, we cannot catch IE7 window with below logic on Vista
            // without Administrative previlege.
            ShellWindows windows = new ShellWindowsClass();
            int count = windows.Count;

            InternetExplorer myIE = null;
            while (myIE == null)
            {
                SUISleeper.Sleep(1000);
                if (windows.Count >= count + 1)
                {
                    foreach (InternetExplorer tmpIE in windows)
                    {
                        if (tmpIE.FullName.EndsWith(IEProcess, true, null))
                        {
                            myIE = tmpIE;
                        }
                    }
                }
            }

            SUIIE ie = new SUIIE(myIE);
            ie.ieWin = new SUIWindow(new IntPtr(myIE.HWND));

            if (ie.WaitingForLoadComplete())
            {
                //If the specified title is a null string, we will return the default IE window we find.
                if (IETitle == null || ie.IEWin.WindowText.Equals(IETitle + SUIIE.IETitleSuffix))
                {
                    ie.IEWin.Maximized = true;
                    return ie;
                }
            }
            return null;
        }

        public bool WaitingForLoadComplete()
        {
            int checksequence = 60;
            while (true)
            {
                SUISleeper.Sleep(2000);
                if (ie.ReadyState == tagREADYSTATE.READYSTATE_COMPLETE)
                    return true;
                if (checksequence > 0)
                {
                    checksequence--;
                }
                else
                {
                    return false;
                }
            }
        }
        public SUIBitmap GetClientArea()
        {
            IEWin.BringToTopMost();
            SUIWindow client = null;
            if (SUIIE.Version == 6)
            {
                client = IEWin.FindChildWindow("Shell DocObject View", string.Empty);
            }
            else
            {
                SUIWindow temp = IEWin.FindChildWindow("TabWindowClass", IEWin.WindowText);
                client = temp.FindChildWindow("Shell DocObject View", string.Empty);

            }
            if (client != null)
            {
                SUIBitmap clientImage = SUIImage.GetImageFromWindow(client);
                return clientImage;
            }
            return null;
        }

        public SUIWindow GetClientWindow()
        {
            IEWin.BringToTopMost();
            SUIWindow client = null;
            if (SUIIE.Version == 6)
            {
                client = IEWin.FindChildWindow("Shell DocObject View", string.Empty);
            }
            else
            {
                SUIWindow temp = IEWin.FindChildWindow("TabWindowClass", IEWin.WindowText);
                client = temp.FindChildWindow("Shell DocObject View", string.Empty);
            }
            return client;
        }

        public static void CleanIEProcess()
        {
            //Try to kill all IE processes.
            // If there'are any exceptions, try the second time.
            foreach (Process p in Process.GetProcessesByName(IEProcessName))
            {
                try
                {
                    p.Kill();
                }
                catch
                {
                    try
                    {
                        p.Kill();
                    }
                    catch { }
                }
            }

            //Try to kill all WebServer processes.
            // If there'are any exceptions, try the second time.
            foreach (Process p in Process.GetProcessesByName(WebServerProcessName))
            {
                try
                {
                    p.Kill();
                }
                catch
                {
                    try
                    {
                        p.Kill();
                    }
                    catch { }
                }
            }
        }

        public static SUIIE CreateIEWithURL(string URL)
        {
            InternetExplorer ie = new InternetExplorerClass();
            ie.Visible = true;
            SUIWindow ieWin = new SUIWindow(new IntPtr(ie.HWND));
            SUIIE instance = new SUIIE(ie);
            instance.ieWin = ieWin;
            Object obj = new Object();
            ie.Navigate(URL, ref obj, ref obj, ref obj, ref obj);
            if (instance.WaitingForLoadComplete())
            {
                instance.IEWin.Maximized = true;
                return instance;
            }
            return null;
        }
        public string GetHTMLResource()
        {
            try
            {
                return ((IHTMLDocument2)this.ie.Document).body.innerHTML;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public void Quit()
        {
            ie.Quit();
        }

        public void Close()
        {
            IEWin.Close();
        }
    }
}
