using ContactsUWP.Common.Collections;
using ContactsUWP.Common.Helper;
using ContactsUWP.Data;
using ContactsUWP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContactsUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RootHost : Page
    {
        GroupedObservableCollection<char, Contact> ContactsGrouped { get; }

        private bool isPageInSplitMode;

        public RootHost()
        {
            InitializeComponent();

            isPageInSplitMode = true;
            ////ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            ////formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            ////formattableTitleBar.ButtonForegroundColor = Colors.Gray;

            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += RootHost_BackRequested;

            SizeChanged += RootHost_SizeChanged;

            ContactsGrouped = new GroupedObservableCollection<char, Contact>(s => s.Name != null && s.Name.Count() > 0 ? s.Name[0] : ' ', DataService.Instance.Contacts);
            ContactsGroupedSource.Source = ContactsGrouped;

            DataService.Instance.Contacts.ContactsAdded += Contacts_ContactsAdded;
            DataService.Instance.Contacts.ContactsRemoved += Contacts_ContactsRemoved;

            rootframe.Navigated += Rootframe_Navigated;


            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                if (ApiInformation.IsEventPresent(typeof(HardwareButtons).FullName, "BackPressed"))
                    HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            InitialNavigation();
        }

        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (!Frame.CanGoBack)
            {
                e.Handled = true;
                MessageDialog dialog = new MessageDialog("This action will close the App", "Close App ?");
                    
                dialog.Commands.Add(new UICommand("Yes", x => Application.Current.Exit()));
                dialog.Commands.Add(new UICommand("No"));

                var result = await dialog.ShowAsync();
            }
        }

        private void Rootframe_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            ////SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
            ////    ((Frame)sender).CanGoBack ?
            ////    AppViewBackButtonVisibility.Visible :
            ////    AppViewBackButtonVisibility.Collapsed;
        }

        private void Contacts_ContactsRemoved(IEnumerable<Contact> obj)
        {
            obj.ToList().ForEach(c => ContactsGrouped.Remove(c));
        }

        private void Contacts_ContactsAdded(IEnumerable<Contact> obj)
        {
            obj.ToList().ForEach(c => ContactsGrouped.Add(c));
        }

        private void InitialNavigation()
        {
            if (DataService.Instance.Contacts.Count() > 0)
            {
                var firstContact = DataService.Instance.Contacts.First();
                rootframe.Navigate(typeof(ContactDetailPage), firstContact);
            }
            else
            {
                rootframe.Navigate(typeof(AddContactPage));
            }
        }

        private void SetVisualState(double width)
        {
            if (width > 0 && width < 720 && isPageInSplitMode)
            {
                VisualStateManager.GoToState(this, "state_320", false);
            }
            else if (width > 720 && isPageInSplitMode)
            {
                VisualStateManager.GoToState(this, "state_720", false);
            }
        }

        private void RootHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetVisualState(e.NewSize.Width);
        }

        private void RootHost_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (VisualStateHelper.SwitchVisualState(this, VisualStateGroup, "state_320", "state_FrameZero")) return;

            if (rootframe.CanGoBack)
            {
                e.Handled = true;
                rootframe.GoBack();
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
        }

        private void OnContactSelected(object sender, ItemClickEventArgs e)
        {
            Contact contact = e.ClickedItem as Contact;

            if (contact != null)
            {
                rootframe.Navigate(typeof(ContactDetailPage), contact);
                VisualStateHelper.SwitchVisualState(this, VisualStateGroup, "state_FrameZero", "state_320");
            }
        }

        private void BtnContacts_Click(object sender, RoutedEventArgs e)
        {
            InitialNavigation();
            isPageInSplitMode = true;
            SetVisualState(ActualWidth);
        }

        private void BtnAddContact_Click(object sender, RoutedEventArgs e)
        {
            VisualStateHelper.SwitchVisualState(this, VisualStateGroup, "state_FrameZero", "state_320");
            rootframe.Navigate(typeof(AddContactPage));
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "state_320", false);
            isPageInSplitMode = false;
            rootframe.Navigate(typeof(SettingsPage));
        }

        private async void ContactImageEllipse_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Ellipse el = sender as Ellipse;
            if (el != null)
            {
                // TODO: bei edit und update profilbild null!?
                Contact c = el.DataContext as Contact;
                if (c != null)
                {
                    ImageBrush ib = new ImageBrush();

                    if (c.ContactImage != null)
                    {
                        ib.ImageSource = await FileHelper.LoadImageFromBytesAsync(c.ContactImage);
                    }
                    else
                    {
                        ib.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/user_01.png"));
                    }

                    el.Fill = ib;
                }
            }
        }

        private void ContactsAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (!string.IsNullOrWhiteSpace(sender.Text))
                {
                    List<string> suggestions = new List<string>();
                    foreach (var item in DataService.Instance.Contacts.Where(c => c.Name.Contains(sender.Text)))
                    {
                        suggestions.Add(item.Name.ToString());
                    }
                    sender.ItemsSource = suggestions;
                }
                else
                {
                    sender.ItemsSource = ContactsGrouped;
                }
            }
        }

        private void ContactsAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            GroupedObservableCollection<string, Contact> suggestions = new GroupedObservableCollection<string, Contact>(s => s.Name[0].ToString().ToUpper(), new List<Contact>());

            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                foreach (var item in DataService.Instance.Contacts.Where(c => c.Name.Contains(args.ChosenSuggestion.ToString())))
                {
                    suggestions.Add(item);
                }
            }
            else
            {
                // Use args.QueryText to determine what to do.
                // User selected an item from the suggestion list, take an action on it here.
                foreach (var item in DataService.Instance.Contacts.Where(c => c.Name.Contains(args.QueryText)))
                {
                    suggestions.Add(item);
                }
            }
            ContactsGroupedSource.Source = suggestions;
        }

        private void ContactsAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            sender.Text = args.SelectedItem.ToString();
        }
    }
}
