using System.Collections.Generic;
using UnityEngine;

namespace MenuStack
{
    /// <summary>
    /// Represents the root node (and controller) of a menu system
    /// </summary>
    public class MenuRoot : MonoBehaviour
    {
        /// <summary>
        /// Menu event handler
        /// </summary>
        /// <param name="menu">the changed menu</param>
        public delegate void MenuChangedHandler(Menu menu);

        /// <summary>
        /// Menu opened event. fired when a menu is opened
        /// </summary>
        public event MenuChangedHandler Opened;

        /// <summary>
        /// Menu closed event. fired when a menu is closed
        /// </summary>
        public event MenuChangedHandler Closed;

        /// <summary>
        /// Disabling runtime menu tagging requires the caller to specify <see cref="TrackedMenus"/> in order for anything to work
        /// </summary>
        /// <remarks>
        /// The caller may find this useful if they see runtime perf costs of iterating over a large menu system
        /// </remarks>
        [Tooltip("With this on, we won't auto tag the menu tree at runtime")]
        public bool DisableRuntimeMenuTagging = true;

        /// <summary>
        /// A custom prefix with which menu components are identified
        /// </summary>
        /// <remarks>
        /// This is only used when <see cref="DisableRuntimeMenuTagging"/> is <c>false</c>
        /// </remarks>
        [Tooltip("The prefix with which menu objects should begin")]
        public string MenuPrefix = "menu";

        /// <summary>
        /// Manually specify <see cref="Menu"/>s that this <see cref="MenuRoot"/> controls
        /// </summary>
        /// <remarks>
        /// Overriden at runtime when <see cref="DisableRuntimeMenuTagging"/> is <c>false</c>
        /// </remarks>
        [Tooltip("Manually track specific menus as part of this MenuRoot")]
        public Menu[] TrackedMenus;

        /// <summary>
        /// The initially selected menu
        /// </summary>
        /// <remarks>
        /// If this isn't set, the first <see cref="Menu"/> in the heirarchy will be used
        /// </remarks>
        [Tooltip("Manually set the default selected menu")]
        public Menu SelectedMenu;

        /// <summary>
        /// Internal history stack
        /// </summary>
        private Stack<Menu> history = new Stack<Menu>();

        /// <summary>
        /// Unity engine hook for object awake
        /// </summary>
        void Awake()
        {
            if (!DisableRuntimeMenuTagging)
            {
                this.TrackedMenus = RTag(this.transform).ToArray();
            }

            if (TrackedMenus != null)
            {
                foreach (var menu in TrackedMenus)
                {
                    menu.SetInteractable(false);
                    menu.SetVisible(false);
                }
            }

            if (this.SelectedMenu == null && this.TrackedMenus != null && this.TrackedMenus.Length > 0)
            {
                this.SelectedMenu = this.TrackedMenus[0];
            }

            if (SelectedMenu != null)
            {
                Open(SelectedMenu);
            }
        }

        /// <summary>
        /// Recursive tagger for menu components
        /// </summary>
        /// <remarks>
        /// Only used when <see cref="DisableRuntimeMenuTagging"/> is <c>false</c>
        /// </remarks>
        /// <param name="root">the <see cref="Transform"/> at which tagging should start</param>
        /// <returns>a list of tagged <see cref="Menu"/> components</returns>
        List<Menu> RTag(Transform root)
        {
            List<Menu> tracked = new List<Menu>();
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                var menu = child.GetComponent<Menu>();

                if (child.name.StartsWith(MenuPrefix) && menu == null)
                {
                    menu = child.gameObject.AddComponent<Menu>();
                }

                if (menu != null)
                {
                    tracked.Add(menu);
                }

                if (child.childCount > 0)
                {
                    tracked.AddRange(RTag(child));
                }
            }

            return tracked;
        }
        
        /// <summary>
        /// Closes the currently open menu
        /// </summary>
        public void Close()
        {
            var top = history.Count == 0 ? null : history.Pop();

            if (top != null)
            {
                // order here is significant (we don't disable interaction on components in inactive gameobjects)
                top.SetInteractable(false);
                top.SetVisible(false);

                if (this.Closed != null)
                {
                    this.Closed(top);
                }
            }

            var newTop = history.Count == 0 ? null : history.Peek();

            if (newTop != null)
            {
                newTop.SetVisible(true);
                newTop.SetInteractable(true);
            }
        }

        /// <summary>
        /// Opens a particular menu
        /// </summary>
        /// <remarks>
        /// If <c>leaveOldVisible</c> is <c>true</c> ensure the <see cref="RectTransform"/> z value is correctly set
        /// </remarks>
        /// <param name="menu"><see cref="Menu"/> to open</param>
        /// <param name="leaveOldVisible">indicates if we should leave the current <see cref="Menu"/> visible</param>
        public void Open(Menu menu, bool leaveOldVisible = false)
        {
            var top = history.Count == 0 ? null : history.Peek();

            if (top != null)
            {
                top.SetInteractable(false);

                if (!leaveOldVisible)
                {
                    top.SetVisible(false);
                }
            }

            history.Push(menu);

            menu.SetVisible(true);
            menu.SetInteractable(true);

            if (this.Opened != null)
            {
                this.Opened(menu);
            }
        }
    }
}