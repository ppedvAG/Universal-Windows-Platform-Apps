using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel;
using Windows.Globalization;
using System.Collections.Generic;
using Windows.System.UserProfile;
using Windows.ApplicationModel.Resources.Core;
using System.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContactsUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RequestedTheme = ElementTheme.Light;
        }
    }
}
