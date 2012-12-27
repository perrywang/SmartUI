using System;
using System.Collections.Generic;
using System.Text;

namespace SUI.Base.Flex
{
    public class FlashPlayerWindow : SUIWindow
    {
        private int _xOffside = 4;
        public int XOffSide
        {
            get
            {
                return _xOffside;
            }
        }

        private int _yOffside = 42;
        public int YOffSide
        {
            get
            {
                return _yOffside;
            }
        }

        public void SetStageOffSide(int xf, int yf)
        {
            _xOffside = xf;
            _yOffside = yf;
        }

        public FlashPlayerWindow(IntPtr mainWinHandle)
            : base(mainWinHandle)
        {
        }
    }
}
