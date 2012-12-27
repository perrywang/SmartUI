using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

namespace SUI.Base.UIControls.HtmlControls
{
    public class SUIHtmlDiv : SUIHtmlControlBase
    {
        private IHTMLDivElement divElement;
        public SUIHtmlDiv(SUIHtmlDocument _doc, IHTMLElement _element)
            : base(_doc, _element)
        {
            divElement = (IHTMLDivElement)_element;
        }

        public SUIHtmlDiv(SUIHtmlControlBase ctrl)
            : base(ctrl)
        {
            divElement = (IHTMLDivElement)ctrl.HtmlElement;
        }

        public static bool IsDivElement(SUIHtmlControlBase ctrl)
        {
            bool itis = false;
            try
            {
                IHTMLDivElement span = ctrl.HtmlElement as mshtml.IHTMLDivElement;
                itis = true;
            }
            catch
            {
                itis = false;
            }
            return itis;
        }

        public IHTMLDivElement IHTMLDivElement
        {
            get
            {
                return divElement;
            }
        }
    }
}
