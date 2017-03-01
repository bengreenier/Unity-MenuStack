using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuStack.Navigation
{
    /// <summary>
    /// Represents a menu navigator that navigates the next <see cref="Menu"/> in the <see cref="MenuRoot"/>
    /// </summary>
    public class NextMenuNavigator : BaseMenuNavigator
    {
        /// <summary>
        /// Provides a location to navigate to, when navigation occurs
        /// </summary>
        /// <returns><see cref="Menu"/> to navigate to</returns>
        protected override Menu GetNavigationLocation()
        {
            return FindFirstRecursively(this.GetComponentInParent<MenuRoot>().transform, this.GetComponentInParent<Menu>());
        }

        /// <summary>
        /// Internal recursive matcher
        /// </summary>
        /// <param name="root">the root level transform to start matching</param>
        /// <param name="current">the current menu, we want the one after this</param>
        /// <returns>a matched <see cref="Menu"/>, or <c>null</c></returns>
        public Menu FindFirstRecursively(Transform root, Menu current)
        {
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                var menu = child.GetDerivedMenuComponent();

                if (menu != null && menu != current)
                {
                    return menu;
                }

                if (child.childCount > 0)
                {
                    var res = FindFirstRecursively(child, current);

                    if (res != null)
                    {
                        return res;
                    }
                }
            }

            return null;
        }
    }
}
