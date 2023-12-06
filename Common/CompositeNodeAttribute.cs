using System;

namespace UMSV
{

    public class CompositeNodeAttribute : Attribute
    {
        public CompositeNodeAttribute()
        {
            // Default Index is int.MaxValue, so the item is placed after all other items, when the Index is not specified.
            Index = int.MaxValue;
            // Default GroupIndex is int.MaxValue, so the Group is placed after all other Groups, when the GroupIndex is not specified.
            GroupIndex = int.MaxValue;
            ChildMode = ChildModes.SingleChild;
        }

        /// <summary>
        /// This tag must be the same as the one used in the corresponding NodeGroup element.
        /// </summary>
        public string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Path to the icon image of this node.
        /// </summary>
        public string Icon
        {
            get;
            set;
        }

        /// <summary>
        /// The title of this node to be displayed on menu, naturally the same as its description.
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        public ChildModes ChildMode
        {
            get;
            set;
        }

        /// <summary>
        /// Determines the order of CompositeNodes in a group when appearing in a menu.
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Grouping composite nodes causes them to be placed together when appearing in the menu and separates each group from others by separaters.
        /// When placing on a menu, groups are ordered by their GroupIndex.
        /// </summary>
        public int GroupIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sub-menu in which this item is to be placed. The sub menu is single level and this property does not accept path.
        /// </summary>
        public string SubMenu
        {
            get;
            set;
        }

        public string ViewRole
        {
            get;
            set;
        }
    }

    public enum ChildModes
    {
        MultiChild,
        SingleChild,
        Childless
    }
}
