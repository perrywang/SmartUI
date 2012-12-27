using System;
using System.Collections.Generic;
using System.Text;
using SUI.Base.UIControls.WinControls.Win32;

namespace SUI.Base.UIControls.WinControls.Dotnet
{
    public class SUIDotNetTreeView : SUIDotNetControl
    {
        SUITreeView suiTreeView = null;
        public  SUIDotNetTreeView(IntPtr hWnd) 
            : base(hWnd)
        {
            suiTreeView = new SUITreeView(hWnd);
        }
        
        public SUIDotNetTreeView(SUIWindow sui)
            : this(sui.WindowHandle)
        {
        }

        public SUIDotNetTreeView(SUIDotNetControl ctrl)
            : this(ctrl.WindowHandle)
        {
        }
        public SUITreeViewNode GetNextItem(IntPtr currentItem, int flag)
        {
            return suiTreeView.GetNextItem(currentItem, flag);
        }
        public SUITreeViewNode FindTopLevelNode(string text)
        {
            return suiTreeView.FindTopLevelNode(text);
        }
        public SUITreeViewNode Root
        {
            get
            {
                return suiTreeView.Root;
            }
        }
        public void SelectNode(SUITreeViewNode node)
        {
            suiTreeView.SelectNode(node);
        }
        void ExpandNode(SUITreeViewNode node, int flag)
        {
            suiTreeView.ExpandNode(node, flag);
        }
        public void ExpandNode(SUITreeViewNode node)
        {
            suiTreeView.ExpandNode(node);
        }
        public void Collapse(SUITreeViewNode node)
        {
            suiTreeView.Collapse(node);
        }
        public SUITreeViewNode SelectedNode
        {
            get
            {
                return FindSelectedNode(null);
            }
        }
        public SUITreeViewNode FindSelectedNode(SUITreeViewNode parentNode)
        {
            return suiTreeView.FindSelectedNode(parentNode);
        }
        public void ClickToSelectNode(SUITreeViewNode node)
        {
            suiTreeView.ClickToSelectNode(node);
        }
    }
}
