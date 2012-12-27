using System;
using System.Collections.Generic;
using System.Text;
using Selenium;

namespace SUI.Base.SeleniumProxy
{
    public class SUISeleniumProxy : DefaultSelenium
    {
        public static string DefaultServer = "localhost";
        public static int DefaultPort = 4444;
        public static string DefaultBrowserName = "*iexplore";
        public static string DefaultInitialURL = "http://www.google.com";
        public SUISeleniumProxy(string server, int port, string browserName, string initialURL)
            : base(server, port, browserName, initialURL)
        { }
        public SUISeleniumProxy(string browserName)
            : this(DefaultServer, DefaultPort, browserName, DefaultInitialURL)
        { }
        public SUISeleniumProxy(string browserName, string initialURL)
            : this(DefaultServer, DefaultPort, browserName, initialURL)
        { }

        public SUISeleniumProxy()
            : this(DefaultServer, DefaultPort, DefaultBrowserName, DefaultInitialURL)
        { }

        ~SUISeleniumProxy()
        {
            try
            {
                Stop();
            }
            catch
            {}
        }

    }
}
