using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace ContactsUWP.Model
{
    public class Contacts : List<Contact>
    {
        public event Action<IEnumerable<Contact>> ContactsAdded;
        public event Action<IEnumerable<Contact>> ContactsRemoved;

        public Contacts()
        {

        }

        public Contacts(List<Contact> list)
        {
            AddRange(list);
        }

        public new void Add(Contact item)
        {
            base.Add(item);
            ContactsAdded?.Invoke(new Contact[] { item });
        }

        public new void AddRange(IEnumerable<Contact> collection)
        {
            base.AddRange(collection);
            ContactsAdded?.Invoke(collection);
        }

        public new bool Remove(Contact item)
        {
            bool ret = base.Remove(item);
            if (ret)
            {
                ContactsRemoved?.Invoke(new Contact[] { item });
                return ret;
            }

            return ret;
        }

        public new int RemoveAll(Predicate<Contact> match)
        {
            int ret = base.RemoveAll(match);
            ContactsRemoved?.Invoke(FindAll(match));
            return ret;
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            ContactsRemoved?.Invoke(new Contact[] { this[index] });
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            ContactsRemoved?.Invoke(GetRange(index, count));
        }
    }
}
