using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuStack.Navigation
{
    /// <summary>
    /// Represents a menu navigator that goes back (effectively closing the current menu) in the <see cref="MenuRoot"/>
    /// </summary>
    public class BackMenuNavigator : MonoBehaviour, INavigator
    {
        /// <summary>
        /// Triggers navigation
        /// </summary>
        public void Navigate()
        {
            this.GetComponentInParent<MenuRoot>().Close();
        }
    }
}
