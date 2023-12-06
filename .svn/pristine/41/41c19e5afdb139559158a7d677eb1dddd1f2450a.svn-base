using UMSV.Schema;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UMSV.ViewModels
{
    public class CompositeNodeViewModel : ViewModelBase
    {
        public event EventHandler Removing;

        public CompositeNodeViewModel(TreeNode relatedTreeNode, GraphViewModel graphModel)
        {
            this.TreeNode = relatedTreeNode;
            this.GraphModel = graphModel;
        }

        internal void OnRemoving()
        {
            if (Removing != null)
                Removing(this, EventArgs.Empty);
        }

        public TreeNode TreeNode
        {
            private set;
            get;
        }

        private bool isTargetLocked;
        /// <summary>
        /// Gets or sets whether the target node for this node can be selected or the targetSelector is locked.
        /// </summary>
        public bool IsTargetLocked
        {
            get
            {
                return isTargetLocked;
            }
            set
            {
                isTargetLocked = value;
                SendPropertyChanged("IsTargetLocked");
            }
        }

        public NodeGroup NodeData
        {
            get
            {
                return TreeNode.GraphNode;
            }
        }

        public GraphViewModel GraphModel
        {
            private set;
            get;
        }

    }
}
