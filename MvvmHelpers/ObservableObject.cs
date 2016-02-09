using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;

namespace MvvmHelpers
{
    /// <summary>
    /// Observable object with INotifyPropertyChanged implemented
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// The property watchers.
        /// </summary>
        readonly List<KeyValuePair<string, List<Tuple<Action<object>, Func<object, bool>>>>> PropertyWatchers 
        = new List<KeyValuePair<string, List<Tuple<Action<object>, Func<object, bool>>>>>();

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
        /// <param name="backingStore">Backing store.</param>
        /// <param name="value">Value.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="onChanged">On changed.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected bool SetProperty<T>(
            ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);

            var watchers = PropertyWatchers.FirstOrDefault(pw => pw.Key == propertyName);

            if (!watchers.Equals(default(KeyValuePair<string, List<Tuple<Action<object>, Func<object, bool>>>>)))
            {
                foreach (Tuple<Action<object>, Func<object, bool>> watcher in watchers.Value)
                {
                    if (watcher.Item2(backingStore))
                        watcher.Item1(backingStore);
                }
            }
            return true;
        }

        /// <summary>
        /// Watchs the property.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="action">Action.</param>
        /// <param name="filter">Filter.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void WatchProperty<T>(string propertyName, Action<T> action, Func<T, bool> filter = null)
        {
            if (PropertyWatchers.All(pw => pw.Key != propertyName))
                PropertyWatchers.Add(new KeyValuePair<string, List<Tuple<Action<object>, Func<object, bool>>>>(propertyName, new List<Tuple<Action<object>, Func<object, bool>>>()));

            PropertyWatchers.Single(pw => pw.Key == propertyName).Value?.Add(new Tuple<Action<object>, Func<object, bool>>(value => action((T)value),  value => filter((T)value)));
        }

        /// <summary>
        /// Clears the watchers.
        /// </summary>
        public void ClearWatchers() => PropertyWatchers.Clear();


        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}

