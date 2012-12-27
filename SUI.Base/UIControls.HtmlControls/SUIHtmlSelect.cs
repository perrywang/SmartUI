using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

namespace SUI.Base.UIControls.HtmlControls
{
    public class SUIHtmlSelect: SUIHtmlControlBase
    {
        private IHTMLSelectElement selectElement;
        public SUIHtmlSelect(SUIHtmlDocument _doc, IHTMLElement _element)
            : base(_doc, _element)
        {
            selectElement = (IHTMLSelectElement)_element;
        }

        public SUIHtmlSelect(SUIHtmlControlBase ctrl)
            : base(ctrl)
        {
            selectElement = (IHTMLSelectElement)ctrl.HtmlElement;
        }

        public static bool IsSelectElement(SUIHtmlControlBase ctrl)
        {
            bool itis = false;
            try
            {
                IHTMLSelectElement select = ctrl.HtmlElement as IHTMLSelectElement;
                int size = select.size;
                itis = true;
            }
            catch
            {
                itis = false;
            }
            return itis;
        }

        public IHTMLSelectElement IHTMLSelectElement
        {
            get
            {
                return selectElement;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return selectElement.selectedIndex;
            }
            set
            {
                selectElement.selectedIndex = value;              
                object obj = value;
                HtmlElement3.FireEvent("onchange", ref obj);               
            }
        }

        public int Length
        {
            get{
                return selectElement.length;
            }
        }

        public string GetTextByIndex(int index)
        {
            if (index >= Length || index < 0)
                throw new SUIException("Index is out of range!");

            IHTMLOptionElement option = (IHTMLOptionElement)selectElement.item(index, index);
            return option.text;
        }

        public string GetValueByIndex(int index)
        {
            if (index >= Length || index < 0)
                throw new SUIException("Index is out of range!");

            IHTMLOptionElement option = (IHTMLOptionElement)selectElement.item(index, index);
            return option.value;
        }

        public string Value
        {
            set
            {
                selectElement.value = value;
                object obj = value;
                HtmlElement3.FireEvent("onchange", ref obj);
            }
        }
    }
}
