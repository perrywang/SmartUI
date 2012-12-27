using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

namespace SUI.Base.UIControls.HtmlControls
{
    public class SUIHtmlInputButton : SUIHtmlControlBase
    {
        private IHTMLInputButtonElement buttonElement;
        public SUIHtmlInputButton(SUIHtmlDocument _doc, IHTMLElement _element)
            : base(_doc, _element)
        {
            buttonElement = (IHTMLInputButtonElement)_element;
        }

        public SUIHtmlInputButton(SUIHtmlControlBase ctrl)
            : base(ctrl)
        {
            buttonElement = (IHTMLInputButtonElement)ctrl.HtmlElement;
        }

        public static bool IsInputButtonElement(SUIHtmlControlBase ctrl)
        {
            bool itis = false;
            try
            {
                IHTMLInputButtonElement input = ctrl.HtmlElement as IHTMLInputButtonElement;
                string value = input.value;
                itis = true;
            }
            catch
            {
                itis = false;
            }
            return itis;
        }

        public IHTMLInputButtonElement IHTMLInputButtonElement
        {
            get
            {
                return buttonElement;
            }
        }
    }
}
