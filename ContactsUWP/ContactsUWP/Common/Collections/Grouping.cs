﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ContactsUWP.Common.Collections
{
    public class Grouping<TKey, TElement> : ObservableCollection<TElement>, IGrouping<TKey, TElement>
    {
        public Grouping(TKey key)
        {
            Key = key;
        }

        public Grouping(TKey key, IEnumerable<TElement> items) : this(key)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public TKey Key { get; }
    }
}
