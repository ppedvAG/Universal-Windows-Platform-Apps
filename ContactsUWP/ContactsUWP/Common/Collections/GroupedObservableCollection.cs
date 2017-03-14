﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace ContactsUWP.Common.Collections
{
    /// <summary>
    /// https://github.com/mikegoatly/GroupedObservableCollection/blob/master/Munklesoft.Common.Collections/GroupedObservableCollection.cs
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public class GroupedObservableCollection<TKey, TElement> : ObservableCollection<Grouping<TKey, TElement>>
       where TKey : IComparable<TKey>
    {
        private readonly Func<TElement, TKey> readKey;

        public event Action<TElement> elementAdded;
        public event Action<TElement> elementRemoved;

        /// <summary>
        /// This is used as an optimisation for when items are likely to be added in key order and there is a good probability
        /// that when an item is added, then next one will be in the same grouping.
        /// </summary>
        private Grouping<TKey, TElement> lastEffectedGroup;

        public GroupedObservableCollection(Func<TElement, TKey> readKey)
        {
            this.readKey = readKey;
        }

        public GroupedObservableCollection(Func<TElement, TKey> readKey, IEnumerable<TElement> items)
            : this(readKey)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public bool Contains(TElement item)
        {
            return Contains(item, (a, b) => a.Equals(b));
        }

        public bool Contains(TElement item, Func<TElement, TElement, bool> compare)
        {
            var key = readKey(item);

            var group = TryFindGroup(key);
            return group != null && group.Any(i => compare(item, i));
        }

        public IEnumerable<TElement> EnumerateItems()
        {
            return this.SelectMany(g => g);
        }

        public void Add(TElement item)
        {
            var key = readKey(item);

            FindOrCreateGroup(key).Add(item);
            elementAdded?.Invoke(item);
        }

        public IEnumerable<TKey> Keys => this.Select(i => i.Key);

        public GroupedObservableCollection<TKey, TElement> ReplaceWith(GroupedObservableCollection<TKey, TElement> replacementCollection, IEqualityComparer<TElement> itemComparer)
        {
            // First make sure that the top level group containers match
            var replacementKeys = replacementCollection.Keys.ToList();
            var currentKeys = new HashSet<TKey>(Keys);
            RemoveGroups(currentKeys.Except(replacementKeys));
            this.EnsureGroupsExist(replacementKeys);

            Debug.Assert(Keys.SequenceEqual(replacementCollection.Keys), "Expected this collection to have exactly the same keys in the same order at this point");

            for (var i = 0; i < replacementCollection.Count; i++)
            {
                MergeGroup(this[i], replacementCollection[i], itemComparer);
            }

            return this;
        }

        private static void MergeGroup(Grouping<TKey, TElement> current, Grouping<TKey, TElement> replacement, IEqualityComparer<TElement> itemComparer)
        {
            // Shortcut the matching and reordering process if the sequences are the same
            if (current.SequenceEqual(replacement, itemComparer))
            {
                return;
            }

            // First remove any items that are not present in the replacement
            var resultSet = new HashSet<TElement>(replacement, itemComparer);
            foreach (var toRemove in current.Except(resultSet, itemComparer).ToList())
            {
                current.Remove(toRemove);
            }

            Debug.Assert(new HashSet<TElement>(current, itemComparer).IsSubsetOf(replacement), "Expected the current group to be a subset of the replacement group");

            // var currentItemIndexes = current.Select((item, index) => new { item, index }).ToDictionary(i => i.item, i => i.index, itemComparer);
            // var replacementItemIndexes = replacement.Select((item, index) => new { item, index }).ToDictionary(i => i.item, i => i.index, itemComparer);

            var currentItemSet = new HashSet<TElement>(current, itemComparer);
            for (var i = 0; i < replacement.Count; i++)
            {
                var findElement = replacement[i];

                if (i >= current.Count || !itemComparer.Equals(current[i], findElement))
                {
                    if (currentItemSet.Contains(findElement))
                    {
                        // The current set already contains the item, but it's in a different position
                        // Find out where it is in the current collection and move it from there
                        // NOTE this isn't very optimal if large sets are being reordered a lot, but the general use case for
                        // this sort of list is that there is some inherent order that won't be changing.
                        var moved = false;
                        for (var j = i + 1; j < current.Count; j++)
                        {
                            if (itemComparer.Equals(current[j], findElement))
                            {
                                current.Move(i, j);
                                moved = true;
                                break;
                            }
                        }

                        Debug.Assert(moved, "Expected that the element should have been moved");
                    }
                    else
                    {
                        // This is a new element, insert it here
                        current.Insert(i, replacement[i]);
                    }
                }
            }
        }

        private void EnsureGroupsExist(IList<TKey> requiredKeys)
        {
            Debug.Assert(new HashSet<TKey>(Keys).IsSubsetOf(requiredKeys), "Expected this collection to contain no additional keys than the new collection at this point");

            if (Count == requiredKeys.Count)
            {
                // Nothing to do.
                return;
            }

            for (var i = 0; i < requiredKeys.Count; i++)
            {
                if (Count <= i || !this[i].Key.Equals(requiredKeys[i]))
                {
                    Insert(i, new Grouping<TKey, TElement>(requiredKeys[i]));
                }
            }
        }

        private void RemoveGroups(IEnumerable<TKey> keys)
        {
            var keySet = new HashSet<TKey>(keys);
            for (var i = Count - 1; i >= 0; i--)
            {
                if (keySet.Contains(this[i].Key))
                {
                    RemoveAt(i);
                }
            }
        }

        public bool Remove(TElement item)
        {
            var key = readKey(item);
            var group = TryFindGroup(key);
            var success = group != null && group.Remove(item);

            if (group != null && group.Count == 0)
            {
                Remove(group);
                lastEffectedGroup = null;
            }

            if (success)
                elementRemoved?.Invoke(item);

            return success;
        }

        private Grouping<TKey, TElement> TryFindGroup(TKey key)
        {
            if (lastEffectedGroup != null && lastEffectedGroup.Key.Equals(key))
            {
                return lastEffectedGroup;
            }

            var group = this.FirstOrDefault(i => i.Key.Equals(key));

            lastEffectedGroup = group;

            return group;
        }

        private Grouping<TKey, TElement> FindOrCreateGroup(TKey key)
        {
            if (lastEffectedGroup != null && lastEffectedGroup.Key.Equals(key))
            {
                return lastEffectedGroup;
            }

            var match = this.Select((group, index) => new { group, index }).FirstOrDefault(i => i.group.Key.CompareTo(key) >= 0);
            Grouping<TKey, TElement> result;
            if (match == null)
            {
                // Group doesn't exist and the new group needs to go at the end
                result = new Grouping<TKey, TElement>(key);
                Add(result);
            }
            else if (!match.group.Key.Equals(key))
            {
                // Group doesn't exist, but needs to be inserted before an existing one
                result = new Grouping<TKey, TElement>(key);
                Insert(match.index, result);
            }
            else
            {
                result = match.group;
            }

            lastEffectedGroup = result;

            return result;
        }
    }
}
