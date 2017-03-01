using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuStack
{
    /// <summary>
    /// Represents any menu (a logical grouping of ui elements) within a <see cref="MenuRoot"/>
    /// </summary>
    public class Menu : MonoBehaviour
    {
        /// <summary>
        /// The runtime name for this menu
        /// </summary>
        public string Name
        {
            get
            {
                return this.CustomName == null ? this.name : this.CustomName;
            }
        }

        /// <summary>
        /// Indicates if this menu is visible
        /// </summary>
        public bool Visible
        {
            get;
            protected set;
        }

        /// <summary>
        /// Indicates if this menu is interactable
        /// </summary>
        public bool Interactable
        {
            get;
            protected set;
        }

        /// <summary>
        /// A custom name for this menu
        /// </summary>
        /// <remarks>
        /// Overrides the default <see cref="Name"/>
        /// </remarks>
        [Tooltip("Overrides the default name")]
        public string CustomName = null;

        /// <summary>
        /// Internal flag used to track when this object hasn't been <see cref="Start"/>-ed yet
        /// </summary>
        private bool uninitialized = true;

        /// <summary>
        /// Unity engine hook for object start
        /// </summary>
        void Start()
        {
            this.uninitialized = false;
        }

        /// <summary>
        /// Setter to change the interactable state of this menu
        /// </summary>
        /// <remarks>
        /// Note: this changes the state of the menus underlying <see cref="Selectable"/> components
        /// </remarks>
        /// <param name="value">new state</param>
        public virtual void SetInteractable(bool value)
        {
            if (!value && (this.Interactable || this.uninitialized))
            {
                SetChildComponents<Selectable>(false);
            }
            else if (value && (!this.Interactable || this.uninitialized))
            {
                SetChildComponents<Selectable>(true, skipMenus: true);
            }

            this.Interactable = value;
        }

        /// <summary>
        /// Setter to change the visible state of this menu
        /// </summary>
        /// <remarks>
        /// Note: this changes the state of the menus underlying <see cref="GameObject"/>s
        /// </remarks>
        /// <param name="value">new state</param>
        public virtual void SetVisible(bool value)
        {
            if (!value && (this.Visible || this.uninitialized))
            {
                SetChildActive(false, skipMenus: true);
            }
            else if (value && (!this.Visible || this.uninitialized))
            {
                SetChildActive(true, skipMenus: true);
            }

            this.Visible = value;
        }

        /// <summary>
        /// Internal helper to enable/disable child <see cref="Behaviour"/>s
        /// </summary>
        /// <typeparam name="TComp">component type</typeparam>
        /// <param name="value">new state</param>
        /// <param name="skipMenus">indicates if we should include of exclude children with <see cref="Menu"/> components</param>
        private void SetChildComponents<TComp>(bool value, bool skipMenus = false) where TComp : Behaviour
        {
            foreach (var comp in this.GetComponentsInChildren<TComp>())
            {
                if (skipMenus && comp.GetComponent<Menu>() != null)
                {
                    continue;
                }

                comp.enabled = value;
            }
        }

        /// <summary>
        /// Internal helper to enable/disable child <see cref="GameObject"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="skipMenus">indicates if we should include of exclude children with <see cref="Menu"/> components</param>
        private void SetChildActive(bool value, bool skipMenus = false)
        {
            for (var i = 0; i < this.transform.childCount; i++)
            {
                var child = this.transform.GetChild(i);

                if (skipMenus && child.GetComponent<Menu>() != null)
                {
                    continue;
                }

                child.gameObject.SetActive(value);
            }
        }
    }
}
