using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace MvvmHelpers
{
	public abstract class UndoableObject : INotifyPropertyChanged
	{
		Dictionary<string, object> propertySetters;

		long lastTriggeredUndoEventId;

		/// <summary>
		/// Notification that a property of this entity has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Notification that this entity or one of its references has registered an undo action.
		/// By default, these notifications are ignored. However, you can create an undo manager
		/// by listening for these events and recording them.
		/// </summary>
		public event EventHandler<UndoEventArgs> RegisteredUndo;

		/// <summary>
		/// Sets the property if it has changed and registers an undo action for it.
		/// </summary>
		protected bool SetProperty<T>(ref T backingField, T newValue, Action action = null, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(backingField, newValue))
				return false;

			var oldValue = backingField;
			Unbind(propertyName, oldValue, false);
			backingField = newValue;
			Bind(propertyName, newValue, false);

			action?.Invoke();
			OnPropertyChanged(propertyName);

			return true;
		}

		/// <summary>
		/// Sets the property if it has changed and registers an undo action for it.
		/// </summary>
		protected bool SetUndoableProperty<T>(ref T backingField, T newValue, Action action = null, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(backingField, newValue))
				return false;

			var oldValue = backingField;
			Unbind(propertyName, oldValue, true);
			backingField = newValue;
			Bind(propertyName, newValue, true);

			var ru = RegisteredUndo;
			if (ru != null)
			{
				var e = new UndoEventArgs("Change {0}".Localize(propertyName.Localize()),
										   () => GetPropertySetter<T>(propertyName)(oldValue));
				Interlocked.Exchange(ref lastTriggeredUndoEventId, e.EventId);
				ru(this, e);
			}

			action?.Invoke();
			OnPropertyChanged(propertyName);

			return true;
		}

		void Bind(string propertyName, object value, bool undoable)
		{
			if (value == null) return;

			if (value is INotifyPropertyChanged oe)
			{
				BindReference(propertyName, oe, undoable);
			}
			else if (value is INotifyCollectionChanged oce)
			{
				BindReferenceList(propertyName, oce, undoable);
			}
		}

		void Unbind(string propertyName, object value, bool undoable)
		{
			if (value == null) return;

			if (value is INotifyPropertyChanged oe)
			{
				UnbindReference(propertyName, oe, undoable);
			}
			else if (value is INotifyCollectionChanged oce)
			{
				UnbindReferenceList(propertyName, oce, undoable);
			}
		}

		/// <summary>
		/// Triggers the <see cref="PropertyChanged"/> event.
		/// Override to add your own handling of property changed events.
		/// </summary>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			var pc = PropertyChanged;
			if (pc != null)
			{
				pc(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Gets an action to set the named property.
		/// Calling this method is much more efficient than using reflection.
		/// </summary>
		protected Action<T> GetPropertySetter<T>(string propertyName)
		{
			if (propertySetters != null)
			{
				lock (propertySetters)
				{
					if (propertySetters.TryGetValue(propertyName, out var o) && o is Action<T> cachedAction)
						return cachedAction;
				}
			}

            var property = GetType().GetRuntimeProperty(propertyName);
            var method = property?.SetMethod;
			if (method == null)
				throw new InvalidOperationException($"Cannot find property setter for {GetType().FullName}.{propertyName}");

			var newAction = (Action<T>)method.CreateDelegate(typeof(Action<T>), this);
			if (propertySetters == null)
			{
				propertySetters = new Dictionary<string, object>();
			}
			lock (propertySetters)
			{
				propertySetters[propertyName] = newAction;
			}
			return newAction;
		}

		/// <summary>
		/// Attach any event handlers and update any state needed
		/// when the given entity is referenced by this entity.
		/// The default implementation subscribes to the <see cref="RegisteredUndo"/> and <see cref="PropertyChanged"/> events.
		/// </summary>
		protected virtual void BindReference(string propertyName, INotifyPropertyChanged reference, bool undoable)
		{
			reference.PropertyChanged += OnReferencePropertyChanged;
			if (undoable && reference is UndoableObject uref)
				uref.RegisteredUndo += OnReferenceRegisteredUndo;
		}

		/// <summary>
		/// Detach any event handlers and update any state needed
		/// when the given entity is no longer referenced by the property on this entity.
		/// The default implementation unsubscribes from the <see cref="RegisteredUndo"/> and <see cref="PropertyChanged"/> events.
		/// </summary>
		protected virtual void UnbindReference(string propertyName, INotifyPropertyChanged reference, bool undoable)
		{
			reference.PropertyChanged -= OnReferencePropertyChanged;
			if (undoable && reference is UndoableObject uref)
				uref.RegisteredUndo -= OnReferenceRegisteredUndo;
		}

		void BindReferenceList(string propertyName, INotifyCollectionChanged references, bool undoable)
		{
			if (undoable)
				references.CollectionChanged += OnUndoableReferenceListChanged;
			else
				references.CollectionChanged += OnBoringReferenceListChanged;

			if (references is System.Collections.IEnumerable e)
			{
				foreach (var i in e)
				{
					Bind("", i, undoable);
				}
			}
		}

		void UnbindReferenceList(string propertyName, INotifyCollectionChanged references, bool undoable)
		{
			if (undoable)
				references.CollectionChanged -= OnUndoableReferenceListChanged;
			else
				references.CollectionChanged -= OnBoringReferenceListChanged;

			if (references is System.Collections.IEnumerable e)
			{
				foreach (var i in e)
				{
					Unbind("", i, undoable);
				}
			}
		}

		/// <summary>
		/// Event handler for when a referenced entity registers an undo action.
		/// The default implementation forwards the event using <see cref="RegisteredUndo"/>.
		/// </summary>
		void OnReferenceRegisteredUndo(object sender, UndoEventArgs e)
		{
			// Relay undo events
			var lastId = Interlocked.Exchange(ref lastTriggeredUndoEventId, e.EventId);
			if (lastId != e.EventId)
				RegisteredUndo?.Invoke(sender, e);
		}

		/// <summary>
		/// Event handler for when a referenced entity's property has changed.
		/// The default implementation does nothing.
		/// </summary>
		protected virtual void OnReferencePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		/// <summary>
		/// Event handler for when the structure of a list property changes.
		/// This includes adding, removing, and updating (change the reference, not mutating) items.
		/// </summary>
		protected virtual void OnReferenceListChanged(object sender, NotifyCollectionChangedEventArgs e, bool undoable)
		{
			//
			// Unbind the old and bind the new items
			//
			if (e.OldItems != null)
			{
				foreach (var i in e.OldItems)
				{
					Unbind("", i, undoable);
				}
			}
			if (e.NewItems != null)
			{
				foreach (var i in e.NewItems)
				{
					Bind("", i, undoable);
				}
			}

			//
			// Convert collection changed events into undoable actions
			//
			if (!undoable || !(sender is System.Collections.IList list))
				return;

			var ru = RegisteredUndo;
			if (ru != null)
			{
				Action undo = null;
				string message = null;
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						undo = () => {
							var n = e.NewItems.Count;
							for (var i = 0; i < n; i++)
								list.Remove(e.NewItems[i]);
						};
						message = e.NewItems.Count > 1 ? "Add Many".Localize() : "Add {0}".Localize(e.NewItems[0]);
						break;
					case NotifyCollectionChangedAction.Move:
						throw new NotSupportedException("Cannot handle moving items in a reference list.");
					case NotifyCollectionChangedAction.Remove:
						undo = () => {
							var n = e.OldItems.Count;
							for (var i = 0; i < n; i++)
								list.Insert(e.OldStartingIndex + i, e.OldItems[i]);
						};
						message = "Remove".Localize();
						break;
					case NotifyCollectionChangedAction.Replace:
						undo = () => {
							var nn = e.NewItems.Count;
							for (var i = 0; i < nn; i++)
								list.Remove(e.NewItems[i]);
							var on = e.OldItems.Count;
							for (var i = 0; i < on; i++)
								list.Insert(e.OldStartingIndex + i, e.OldItems[i]);
						};
						message = "Replace".Localize();
						break;
					case NotifyCollectionChangedAction.Reset:
						undo = () => {
							var n = e.OldItems.Count;
							for (var i = 0; i < n; i++)
								list.Insert(e.OldStartingIndex + i, e.OldItems[i]);
						};
						message = Localization.Localize("Reset");
						break;
				}
				if (undo != null)
					ru(this, new UndoEventArgs(message, undo));
			}
		}

		void OnUndoableReferenceListChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnReferenceListChanged(sender, e, true);
		}

		void OnBoringReferenceListChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnReferenceListChanged(sender, e, false);
		}

		public class UndoEventArgs : EventArgs
		{
			static long nextEventId = 1;

			/// <summary>
			/// A process-unique id for this undo event. This is used to prevent
			/// recursive and duplicate broadcasts of undo events for cyclic graphs.
			/// </summary>
			/// <value>The event identifier.</value>
			public long EventId { get; }

			/// <summary>
			/// Gets a summary of what caused this undo event to be emitted.
			/// </summary>
			public string Message { get; }

			/// <summary>
			/// The action to execute to undo whatever caused this event to be emitted.
			/// </summary>
			public Action UndoAction { get; }

			public UndoEventArgs(string message, Action undoAction)
			{
				EventId = Interlocked.Increment(ref nextEventId);
				Message = message;
				UndoAction = undoAction;
			}

			public override string ToString() => $"#{EventId} {Message}";
		}
	}

    static class Localization
    {
        public static string Localize(this string message)
        {
            return message;
        }

        public static string Localize(this string format, params object[] args)
        {
            return string.Format (format, args);
        }
    }
}
