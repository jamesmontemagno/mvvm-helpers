using System;
using System.Windows.Input;

namespace MvvmHelpers.Commands
{

	/// <summary>
	/// Generic Implementation of ICommand
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Command<T> : Command
	{
		public Command(Action<T> execute)
			: base(o =>
			{
				if (CommandUtils.IsValidCommandParameter<T>(o))
					execute((T)o);
			})
		{
			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}
		}

		public Command(Action<T> execute, Func<T, bool> canExecute)
			: base(o =>
			{
				if (CommandUtils.IsValidCommandParameter<T>(o))
					execute((T)o);
			}, o =>
			{
				return CommandUtils.IsValidCommandParameter<T>(o) && canExecute((T)o);
			})
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));
			if (canExecute == null)
				throw new ArgumentNullException(nameof(canExecute));
		}
	}

	/// <summary>
	/// Implementation of ICommand
	/// </summary>
	public class Command : ICommand
	{
		readonly Func<object, bool> canExecute;
		readonly Action<object> execute;
		readonly WeakEventManager weakEventManager = new WeakEventManager();

		public Command(Action<object> execute)
		{
			this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
		}

		public Command(Action execute) : this(o => execute())
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));
		}

		public Command(Action<object> execute, Func<object, bool> canExecute) : this(execute)
		{
			this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
		}

		public Command(Action execute, Func<bool> canExecute) : this(o => execute(), o => canExecute())
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));
			if (canExecute == null)
				throw new ArgumentNullException(nameof(canExecute));
		}

		public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

		public event EventHandler CanExecuteChanged
		{
			add { weakEventManager.AddEventHandler(value); }
			remove { weakEventManager.RemoveEventHandler(value); }
		}

		public void Execute(object parameter) => execute(parameter);

		public void RaiseCanExecuteChanged() => weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
	}
}
