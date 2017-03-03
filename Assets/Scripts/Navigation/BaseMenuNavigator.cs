using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace MenuStack.Navigation
{
    /// <summary>
    /// Base for a control capable of navigating to a different <see cref="Menu"/> within a <see cref="MenuRoot"/>
    /// </summary>
    /// <remarks>
    /// <see cref="ExecuteInEditMode"/> makes this base super powerful.
    /// let's be extra careful about what we run in <see cref="Start"/>
    /// </remarks>
    [ExecuteInEditMode]
    public abstract class BaseMenuNavigator : MonoBehaviour, INavigator
    {
        /// <summary>
        /// Provides a location to navigate to, when navigation occurs
        /// </summary>
        /// <returns><see cref="Menu"/> to navigate to</returns>
        protected abstract Menu GetNavigationLocation();

        /// <summary>
        /// Triggers navigation
        /// </summary>
        public virtual void Navigate()
        {
            this.GetComponentInParent<MenuRoot>().OpenAsync(this.GetNavigationLocation());
        }

        /// <summary>
        /// Unity engine hook for start
        /// </summary>
        void Start()
        {
            // only autowire if we're in the editor, obvs
            if (!Application.isPlaying && Application.isEditor)
            {
                TryAutoWire();
            }
        }

        /// <summary>
        /// Internal helper to try and autowire
        /// </summary>
        void TryAutoWire()
        {
            var button = this.GetComponent<Button>();
            if (button == null)
            {
                return;
            }

            var navigator = this.GetComponent<BaseMenuNavigator>();
            if (navigator == null)
            {
                return;
            }

            UnityEventTools.AddPersistentListener(button.onClick, navigator.Navigate);
        }
    }
}
