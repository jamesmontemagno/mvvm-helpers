using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MvvmHelpers
{
    /// <summary> 
    /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed. 
    /// </summary> 
    /// <typeparam name="T"></typeparam> 
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {

        private const string CountName = nameof(Count);
        private const string IndexerName = "Item[]";

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
        /// </summary> 
        public ObservableRangeCollection()
            : base()
        {
        }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
        /// </summary> 
        /// <param name="collection">collection: The collection from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        public ObservableRangeCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
        {
            if (notificationMode != NotifyCollectionChangedAction.Add && notificationMode != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Mode must be either Add or Reset for AddRange.", nameof(notificationMode));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection is ICollection<T> list)
            {
               if (list.Count == 0) return;
            }
            else if (!collection.Any()) return;
            else list = new List<T>(collection);

            CheckReentrancy();  

            int startIndex = Count;            
            foreach (var i in collection)
                Items.Add(i);

            NotifyProperties();
            if (notificationMode == NotifyCollectionChangedAction.Reset)
               OnCollectionReset();
            else
              OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list as IList ?? list.ToList(), startIndex));
        }

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when the list is being cleared;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void ClearItems()
        {
            if (Count > 0)
                base.ClearItems();
        }


        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T).
        /// </summary> 
        public virtual void RemoveRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Remove)
        {
            if (notificationMode != NotifyCollectionChangedAction.Remove && notificationMode != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Mode must be either Remove or Reset for RemoveRange.", nameof(notificationMode));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (Count == 0) return;
            if (collection is ICollection<T> list && list.Count == 0) return;
            else if (!collection.Any()) return;

            CheckReentrancy();          
            if (notificationMode == NotifyCollectionChangedAction.Reset)
            {
                foreach (var i in collection)
                    Items.Remove(i);

                OnCollectionReset();
                NotifyProperties();
                return;
            }         

            var removed = new Dictionary<int, List<T>>();
            var curSegmentIndex = -1;
            foreach (var item in collection)
            {
                var index = IndexOf(item);
                if (index < 0) continue;

                Items.RemoveAt(index);

                if (!removed.TryGetValue(index - 1, out var segment) && !removed.TryGetValue(index, out segment))
                {
                    curSegmentIndex = index;
                    removed[index] = new List<T> { item };
                }
                else
                    segment.Add(item);
            }

            if (Count == 0)
                OnCollectionReset();
            else
                foreach (var segment in removed)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, segment.Value, segment.Key));

            NotifyProperties();
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified item. 
        /// </summary> 
        public void Replace(T item) => ReplaceRange(new T[] { item });

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection. 
        /// </summary> 
        /// <param name="noDuplicates">
        /// Sets whether we should ignore items already in the collection when adding items.
        /// false (default) items already existing in the collection will be reused to increase performance.
        /// true - perform regular clear and add, and notify about a reset when done.
        /// </param>
        public void ReplaceRange(IEnumerable<T> collection, bool reset = false)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection is IList<T> list)
            {
                if (list.Count == 0)
                {
                    Clear();
                    return;
                }
            }
            else if (!collection.Any()) 
            {
                Clear();
                return;
            }
            else list = new List<T>(collection);

            CheckReentrancy();

            if (reset)
            {
                Items.Clear();
                AddRange(collection, NotifyCollectionChangedAction.Reset);
                return;
            }

            var oldCount = Count;
            var lCount = list.Count;

            for (int i = 0; i < Math.Max(Count, lCount); i++)
            {
                if (i < Count && i < lCount)
                {
                    T old = this[i], @new = list[i];
                    if (Equals(old, @new))
                        continue;
                    else
                    {
                        Items[i] = @new;
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, @new, @old, i));
                    }
                }
                else if (Count > lCount)
                {
                    var removed = new Stack<T>();
                    for (var j = Count - 1; j >= i; j--)
                    {
                        removed.Push(this[j]);
                        Items.RemoveAt(j);
                    }
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed.ToList(), i));
                    break;
                }
                else
                {
                    var added = new List<T>();
                    for (int j = i; j < list.Count; j++)
                    {
                        var @new = list[j];
                        Items.Add(@new);
                        added.Add(@new);
                    }
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added, i));
                    break;
                }
            }

            NotifyProperties(Count != oldCount);
        }

        void OnCollectionReset() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        void NotifyProperties(bool count = true)
        {
            if (count)
                OnPropertyChanged(new PropertyChangedEventArgs(CountName));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
        }
    }
}
