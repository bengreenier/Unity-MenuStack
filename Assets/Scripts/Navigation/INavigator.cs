using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuStack.Navigation
{
    /// <summary>
    /// Defines a common interface that all navigators implement
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Triggers navigation
        /// </summary>
        void Navigate();
    }
}
