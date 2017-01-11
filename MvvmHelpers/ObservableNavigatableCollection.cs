using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvvmHelpers
{
    /// <summary>
    /// Represents a dynamic data collection that provides the option to easily naviage the items in the collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableNavigatableCollection<T> : ObservableCollection<T>
    {
        private int currentIndex = 0;

        /// <summary>
        /// Determines if the collection has items.
        /// </summary>
        public bool HasItems
        {
            get { return this != null && this.Count > 0; }
        }

        /// <summary>
        /// Determines if the item is at the start of the collection.
        /// </summary>
        public bool IsAtStart
        {
            get { return currentIndex == 0; }
        }

        /// <summary>
        /// Determines if the item is at the end of the collection.
        /// </summary>
        public bool IsAtEnd
        {
            get { return currentIndex == this.Items.Count - 1; }
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
        /// </summary>
        public ObservableNavigatableCollection()
        {
        }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
        /// </summary> 
        /// <param name="collection">collection: The collection from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        public ObservableNavigatableCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        /// Moves to the first item in the collection.
        /// </summary>
        /// <returns></returns>
        public T MoveFirst()
        {
            if (this.Items == null)
                return default(T);

            currentIndex = 0;
            return this[currentIndex];
        }

        /// <summary>
        /// Moves to the previous item in the collection.
        /// </summary>
        /// <returns></returns>
        public T MovePrevious()
        {
            if (this.Items == null)
                return default(T);

            if (this.currentIndex >= 1)
                currentIndex--;

            return this[currentIndex];
        }

        /// <summary>
        /// Moves to the next item in the collection.
        /// </summary>
        /// <returns></returns>
        public T MoveNext()
        {
            if (this.Items == null)
                return default(T);

            if (this.currentIndex <= this.Items.Count - 1)
                currentIndex++;

            return this[currentIndex];
        }

        /// <summary>
        /// Moves to the last item in the collection.
        /// </summary>
        /// <returns></returns>
        public T MoveLast()
        {
            if (this.Items == null)
                return default(T);

            currentIndex = this.Items.Count - 1;
            return this[currentIndex];
        }
    }
}
