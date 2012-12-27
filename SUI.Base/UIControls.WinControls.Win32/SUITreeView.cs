using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.Win;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SUI.Base.UIControls.WinControls.Win32
{
    public class SUITreeView : SUIWindow
    {
        public SUITreeView(IntPtr hWnd)
            : base(hWnd)
        { }

        public SUITreeView(SUIWindow win)
            : base(win)
        { }

        internal SUITreeViewNode GetNextItem(IntPtr currentItem, int flag)
        {
            IntPtr itemPtr = new IntPtr(SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.TVM_GETNEXTITEM, new IntPtr(flag), currentItem));
            TVITEM item = new TVITEM();
            item.hItem = itemPtr;
            return new SUITreeViewNode(this, item); 
        }

        public SUITreeViewNode FindTopLevelNode(string text)
        {
            return FindTopLevelNode(text, false);
        }

        public SUITreeViewNode FindTopLevelNode(string text, bool isRegex)
        {
            SUITreeViewNode node = null;
            Regex reg = null;

            if(isRegex)
                reg = new Regex(text);

            // Get root node.
            node = GetNextItem(IntPtr.Zero, SUIMessage.TVGN_ROOT);

            // Iterate all top level nodes to search target node.
            while (!node.IsNullNode)
            {
                if (isRegex)
                {
                    if (reg.IsMatch(node.Text))
                    {
                        break;
                    }
                }
                else
                {
                    if (node.Text.Equals(text))
                    {
                        break;
                    }
                }

                node = node.NextSiblingNode;
            }
            return node;
        }

        public SUITreeViewNode FindTopLevelNode(int index)
        {
            SUITreeViewNode node = null;

            // Get root node.
            node = GetNextItem(IntPtr.Zero, SUIMessage.TVGN_ROOT);

            // Iterate all top level nodes to search target node.
            while (!node.IsNullNode)
            {
                if (index == 0)
                {
                    break;
                }

                node = node.NextSiblingNode;
                index--;
            }
            return node;
        }

        public SUITreeViewNode Root
        {
            get 
            {
                return GetNextItem(IntPtr.Zero, SUIMessage.TVGN_ROOT);
            }
        }

        public void SelectNode(SUITreeViewNode node)
        {
            if (node.IsNullNode)
                throw new SUIException("Cannot select NULL TreeView node!");
            SUIWinAPIs.SendMessage(WindowHandle, SUIMessage.TVM_SELECTITEM, new IntPtr(SUIMessage.TVGN_CARET), node.hItem);
        }

        internal void ExpandNode(SUITreeViewNode node, int flag)
        {
            SUIWinAPIs.PostMessage(WindowHandle, SUIMessage.TVM_EXPAND, new IntPtr(flag), node.hItem);
            SUISleeper.Sleep(500);
        }

        public void ExpandNode(SUITreeViewNode node)
        {
            if (node.IsNullNode)
                throw new SUIException("Cannot expand NULL TreeView node!");
            ExpandNode(node, SUIMessage.TVE_EXPAND);
        }

        public void Collapse(SUITreeViewNode node)
        {
            if (node.IsNullNode)
                throw new SUIException("Cannot Collapse NULL TreeView node!");
            ExpandNode(node, SUIMessage.TVE_COLLAPSE);
        }

        // Please note that we suppose that there is only one selected note.
        // As for the muiltiple selection cases, we will support if necessary later.
        public SUITreeViewNode SelectedNode
        {
            get
            {
                return FindSelectedNode(null);
            }
        }
        
        public SUITreeViewNode FindSelectedNode(SUITreeViewNode parentNode)
        {
            SUITreeViewNode startNode = null;
            if (parentNode == null)
                startNode = Root;
            else
                startNode = parentNode.FirstChildNode;

            // Iterate all nodes on the same level.
            while (!startNode.IsNullNode)
            {
                if (startNode.IsSelected)
                {
                    return startNode;
                }
                else
                {
                    //Depth first recursive search.
                    SUITreeViewNode foundNode = FindSelectedNode(startNode);
                    if (!foundNode.IsNullNode)
                        return foundNode;
                }

                startNode = startNode.NextSiblingNode;
            }
            TVITEM item = new TVITEM();
            item.hItem = IntPtr.Zero;
            return new SUITreeViewNode(this, item);
        }

        public void ClickToSelectNode(SUITreeViewNode node)
        {
            SelectNode(node);
            SUIMouse.MouseClick(this, node.Rectangle.X + 5, node.Rectangle.Y + 5);
        }
    }
}
