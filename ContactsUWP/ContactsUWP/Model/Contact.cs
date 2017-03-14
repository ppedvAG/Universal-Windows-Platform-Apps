using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ContactsUWP.Model
{
    public class Contact : INotifyPropertyChanged
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set
            {
                if (value != _address)
                {
                    _address = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set
            {
                if (value != _phone)
                {
                    _phone = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _mail;

        public string Mail
        {
            get { return _mail; }
            set
            {
                if (value != _mail)
                {
                    _mail = value;
                    RaisePropertyChanged();
                }
            }
        }

        private byte[] _contactImage;

        public byte[] ContactImage
        {
            get { return _contactImage; }
            set
            {
                if (value != _contactImage)
                {
                    _contactImage = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _creationDate;

        public string CreationDate
        {
            get { return _creationDate; }
            set
            {
                if (value != _creationDate)
                {
                    _creationDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isPinned;

        public bool IsPinned
        {
            get { return _isPinned; }
            set
            {
                if (value != _isPinned)
                {
                    _isPinned = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Contact()
        {
            _id = string.Empty;
            _mail = string.Empty;
            _name = string.Empty;
            _phone = string.Empty;
            _address = string.Empty;
        }

        public Contact Copy()
        {
            return (Contact)MemberwiseClone();
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
