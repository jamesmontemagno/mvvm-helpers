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
        /// Sets the property.
        /// </summary>
        /// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
        /// <param name="backingStore">Backing store.</param>
        /// <param name="value">Value.</param>
        /// <param name="validateValue">Validates value.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="onChanged">On changed.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected virtual bool SetProperty<T>(
            ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null,
            Func<T, T, bool> validateValue = null)
        {
            //if value didn't change
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            //if value changed but didn't validate
            if (validateValue != null && !validateValue(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets a property in class
        /// </summary>
        /// <typeparam name="T">Type of model class</typeparam>
        /// <typeparam name="X">Type of property to update</typeparam>
        /// <param name="backingStore">Model object</param>
        /// <param name="value">Value of property</param>
        /// <param name="propertyName">Name of property to update</param>
        /// <param name="onChanged">Event to launch</param>
        /// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
        protected virtual bool SetProperty<T, X>(
            ref T backingStore,
            X value,
            string propertyName,
            Action onChanged = null) where T : class
        {
            var property = typeof(T).GetProperties().FirstOrDefault(f => f.Name.ToLowerInvariant() == propertyName.ToLowerInvariant());
            if (property == null) return false;

            var originalValue = property.GetValue(backingStore)?.ToString() ?? "";
            var newValue = value?.ToString() ?? "";

            // if value didn't change
            if (originalValue == newValue) return false;

            // Assign values
            property.SetValue(backingStore, value);

            // Event launching
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}

