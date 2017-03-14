using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsUWP.ViewModels
{
    public class AddContactPageViewModel
    {
        // false == add, true == edit
        private bool isInEditMode;

        private ContactViewModel contact_tmp = new ContactViewModel();

        public ContactViewModel Contact { get; set; } = new ContactViewModel();

        public AddContactPage()
        {
            InitializeComponent();
        }

        private async void BtnSaveContact_Click(object sender, RoutedEventArgs e)
        {
            if (isInEditMode)
                App.ContactsViewModel.ContactsGrouped.Remove(contact_tmp);
            else
            {
                Contact.Id = Guid.NewGuid().ToString();
                Contact.TimeStampCreation = DateTime.Now;
            }

            App.ContactsViewModel.ContactsGrouped.Add(Contact);
            await App.ContactsViewModel.SaveAsync();
            RootHost.RootFrame.Navigate(typeof(ContactDetailPage), Contact);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            contact_tmp = e.Parameter as ContactViewModel;

            if (contact_tmp != null)
            {
                isInEditMode = true;
                Contact = contact_tmp.Copy();
                if (contact_tmp.ContactImage != null)
                {
                    await ContactImageViewer.LoadImageFromBytesAsync(contact_tmp.ContactImage);
                }
            }
            else
                contact_tmp = new ContactViewModel();

            DataContext = Contact;
        }

        private async void ContactImageViewer_ImageSelected(ImageViewerControl viewer)
        {
            Contact.ContactImage = await FileHelper.StorageFileAsByteArrayAsync(viewer.ImageFile);
        }
    }
}
