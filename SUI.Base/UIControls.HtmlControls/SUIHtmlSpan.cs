using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

namespace SUI.Base.UIControls.HtmlControls
{
    public class SUIHtmlSpan : SUIHtmlControlBase
    {
        private IHTMLSpanElement spanElement;
        public SUIHtmlSpan(SUIHtmlDocument _doc, IHTMLElement _element)
            : base(_doc, _element)
        {
            spanElement = (IHTMLSpanElement)_element;
        }

        public SUIHtmlSpan(SUIHtmlControlBase ctrl)
            : base(ctrl)
        {
            spanElement = (IHTMLSpanElement)ctrl.HtmlElement;
        }

        public static bool IsSpanElement(SUIHtmlControlBase ctrl)
        {
            bool itis = false;
            try
            {
                IHTMLSpanElement span = ctrl.HtmlElement as mshtml.IHTMLSpanElement;
                itis = true;
            }
            catch
            {
                itis = false;
            }
            return itis;
        }

        public IHTMLSpanElement IHTMLSpanElement
        {
            get
            {
                return spanElement;
            }
        }

        //public string Text
        //{
        //    get
        //    {
        //        return textElement.value;
        //    }
        //    set
        //    {
        //        textElement.value = value;
        //        object obj = value;
        //        HtmlElement3.FireEvent("onchange", ref obj);
        //    }
        //}
    }
}
