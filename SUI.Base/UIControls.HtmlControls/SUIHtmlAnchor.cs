using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

namespace SUI.Base.UIControls.HtmlControls
{
    public class SUIHtmlAnchor : SUIHtmlControlBase
    {
        public SUIHtmlAnchor(SUIHtmlDocument _doc, IHTMLElement _element)
            : base(_doc, _element)
        { }

        public SUIHtmlAnchor(SUIHtmlControlBase ctrl)
            : base(ctrl)
        { }

        public IHTMLAnchorElement AnchorElement
        {
            get
            {
                return (IHTMLAnchorElement)HtmlElement;
            }
        }

        public static bool IsAnchorElement(SUIHtmlControlBase ctrl)
        {
            bool itis = false;
            try
            {
                IHTMLAnchorElement anchor = ctrl.HtmlElement as IHTMLAnchorElement;
                string href = anchor.href;
                itis = true;
            }
            catch
            {
                itis = false;
            }
            return itis;
        }

        public string Href
        {
            get
            {
                return AnchorElement.href;
            }
        }

        public string NameProp
        {
            get
            {
                return AnchorElement.nameProp;
            }
        }
    }
}
