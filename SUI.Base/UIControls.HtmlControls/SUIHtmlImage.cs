using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

namespace SUI.Base.UIControls.HtmlControls
{
    public class SUIHtmlImage : SUIHtmlControlBase
    {
        private IHTMLImgElement imageElement;
        public SUIHtmlImage(SUIHtmlDocument _doc, IHTMLElement _element)
            : base(_doc, _element)
        {
            imageElement = (IHTMLImgElement)_element;
        }

        public SUIHtmlImage(SUIHtmlControlBase ctrl)
            : base(ctrl)
        {
            imageElement = (IHTMLImgElement)ctrl.HtmlElement;
        }

        public static bool IsImageElement(SUIHtmlControlBase ctrl)
        {
            bool itis = false;
            try
            {
                IHTMLImgElement span = ctrl.HtmlElement as mshtml.IHTMLImgElement;
                itis = true;
            }
            catch
            {
                itis = false;
            }
            return itis;
        }

        public IHTMLImgElement IHTMLImgElement
        {
            get
            {
                return imageElement;
            }
        }   
    }
}
