using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SUI.Base.Win
{
    public class SUISleeper
    {
        public static void Sleep(int miliseconds)
        {
            Application.DoEvents();
            Thread.Sleep(miliseconds);
        }
    }
}
