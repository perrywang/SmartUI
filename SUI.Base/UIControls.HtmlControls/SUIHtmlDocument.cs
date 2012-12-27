using System;
using System.Collections.Generic;
using System.Text;
using mshtml;
using SUI.Base.Win;
using System.Collections;
using System.Runtime.InteropServices;
using SHDocVw;
using SUI.Base.Utility;
using System.Text.RegularExpressions;

namespace SUI.Base.UIControls.HtmlControls
{
    public class SUIHtmlDocument
    {
        private IHTMLDocument2 doc;
        private SUIWindow internetServer = null;
        private SUIHtmlDocument(IHTMLDocument2 _doc)
        {
            doc = _doc;
        }

        public static SUIHtmlDocument GetSUIHtmlDocumentFromWindow(SUIWindow window)
        {
            IHTMLDocument2 doc = GetIEDocumentFromWindow(window);
            SUIHtmlDocument SUIDoc = new SUIHtmlDocument(doc);
            SUIDoc.InternetServer = window;
            return SUIDoc;
        }

        public static SUIHtmlDocument FindEmbeddedHtmlDocument(SUIWindow window)
        {
            SUIWindow docWin = null;
            //We need to find the window whose class name is "Internet Explorer_Server".
            foreach (SUIWindow win in window.Children)
            {
                if (win.ClassName.Equals("Internet Explorer_Server"))
                {
                    docWin = win;
                    break;
                }
            }

            if (docWin == null)
            {
                throw new SUIException("Fail to find embedded html document!");
            }

            return GetSUIHtmlDocumentFromWindow(docWin);
        }

        // Gets the Internet Explorer IHTMLDocument2 object for the given
        // IE Server control window handle
        // The class name of this window should be "Internet Explorer_Server"
        private static IHTMLDocument2 GetIEDocumentFromWindow(SUIWindow window)
        {
            if (!window.ClassName.Equals("Internet Explorer_Server"))
                throw new SUIException("Unable to get IHTMLDocument object from this window!");

            IntPtr lResult;
            int lMsg;
            IHTMLDocument2 htmlDocument = null;

            if (window.WindowHandle != IntPtr.Zero)
            {
                // Register the WM_HTML_GETOBJECT message so it can be used
                // to communicate with the Internet Explorer instance
                lMsg = SUIWinAPIs.RegisterWindowMessage("WM_HTML_GETOBJECT");
                // Sends the above registered message to the IE window and
                // waits for it to process it
                SUIWinAPIs.SendMessageTimeout(window.WindowHandle, lMsg, IntPtr.Zero, IntPtr.Zero, SUIWinAPIs.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out lResult);
                if (lResult != IntPtr.Zero)
                {
                    // Casts the value returned by the IE window into 
                    //an IHTMLDocument2 interface
                    htmlDocument = SUIWinAPIs.ObjectFromLresult(lResult, typeof(IHTMLDocument).GUID, IntPtr.Zero) as IHTMLDocument2;
                }
                if (htmlDocument == null)
                {
                    throw new SUIException("Unable to get IHTMLDocument object from this window!");
                }
            }
            return htmlDocument;
        }

        public SUIHtmlControlBase FindElementByID(string id)
        {
            IHTMLElementCollection all = (IHTMLElementCollection)doc.all;
            IHTMLElement e = (IHTMLElement)all.item(id, null);

            if (e == null)
            {
                //Try to find element in frames.
                foreach (SUIHtmlDocument frame in Frames)
                {
                    IHTMLElementCollection allInFrame = (IHTMLElementCollection)frame.doc.all;
                    IHTMLElement eInFrame = (IHTMLElement)allInFrame.item(id, null);
                    if (eInFrame != null)
                    {
                        e = eInFrame;
                        break;
                    }
                }
                if (e == null)
                    return null;
            }

            return new SUIHtmlControlBase(this, e);
        }

        public IHTMLDocument2 IHTMLDocument2
        {
            get
            {
                return doc;
            }
        }

        public IHTMLDocument3 IHTMLDocument3
        {
            get
            {
                return (IHTMLDocument3)doc;
            }
        }

        public SUIHtmlControlBase ElementFromPoint(int x, int y)
        {
            SUIHtmlControlBase ctrl = null;
            IHTMLElement element = this.IHTMLDocument2.elementFromPoint(x, y);
            if (element != null)
                ctrl = new SUIHtmlControlBase(this, element);

            if (ctrl.TagName != null && ctrl.TagName.Equals("FRAME"))
            {
                SUIHtmlDocument frame = GetFrame(ctrl);
                //TODO: implement the logic for embedding frames.
                //ctrl = frame.ElementFromPoint(x, y);
            }

            return ctrl;
        }

        public List<SUIHtmlControlBase> GetElementsByTagName(string tagName)
        {
            List<SUIHtmlControlBase> list = new List<SUIHtmlControlBase>();
            foreach (IHTMLElement e in IHTMLDocument3.getElementsByTagName(tagName))
            {
                SUIHtmlControlBase ctrl = new SUIHtmlControlBase(this, e);
                list.Add(ctrl);
            }
            //Get elements in frames.
            foreach (SUIHtmlDocument frame in Frames)
            {
                foreach (IHTMLElement eInFrame in frame.IHTMLDocument3.getElementsByTagName(tagName))
                {
                    SUIHtmlControlBase ctrl = new SUIHtmlControlBase(this, eInFrame);
                    list.Add(ctrl);
                }
            }
            return list;
        }

        //To determine whether this html document is a frameset which contains one or more frames.
        public bool IsFrameSet
        {
            get
            {
                bool bRet = true;

                IHTMLElement elem = doc.body;
                if (elem != null)
                {
                    //QI for IHtmlBodyElement
                    IntPtr pbody = Marshal.GetIUnknownForObject(elem);
                    IntPtr pbodyelem = IntPtr.Zero;
                    if (pbody != IntPtr.Zero)
                    {
                        int iRet = Marshal.QueryInterface(pbody,
                            ref SUICOMInterops.IID_IHTMLBodyElement, out pbodyelem);
                        Marshal.Release(pbody);
                        if (pbodyelem != IntPtr.Zero)
                        {
                            bRet = false;
                            Marshal.Release(pbodyelem);
                        }
                    }
                }

                return bRet;
            }
        }

        public SUIHtmlDocument GetFrame(SUIHtmlControlBase frameControl)
        {
            SUIHtmlDocument frame = null;
            if (frameControl.TagName != null && frameControl.TagName.Equals("FRAME"))
            {
                int index = 0;
                List<SUIHtmlControlBase> frameTags = GetElementsByTagName("Frame");
                if (frameTags.Count != 0)
                {
                    foreach (SUIHtmlControlBase ctrl in frameTags)
                    {
                        if (frameControl.Name.Equals(ctrl.Name))
                            break;
                        index++;
                    }
                    List<SUIHtmlDocument> frames = Frames;
                    if (index != frameTags.Count && index < frames.Count)
                    {
                        frame = frames[index];
                    }
                }
            }

            return frame;
        }

        public List<SUIHtmlDocument> Frames
        {
            get
            {
                List<SUIHtmlDocument> list = new List<SUIHtmlDocument>();

                try
                {
                    SUICOMInterops.IOleContainer oc = doc as SUICOMInterops.IOleContainer;
                    if (oc == null)
                        return null;

                    //get the OLE enumerator for the embedded objects
                    int hr = 0;
                    SUICOMInterops.IEnumUnknown eu;

                    oc.EnumObjects(SUICOMInterops.tagOLECONTF.OLECONTF_EMBEDDINGS, out eu);

                    object pUnk = null;
                    int fetched = 0;
                    const int MAX_FETCH_COUNT = 1;

                    //get the first ebmedded object
                    hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
                    Marshal.ThrowExceptionForHR(hr);

                    //while sucessfully get a new embedding, continue
                    for (int i = 0; SUICOMInterops.S_OK == hr; i++)
                    {
                        //QI pUnk for the IWebBrowser2 interface
                        IWebBrowser2 brow = pUnk as IWebBrowser2;

                        if (brow != null)
                        {
                            SUIHtmlDocument newDoc = new SUIHtmlDocument((IHTMLDocument2)brow.Document);
                            if (newDoc.IsFrameSet)
                            {
                                List<SUIHtmlDocument> frames = newDoc.Frames;
                                if ((frames != null) && (frames.Count > 0))
                                {
                                    list.AddRange(frames);
                                    frames.Clear();
                                }
                            }
                            else
                            {
                                list.Add(newDoc);
                            }
                        }

                        //get the next ebmedded object
                        hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
                        Marshal.ThrowExceptionForHR(hr);
                    }
                }
                catch
                {
                    //do nothing.
                }
                return list;
            }
        }

        public SUIHtmlControlBase ActiveElement
        {
            get
            {
                SUIHtmlControlBase activeElement = new SUIHtmlControlBase(this, IHTMLDocument2.activeElement);
                if (activeElement.TagName != null && activeElement.TagName.Equals("FRAME"))
                {
                    SUIHtmlDocument frame = GetFrame(activeElement);
                    if (activeElement != null)
                    {
                        activeElement = frame.ActiveElement;
                    }
                }
                return activeElement;
            }
        }

        public SUIHtmlControlBase SearchHtmlElementByID(string tagName, string id, int index)
        {
            List<SUIHtmlControlBase> list = GetElementsByTagName(tagName);
            int i = 0;
            Regex regex = new Regex("[0-9]+");
            string ctrlID = null;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (ctrl.ID != null)
                    ctrlID = regex.Replace(ctrl.ID, "*");
                if ((ctrl.ID != null && ctrlID.Equals(id)) ||
                    (ctrl.ID == null && id == null))
                {
                    if (i == index)
                    {
                        SUIHtmlControlBase newCtrl = new SUIHtmlControlBase(ctrl);
                        return newCtrl;
                    }
                    i++;
                }
            }
            return null;
        }

        public SUIHtmlControlBase SearchHtmlElementByText(string tagName, string text, int index)
        {
            if (text == "null")
                text = null;
            List<SUIHtmlControlBase> list = GetElementsByTagName(tagName);
            int i = 0;
            string str = null;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (ctrl.InnerText != null)
                {
                    if (ctrl.InnerText.Length > 50)
                        str = ctrl.InnerText.Substring(0, 50);//Only use the first 50 chars.
                    else
                        str = ctrl.InnerText;
                    if (str.IndexOf("\r\n") > -1)
                        str = str.Substring(0, str.IndexOf("\r\n"));
                }
                else
                    str = null;

                if ((str != null && str.Equals(text)) ||
                    (str == null && text == null))
                {
                    if (i == index)
                    {
                        SUIHtmlControlBase newCtrl = new SUIHtmlControlBase(ctrl);
                        return newCtrl;
                    }
                    i++;
                }
            }
            return null;
        }

        public SUIHtmlInputText SearchHtmlInputTextByID(string id, int index)
        {
            List<SUIHtmlControlBase> list = GetElementsByTagName("INPUT");
            int i = 0;
            Regex regex = new Regex("[0-9]+");
            string ctrlID = null;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (SUIHtmlInputText.IsInputTextElement(ctrl))
                {
                    if (ctrl.ID != null)
                        ctrlID = regex.Replace(ctrl.ID, "*");
                    if ((ctrl.ID != null && ctrlID.Equals(id)) ||
                        (ctrl.ID == null && id == null))
                    {
                        if (i == index)
                        {
                            SUIHtmlInputText inputText = new SUIHtmlInputText(ctrl);
                            return inputText;
                        }
                        i++;
                    }
                }
            }
            return null;
        }

        public SUIHtmlInputButton SearchHtmlInputButtonByID(string id, int index)
        {
            List<SUIHtmlControlBase> list = GetElementsByTagName("INPUT");
            int i = 0;
            Regex regex = new Regex("[0-9]+");
            string ctrlID = null;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (SUIHtmlInputButton.IsInputButtonElement(ctrl))
                {
                    if (ctrl.ID != null)
                        ctrlID = regex.Replace(ctrl.ID, "*");
                    if ((ctrl.ID != null && ctrlID.Equals(id)) ||
                        (ctrl.ID == null && id == null))
                    {
                        if (i == index)
                        {
                            SUIHtmlInputButton inputButton = new SUIHtmlInputButton(ctrl);
                            return inputButton;
                        }
                        i++;
                    }
                }
            }
            return null;
        }

        public SUIHtmlSelect SearchHtmlSelectByID(string id, int index)
        {
            List<SUIHtmlControlBase> list = GetElementsByTagName("SELECT");
            int i = 0;
            Regex regex = new Regex("[0-9]+");
            string ctrlID = null;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (SUIHtmlSelect.IsSelectElement(ctrl))
                {
                    if (ctrl.ID != null)
                        ctrlID = regex.Replace(ctrl.ID, "*");
                    if ((ctrl.ID != null && ctrlID.Equals(id)) ||
                        (ctrl.ID == null && id == null))
                    {
                        if (i == index)
                        {
                            SUIHtmlSelect select = new SUIHtmlSelect(ctrl);
                            return select;
                        }
                        i++;
                    }
                }
            }
            return null;
        }

        public SUIHtmlAnchor SearchHtmlAnchorByText(string text, int index)
        {
            List<SUIHtmlControlBase> list = GetElementsByTagName("A");
            int i = 0;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (SUIHtmlAnchor.IsAnchorElement(ctrl))
                {
                    SUIHtmlAnchor anchor = new SUIHtmlAnchor(ctrl);
                    if ((ctrl.InnerText != null && ctrl.InnerText.Equals(text)) ||
                        (ctrl.InnerText == null && text == null))
                    {
                        if (i == index)
                        {
                            //SUIHtmlAnchor anchor = new SUIHtmlAnchor(ctrl);
                            return anchor;
                        }
                        i++;
                    }
                }
            }
            return null;
        }

        public SUIHtmlSpan SearchHtmlSpanByText(string text, int index)
        {
            List<SUIHtmlControlBase> list = GetElementsByTagName("SPAN");
            int i = 0;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (SUIHtmlSpan.IsSpanElement(ctrl))
                {
                    if ((ctrl.InnerText != null && ctrl.InnerText.Equals(text)) ||
                        (ctrl.InnerText == null && text == null))
                    {
                        if (i == index)
                        {
                            SUIHtmlSpan span = new SUIHtmlSpan(ctrl);
                            return span;
                        }
                        i++;
                    }
                }
            }
            return null;
        }

        public SUIHtmlImage SearchHtmlImageByNameProp(string text, int index)
        {
            List<SUIHtmlControlBase> list = GetElementsByTagName("IMG");
            int i = 0;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (SUIHtmlImage.IsImageElement(ctrl))
                {
                    SUIHtmlImage img = new SUIHtmlImage(ctrl);

                    string name = img.IHTMLImgElement.nameProp.ToLower();
                    if (name != null)
                    {
                        name = name.Replace("_over", "");
                        //Handle changeable temp file name.
                        //e.g. crystalimagehandler.aspx?dynamicimage=cr_tmp_image_8f388c98-6123-42e4-a244-00ac4e52d6b3.png
                        int indexStart = name.IndexOf("_tmp_image_");
                        if (indexStart > -1)
                        {
                            string strChange = name.Substring(indexStart + 11, name.Length - 4 - (indexStart + 11));
                            name = name.Replace(strChange, "*");
                        }
                    }

                    if ((name != null && name.Equals(text)) ||
                        (name == null && text == null))
                    {
                        if (i == index)
                        {
                            return img;
                        }
                        i++;
                    }
                }
            }
            return null;
        }

        public SUIHtmlDiv SearchHtmlDivByID(string id, int index)
        {
            List<SUIHtmlControlBase> list = GetElementsByTagName("DIV");
            int i = 0;
            Regex regex = new Regex("[0-9]+");
            string ctrlID = null;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (SUIHtmlDiv.IsDivElement(ctrl))
                {
                    if (ctrl.ID != null)
                        ctrlID = regex.Replace(ctrl.ID, "*");
                    if ((ctrl.ID != null && ctrlID.Equals(id)) ||
                        (ctrl.ID == null && id == null))
                    {
                        if (i == index)
                        {
                            SUIHtmlDiv div = new SUIHtmlDiv(ctrl);
                            return div;
                        }
                        i++;
                    }
                }
            }
            return null;
        }

        public SUIHtmlAnchor SearchHtmlAnchorByHref(string href, int index)
        {
            List<SUIHtmlControlBase> list = GetElementsByTagName("A");
            int i = 0;
            foreach (SUIHtmlControlBase ctrl in list)
            {
                if (SUIHtmlAnchor.IsAnchorElement(ctrl))
                {
                    SUIHtmlAnchor anchor = new SUIHtmlAnchor(ctrl);
                    if ((anchor.NameProp != null && anchor.NameProp.Equals(href)) ||
                        (anchor.NameProp == null && href == null))
                    {
                        if (i == index)
                        {
                            return anchor;
                        }
                        i++;
                    }
                }
            }
            return null;
        }
        public SUIWindow InternetServer
        {
            get
            {
                return internetServer;
            }
            set
            {
                internetServer = value;
            }
        }
    }
}
