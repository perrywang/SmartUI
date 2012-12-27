using System;
using System.Collections.Generic;
using System.Text;
using mshtml;
using System.Collections;
using EnvDTE;

namespace SUI.Base.UIControls.HtmlControls
{
    public class SUIHtmlControlBase
    {
        private SUIHtmlDocument doc;
        protected IHTMLElement element;
        public SUIHtmlControlBase(SUIHtmlDocument _doc, IHTMLElement _element)
        {
            doc = _doc;
            element = _element;
        }

        public SUIHtmlControlBase(SUIHtmlControlBase ctrl)
        {
            doc = ctrl.HtmlDocument;
            element = ctrl.HtmlElement;
        }

        public SUIHtmlDocument HtmlDocument
        {
            get
            {
                return doc;
            }
        }

        public IHTMLElement HtmlElement
        {
            get
            {
                return element;
            }
        }

        public IHTMLElement3 HtmlElement3
        {
            get
            {
                return (IHTMLElement3)element;
            }
        }

        public void Click()
        {
            try
            {
                element.click();
            }
            catch (Exception e)
            {
                if (!(this is SUIHtmlAnchor))//Ignore anchor exception
                    throw;
            }
        }

        public void Click(bool mayPopupDlg)
        {
            //If it may popup a dialog to block the execution of current thread, we need to 
            // try clicking in a seperate thread.
            if (mayPopupDlg)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(Click));
                t.Priority = System.Threading.ThreadPriority.Highest;
                t.Start();
                t.Join(1000);
            }
            else
            {
                Click();
            }
        }

        public SUIHtmlControlCollection Children
        {
            get
            {
                SUIHtmlControlCollection collection = new SUIHtmlControlCollection();
                IHTMLElementCollection children = (IHTMLElementCollection)element.children;
                foreach (IHTMLElement e in children)
                {
                    collection.Add(new SUIHtmlControlBase(doc, e));
                }
                return collection;
            }
        }

        public SUIHtmlControlBase FindChildByIndex(int index)
        {
            IHTMLElementCollection children = (IHTMLElementCollection)element.children;
            IHTMLElement e = (IHTMLElement)children.item(null, index);

            return new SUIHtmlControlBase(doc, e);
        }

        public SUIHtmlControlBase FindChildByName(string name)
        {
            IHTMLElementCollection children = (IHTMLElementCollection)element.children;
            IHTMLElement e = (IHTMLElement)children.item(name, null);

            return new SUIHtmlControlBase(doc, e);
        }

        public object GetAttributeValue(string attrName)
        {
            //case insensitive.
            object obj = element.getAttribute(attrName, 0);
            return obj;
        }

        public string OuterHtml
        {
            get
            {
                return HtmlElement.outerHTML;
            }
        }

        public string InnerText
        {
            get
            {
                return element.innerText;
            }
        }

        public string InnerHtml
        {
            get
            {
                return element.innerHTML;
            }
        }

        public string TagName
        {
            get
            {
                return element.tagName;
            }
        }

        public string ID
        {
            get
            {
                return element.id;
            }
        }

        public string Name
        {
            get
            {
                return (string)GetAttributeValue("name");
            }
        }

        public SUIHtmlControlBase ParentElement
        {
            get
            {
                return new SUIHtmlControlBase(doc, HtmlElement.parentElement);
            }
        }
    }

    public class SUIHtmlControlCollection : ReadOnlyCollectionBase
    {
        public void Add(SUIHtmlControlBase ctl)
        {
            this.InnerList.Add(ctl);
        }

        public SUIHtmlControlBase this[int index]
        {
            get
            {
                return (SUIHtmlControlBase)this.InnerList[index];
            }
        }
    } 
}
