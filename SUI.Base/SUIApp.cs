using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualBasic;
using SUI.Base.Win;
using System.IO;
using SUI.Base.Utility;

namespace SUI.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class SUIApp
    {
        private Process _process;
        public Process Process
        {
            get { return _process; }
            set { _process = value; }
        }

        private SUIWindow _mainWindow;
        public virtual SUIWindow MainWindow
        {
            get { return _mainWindow; }
            set { _mainWindow = value; }
        }

        private SUIAppParameters _parameters;
        public SUIAppParameters Parameters
        {
            get { return _parameters; }
        }

        public SUIApp()
        {
            _process = null;
        }

        public SUIApp(SUIAppParameters parameters)
        {
            _process = null;
            _parameters = parameters;
        }

        public SUIApp(string exePath)
        {
            _process = null;
            _parameters = new SUIAppParameters(exePath, null);
        }

        public virtual SUIWindow FindMainWindow()
        {
            FileInfo file = new FileInfo(_parameters.ExePath);
            string fileName = file.Name;
            string[] fileAndExtention = fileName.Split('.');
            if (fileAndExtention != null)
            {
                try
                {
                    Process[] processes = Process.GetProcessesByName(fileAndExtention[0]);
                    if (processes != null)
                    {
                        if (processes.Length > 0)
                        {
                            return new SUIWindow(processes[0].MainWindowHandle);
                            //throw new Exception("more than one application running.");
                        }                        
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("FindMainWindow method exception.");
                }

            }
            return null;
        }

        //By default, this index is 1;
        private double performanceIndex = 1;
        private double SuggestedStartTime = 10000; //for EIM testapplication 10 seconds is better choice
        public double PerformanceIndex
        {
            get
            {
                return performanceIndex;
            }
        }

        //By default, we will maximize the main window.
        public void Start()
        {
            Start(1);
        }

        //If needMax != 0, we will maxmize the main window.
        public void Start(int needMax)
        {
            if (_parameters == null)
                return;
            if (_process != null)
                return;

            DateTime startTime = DateTime.Now;

            string workingDirectory;
            if (_parameters.ExePath == null)
            {
                workingDirectory = _parameters.ExePath;
            }
            else
            {
                workingDirectory = _parameters.ExePath.Substring(0, Strings.InStrRev(_parameters.ExePath, @"\", -1, CompareMethod.Binary));
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = _parameters.ExePath;
            startInfo.Arguments = _parameters.Arguments;
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = false; //if vista, set it to true

            //start the process and get the process info
            _process = Process.Start(startInfo);

            if (_parameters.HasWindow)
            {
                int timeOut;
                _process.WaitForInputIdle(_parameters.TimeOut);
                _process.Refresh();

                timeOut = _parameters.TimeOut;
                do
                {
                    if (_process.MainWindowHandle != IntPtr.Zero)
                    {
                        break;
                    }
                    _process.Refresh();
                    timeOut -= 1000;
                    SUISleeper.Sleep(1000);

                }
                while (timeOut >= 0);

                if (_parameters.MainWindowClass != null && _parameters.MainWindowClass != string.Empty) // try to ignore the splash window
                {
                    //_mainWindow = SUIWindow.WaitingForWindow(para.MainWindowClass, para.TimeOut/1000);
                    timeOut = _parameters.TimeOut;
                    do
                    {
                        _process.Refresh();
                        SUIWindow tempWindow;
                        try
                        {
                            tempWindow = new SUIWindow(_process.MainWindowHandle);
                        }
                        catch (Exception e)
                        {
                            tempWindow = null;
                        }
                        if (tempWindow != null && tempWindow.ClassName.Contains(_parameters.MainWindowClass))
                        {
                            _mainWindow = tempWindow;
                            break;
                        }
                        timeOut -= 1000;
                        SUISleeper.Sleep(1000);
                    }
                    while (timeOut >= 0);
                }
                else
                {
                    //_mainWindow = SUIWindow.WaitingForWindow(para.MainWindowClass, para.TimeOut/1000);
                    timeOut = _parameters.TimeOut;
                    do
                    {
                        _process.Refresh();
                        SUIWindow tempWindow;
                        try
                        {
                            tempWindow = new SUIWindow(_process.MainWindowHandle);
                        }
                        catch (Exception e)
                        {
                            tempWindow = null;
                        }

                        if (tempWindow != null)
                        {
                            _mainWindow = tempWindow;
                            break;
                        }
                        timeOut -= 1000;
                        SUISleeper.Sleep(1000);
                    }
                    while (timeOut >= 0);
                }
            }
            DateTime endTime = DateTime.Now;
            CalculatePerformanceIndex(startTime, endTime);

            this.BringToForeground(needMax);
            SUISleeper.Sleep(2000);
            ClosePopupWindow();
        }

        public void ClosePopupWindow()
        {
            if (_mainWindow != null)
            {
                foreach(SUIWindow win in SUIWindow.DesktopWindow.Items)
                {
                    if (win.Parent != null && win.Parent.WindowHandle.Equals(_mainWindow.WindowHandle))
                    {
                        win.Close();
                    }
                }
            }
        }

        private void CalculatePerformanceIndex(DateTime startTime, DateTime endTime)
        {
            TimeSpan timeSpan = endTime.Subtract(startTime);
            if(timeSpan.TotalMilliseconds > SuggestedStartTime)
            {
                performanceIndex = timeSpan.TotalMilliseconds / SuggestedStartTime;
            }
        }

        //If needMax != 0, we will maxmize the main window.
        public virtual void BringToForeground(int needMax)
        {
            if (Process != null && MainWindow != null)
            {
                SUIWindow.SetForegroundWindow(MainWindow);
                if (needMax != 0)
                    MainWindow.Maximized = true;
                SUIWindow.MinimizeAllWindow(MainWindow);
            }
        }

        public virtual void Kill()
        {
            this.Process.Kill();
            this.Process.WaitForExit();
        }

        public virtual void Quit()
        {
            SUISleeper.Sleep(1000);
            MainWindow.CloseChildModalDialog();
            this.Process.CloseMainWindow();
        }

        public virtual void CloseMainWindow()
        {
            if (MainWindow != null)
                MainWindow.Close();
        }
    }

    /// <summary>
    /// SUIAppParameters
    /// </summary>
    public class SUIAppParameters
    {
        private string _exePath;
        public string ExePath
        {
            get { return _exePath; }
            set { _exePath = value; }
        }

        private string _arguments;
        public string Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        private bool _hasWindow;
        public bool HasWindow
        {
            get { return _hasWindow; }
            set { _hasWindow = value; }
        }

        private int _timeOut;
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        private string _mainWindowClass;
        public string MainWindowClass
        {
            get { return _mainWindowClass; }
            set { _mainWindowClass = value; }
        }

        public SUIAppParameters()
        {
            _hasWindow = true;
            _arguments = null;
            _exePath = null;
            _timeOut = 5000;
            _mainWindowClass = null;
        }

        public SUIAppParameters(string exePath, string arguments)
        {
            _hasWindow = true;
            _mainWindowClass = null;
            _timeOut = 5000;
            _arguments = arguments;
            _exePath = exePath;
        }
    }
}
