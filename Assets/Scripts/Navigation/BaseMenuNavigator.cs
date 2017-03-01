﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuStack.Navigation
{
    /// <summary>
    /// Base for a control capable of navigating to a different <see cref="Menu"/> within a <see cref="MenuRoot"/>
    /// </summary>
    public abstract class BaseMenuNavigator : MonoBehaviour, INavigator
    {
        /// <summary>
        /// Allows the caller to decide to leave the previous menu visible
        /// </summary>
        public bool LeavePreviousVisible = false;

        /// <summary>
        /// Provides a location to navigate to, when navigation occurs
        /// </summary>
        /// <returns><see cref="Menu"/> to navigate to</returns>
        protected abstract Menu GetNavigationLocation();

        /// <summary>
        /// Triggers navigation
        /// </summary>
        public void Navigate()
        {
            this.GetComponentInParent<MenuRoot>().Open(this.GetNavigationLocation(), this.LeavePreviousVisible);
        }
    }
}