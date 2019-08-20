﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


namespace MvvmHelpers
{
	/// <summary> 
	/// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed. 
	/// </summary> 
	/// <typeparam name="T"></typeparam> 
	public class ObservableRangeCollection<T> : ObservableCollection<T>
	{

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

			CheckReentrancy();

			if (notificationMode == NotifyCollectionChangedAction.Reset)
			{
				var raiseEvents = false;
				foreach (var i in collection)
				{
					Items.Add(i);
					raiseEvents = true;
				}

				if (raiseEvents)
				{
					OnPropertyChanged(new PropertyChangedEventArgs("Count"));
					OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}

				return;
			}

			var startIndex = Count;
			var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
			foreach (var i in changedItems)
				Items.Add(i);

			if (changedItems.Count == 0)
				return;

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, startIndex));
		}

		/// <summary> 
		/// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). NOTE: with notificationMode = Remove, removed items starting index is not set because items are not guaranteed to be consecutive.
		/// </summary> 
		public void RemoveRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Reset)
		{
			if (notificationMode != NotifyCollectionChangedAction.Remove && notificationMode != NotifyCollectionChangedAction.Reset)
				throw new ArgumentException("Mode must be either Remove or Reset for RemoveRange.", nameof(notificationMode));
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));

			CheckReentrancy();

			if (notificationMode == NotifyCollectionChangedAction.Reset)
			{
				var raiseEvents = false;
				foreach (var i in collection)
				{
					Items.Remove(i);
					raiseEvents = true;
				}

				if (raiseEvents)
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

				return;
			}

			var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
			for (var i = 0; i < changedItems.Count; i++)
			{
				if (!Items.Remove(changedItems[i]))
				{
					changedItems.RemoveAt(i); //Can't use a foreach because changedItems is intended to be (carefully) modified
					i--;
				}
			}

			if (changedItems.Count == 0)
				return;

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems, -1));
		}

		/// <summary> 
		/// Clears the current collection and replaces it with the specified item. 
		/// </summary> 
		public void Replace(T item) => ReplaceRange(new T[] { item });

		/// <summary> 
		/// Clears the current collection and replaces it with the specified collection. 
		/// </summary> 
		public void ReplaceRange(IEnumerable<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			Items.Clear();
			AddRange(collection, NotifyCollectionChangedAction.Reset);
		}

	}
}

