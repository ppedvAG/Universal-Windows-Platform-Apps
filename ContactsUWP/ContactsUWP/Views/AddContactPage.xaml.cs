using ContactsUWP.Common.Controls;
using ContactsUWP.Common.Helper;
using ContactsUWP.Data;
using ContactsUWP.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContactsUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddContactPage : Page
    {
        // false == add, true == edit
        private bool isInEditMode;

        private Contact contact_tmp = new Contact();
        public Contact Contact { get; set; } = new Contact();

        public AddContactPage()
        {
            InitializeComponent();
        }

        private async void BtnSaveContact_Click(object sender, RoutedEventArgs e)
        {
            if (isInEditMode)
                DataService.Instance.Contacts.Remove(contact_tmp);
            else
            {
                Contact.Id = Guid.NewGuid().ToString();
                Contact.CreationDate = DateTime.Now.ToString();
            }

            DataService.Instance.Contacts.Add(Contact);
            await DataService.Instance.SaveAsync();
            Frame.Navigate(typeof(ContactDetailPage), Contact);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            contact_tmp = e.Parameter as Contact;

            if (contact_tmp != null)
            {
                isInEditMode = true;
                Contact = contact_tmp.Copy();
                if (contact_tmp.ContactImage != null)
                {
                    await ContactImageViewer.LoadImageFromBytesAsync(contact_tmp.ContactImage);
                }
            }
            else contact_tmp = new Contact();

            DataContext = Contact;
        }

        private async void ImageCaptureControl_ImageSelected(ImageCaptureControl ctrl)
        {
            Contact.ContactImage = await FileHelper.StorageFileAsByteArrayAsync(ctrl.ImageFile);
        }
    }
}
