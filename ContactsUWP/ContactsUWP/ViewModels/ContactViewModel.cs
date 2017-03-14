using ContactsUWP.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ContactsUWP.ViewModels
{
    public class ContactViewModel : INotifyPropertyChanged
    {
        private Contact _model = new Contact();

        public Contact Model
        {
            get
            {
                return _model;
            }
        }

        public ContactViewModel()
        {

        }

        public ContactViewModel(Contact model)
        {
            _model = model;
        }

        public string Id
        {
            get { return _model.Id; }
            set { _model.Id = value; }
        }

        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; }
        }

        public string Address
        {
            get { return _model.Address; }
            set { _model.Address = value; }
        }

        public string Phone
        {
            get { return _model.Phone; }
            set { _model.Phone = value; }
        }

        public string Mail
        {
            get { return _model.Mail; }
            set { _model.Mail = value; }
        }

        public byte[] ContactImage
        {
            get { return _model.ContactImage; }
            set { _model.ContactImage = value; }
        }

        public DateTime TimeStampCreation
        {
            get { return _model.TimeStampCreation; }
            set { _model.TimeStampCreation = value; }
        }

        public bool IsPinned
        {
            get { return _model.IsPinned; }
            set
            {
                if (value != _model.IsPinned)
                {
                    _model.IsPinned = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ContactViewModel Copy()
        {
            return new ContactViewModel(_model.Copy());
        }
    }
}
