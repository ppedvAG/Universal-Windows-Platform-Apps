using ContactsUWP.Common.Collections;
using ContactsUWP.Data;
using ContactsUWP.Model;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContactsUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactDetailPage : Page
    {
        public Contact SelectedContact { get; set; }

        public ContactDetailPage()
        {
            InitializeComponent();
        }

        public ContactDetailPage(Contact dataContext) : this()
        {
            DataContext = dataContext;
            CommandBar.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager dtManager = DataTransferManager.GetForCurrentView();
            dtManager.DataRequested += OnDataRequested;
            Contact contact = e.Parameter as Contact;

            if (contact != null)
            {
                SelectedContact = contact;
                DataContext = contact;
                return;
            }
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = SelectedContact.Name;
            request.Data.Properties.Description = "Kontaktinformationen";
            request.Data.SetText($"{SelectedContact.Name}\n{SelectedContact.Address}\n{SelectedContact.Mail}\n{SelectedContact.Phone}\n");
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddContactPage), SelectedContact);
        }

        private async void BtnPinToStart_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedContact.IsPinned)
            {
                var imageUri = new Uri("ms-appx:///Assets/user_01.png");

                if (SelectedContact.ContactImage != null)
                {
                    //create file in public folder
                    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                    StorageFile tilePic = await storageFolder.CreateFileAsync(SelectedContact.Id + ".png", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBytesAsync(tilePic, SelectedContact.ContactImage);
                    // not over 200K or 1024px or TemporaryFolder
                    imageUri = new Uri("ms-appdata:///local/" + SelectedContact.Id + ".png", UriKind.Absolute);
                }

                // Create a Secondary tile with all the required arguments.
                var secondaryTile = new SecondaryTile(SelectedContact.Id,
                    SelectedContact.Name,
                    SelectedContact.Name, imageUri, TileSize.Square150x150)
                { RoamingEnabled = true };
                try
                {
                    await secondaryTile.RequestCreateAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                SelectedContact.IsPinned = true;
                //TODO: dirty update model

            }
            else
            {
                //unpin
                SecondaryTile tile = (await SecondaryTile.FindAllAsync()).FirstOrDefault((t) => t.TileId == SelectedContact.Id);
                if (tile != null) await tile.RequestDeleteAsync();

                SelectedContact.IsPinned = false;
                //TODO: dirty update model

            }

            await DataService.Instance.SaveAsync();
        }

        private void BtnShare_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            DataService.Instance.Contacts.Remove(SelectedContact);
            Contact c = DataService.Instance.Contacts.FirstOrDefault();

            if (c != null)
            {
                Frame.Navigate(typeof(ContactDetailPage), c);
            }
            else
            {
                Frame.Navigate(typeof(AddContactPage));
            }
        }

        private void BtnCall_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
