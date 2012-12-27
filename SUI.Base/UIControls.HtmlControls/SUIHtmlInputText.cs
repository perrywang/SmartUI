using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

namespace SUI.Base.UIControls.HtmlControls
{
    public class SUIHtmlInputText : SUIHtmlControlBase
    {
        private IHTMLInputTextElement textElement;
        public SUIHtmlInputText(SUIHtmlDocument _doc, IHTMLElement _element)
            : base(_doc, _element)
        {
            textElement = (IHTMLInputTextElement)_element;
        }

        public SUIHtmlInputText(SUIHtmlControlBase ctrl)
            : base(ctrl)
        {
            textElement = (IHTMLInputTextElement)ctrl.HtmlElement;
        }

        public static bool IsInputTextElement(SUIHtmlControlBase ctrl)
        {
            bool itis = false;
            try
            {
                IHTMLInputElement input = ctrl.HtmlElement as IHTMLInputElement;
                if (input.type.ToLower() == "text" || input.type.ToLower() == "password")
                    itis = true;
            }
            catch
            {
                itis = false;
            }
            return itis;
        }

        public IHTMLInputTextElement IHTMLInputTextElement
        {
            get
            {
                return textElement;
            }
        }

        public string Text
        {
            get
            {
                return textElement.value;
            }
            set
            {
                textElement.value = value;
                textElement.select();
                object obj = value;
                HtmlElement3.FireEvent("onchange", ref obj);
            }
        }
    }
}
