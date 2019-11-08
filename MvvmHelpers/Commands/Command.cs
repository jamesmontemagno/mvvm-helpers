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
		/// <summary>
		/// Command that takes an action to execute
		/// </summary>
		/// <param name="execute">The action to execute of type T</param>
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

		/// <summary>
		/// Command that takes an action to execute
		/// </summary>
		/// <param name="execute">The action to execute of type T</param>
		/// <param name="canExecute">Function to call to determine if it can be executed.</param>
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
		readonly Func<object, bool>? canExecute;
		readonly Action<object> execute;
		readonly WeakEventManager weakEventManager = new WeakEventManager();

		/// <summary>
		/// Command that takes an action to execute.
		/// </summary>
		/// <param name="execute">Action to execute.</param>
		public Command(Action<object> execute)
		{
			this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
		}

		/// <summary>
		/// Command that takes an action to execute.
		/// </summary>
		/// <param name="execute">Action to execute.</param>
		public Command(Action execute) : this(o => execute())
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));
		}

		/// <summary>
		/// Command that takes an action to execute.
		/// </summary>
		/// <param name="execute">Action to execute.</param>
		/// <param name="canExecute">Function to determine if can execute.</param>
		public Command(Action<object> execute, Func<object, bool>? canExecute) : this(execute)
		{
			this.canExecute = canExecute;
		}

		/// <summary>
		/// Command that takes an action to execute.
		/// </summary>
		/// <param name="execute">Action to execute.</param>
		/// <param name="canExecute">Function to determine if can execute.</param>
		public Command(Action execute, Func<bool>? canExecute) : this(o => execute())
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));

			if (canExecute != null)
				this.canExecute = o => canExecute();
		}

		/// <summary>
		/// Invoke the CanExecute method to determine if it can be executed.
		/// </summary>
		/// <param name="parameter">Parameter to test and pass to CanExecute.</param>
		/// <returns>If it can be executed.</returns>
		public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

		/// <summary>
		/// Event handler raised when CanExecute changes.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { weakEventManager.AddEventHandler(value); }
			remove { weakEventManager.RemoveEventHandler(value); }
		}

		/// <summary>
		/// Execute the command with or without a parameter.
		/// </summary>
		/// <param name="parameter">Parameter to pass to execute method.</param>
		public void Execute(object parameter) => execute(parameter);

		/// <summary>
		/// Manually raise a CanExecuteChanged event.
		/// </summary>
		public void RaiseCanExecuteChanged() => weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
	}
}
