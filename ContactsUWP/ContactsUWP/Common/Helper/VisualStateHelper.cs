using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ContactsUWP.Common.Helper
{
    public static class VisualStateHelper
    {
        public static bool SwitchVisualState(Control parent, VisualStateGroup vsg, string inputState, string targetState)
        {
            string actualStateName = string.Empty;

            if (vsg.CurrentState != null)
                actualStateName = vsg.CurrentState.Name;

            if (inputState == actualStateName)
            {
                VisualStateManager.GoToState(parent, targetState, false);
                return true;
            }
            return false;
        }
    }
}
