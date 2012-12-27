using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace SUI.Base.Win
{
    public class SUIProcess
    {
        private SUIWindow mainWindow;
        private Process p;
        public SUIProcess(Process _p)
        {
            p = _p;
        }

        public static void InitializeSMARTUI(SUIWindow sui)
        {
            int proId = 0;
            IntPtr winThread = SUIWinAPIs.GetWindowThreadProcessId(sui.WindowHandle, ref proId);
            IntPtr curThread = SUIWinAPIs.GetCurrentThreadId();
            SUIWinAPIs.AttachThreadInput(curThread, winThread, 1);
        }

        //public SUIWindow StartApplication(string exepath, string title)
        //{
        //    Process p = new Process();
        //    p.StartInfo.FileName = exepath;
        //    p.Start();

        //    Thread.Sleep(1000);
        //    SUIWindow desktop = SUIWindow.DesktopWindow;
        //    SUIWindow sui = SUIWindow.FindSMARTUIWindow(desktop, null, title);

        //    SUIProcess.InitializeSMARTUI(sui);
        //    sui.Maximized = true;
        //    return sui;
        //}

        public SUIWindow FindMainWindowByID(int processId)
        {
            SUIWinAPIs.EnumWindows(new SUI.Base.SUIWindow.EnumSMARTUIWindowsProc(FindTargetWindow), processId);
            return mainWindow;
        }
        public int FindTargetWindow(IntPtr HWND, int lpram)
        {
            int processId = 0;
            SUIWinAPIs.GetWindowThreadProcessId(HWND, ref processId);
            if (processId == lpram)
            {
                if (SUIWinAPIs.IsWindowVisible(HWND))
                {
                    mainWindow = new SUIWindow(HWND);
                    return 0;
                }
            }
            return 1;

        }
    }
}
