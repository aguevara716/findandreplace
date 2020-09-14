using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace FindAndReplace.Wpf.Backend.Collections
{
    public class ObservableHashSet<T> : ICollection<T>,
                                        INotifyCollectionChanged, 
                                        INotifyPropertyChanged
    {
        // Variables
        private readonly HashSet<T> hashSet;

        /// <summary>
        /// Gets the number of elements that are contained in a set
        /// </summary>
        public int Count { get { return hashSet.Count; } }
        public bool IsReadOnly { get { return false; } }

        // Constructors
        /// <summary>
        /// Initializes a new instance of the ObservableHashSet<T> class
        /// that is empty and uses the default equality comparer for the set type.
        /// </summary>
        public ObservableHashSet()
        {
            hashSet = new HashSet<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashSet{T}"/> class
        /// that uses the default equality comparer for the set type, contains elements copied
        /// from the specified collection, and has sufficient capacity to acommodate the
        /// number of elements copied.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements are copied to the ne set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is null
        /// </exception>
        public ObservableHashSet(IEnumerable<T> collection)
        {
            hashSet = new HashSet<T>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashSet{T}"/> class
        /// that is empty and uses the specified equality comparer for the set type.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when
        /// comparing values in the set, or null to use the default <see cref="EqualityComparer{T}"/>
        /// implementation for the set type.
        /// </param>
        public ObservableHashSet(IEqualityComparer<T> comparer)
        {
            hashSet = new HashSet<T>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashSet{T}"/> class
        /// that is empty, but has reserved space for <paramref name="capacity"/> items and uses the default
        /// equality comparer for the set type.
        /// </summary>
        /// <param name="capacity">
        /// The initial size of the <see cref="ObservableHashSet{T}"/>
        /// </param>
        public ObservableHashSet(int capacity)
        {
            hashSet = new HashSet<T>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashSet{T}"/> class
        /// that uses the specified equality <paramref name="comparer"/> for the set type, contains elements
        /// copied from the specified <paramref name="collection"/>, and has sufficient capacity to accommodate
        /// the number of elements copied.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements are copied to the new set.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when
        /// comparing values in the set, or null to use the default <see cref="EqualityComparer{T}"/>
        /// implementation for the set type.
        /// </param>
        public ObservableHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            hashSet = new HashSet<T>(collection, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashSet{T}"/> class
        /// that uses the specified equality <paramref name="comparer"/> for the set type, and has sufficient
        /// capacity to accommodate <paramref name="capacity"/> elements
        /// </summary>
        /// <param name="capacity">
        /// The initial size of the <see cref="ObservableHashSet{T}"/>
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when
        /// comparing values in the set, or null to use the default
        /// <see cref="IEqualityComparer{T}"/> implementation for the set type.
        /// </param>
        public ObservableHashSet(int capacity, IEqualityComparer<T> comparer)
        {
            hashSet = new HashSet<T>(capacity, comparer);
        }

        // Add
        public bool Add(T item)
        {
            var isAdded = hashSet.Add(item);
            if (isAdded)
            {
                OnCollectionReset();
                OnPropertyChanged(nameof(Count));
            }

            return isAdded;
        }

        public Dictionary<T, bool> AddRange(IEnumerable<T> items)
        {
            var itemsAddedStatusDictionary = new Dictionary<T, bool>();
            foreach (var item in items)
            {
                var isAdded = hashSet.Add(item);
                itemsAddedStatusDictionary.Add(item, isAdded);
            }

            var addedItems = itemsAddedStatusDictionary.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
            OnCollectionReset();
            OnPropertyChanged(nameof(Count));

            return itemsAddedStatusDictionary;
        }

        void ICollection<T>.Add(T item)
        {
            hashSet.Add(item);
        }

        // Clear
        public void Clear()
        {
            hashSet.Clear();
            var nccea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionReset();
            OnPropertyChanged(nameof(Count));
        }

        // Contains
        public bool Contains(T item)
        {
            var hasItem = hashSet.Contains(item);
            return hasItem;
        }

        // CopyTo
        public void CopyTo(T[] array, int arrayIndex)
        {
            hashSet.CopyTo(array, arrayIndex);
        }

        // Remove
        public bool Remove(T item)
        {
            var isRemoved = hashSet.Remove(item);
            if (isRemoved)
            {
                OnCollectionReset();
                OnPropertyChanged(nameof(Count));
            }
            return isRemoved;
        }

        public Dictionary<T, bool> RemoveRange(IEnumerable<T> items)
        {
            var itemsRemovalStatusDictionary = new Dictionary<T, bool>();
            foreach (var item in items)
            {
                var isRemoved = hashSet.Remove(item);
                itemsRemovalStatusDictionary.Add(item, isRemoved);
            }

            var removedItems = itemsRemovalStatusDictionary.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
            OnCollectionReset();
            OnPropertyChanged(nameof(Count));

            return itemsRemovalStatusDictionary;
        }

        // GetEnumerator
        public IEnumerator<T> GetEnumerator()
        {
            return hashSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hashSet.GetEnumerator();
        }

        // Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnCollectionReset()
        {
            var nccea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanged?.Invoke(this, nccea);
        }


    }
}
