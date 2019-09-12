using MvvmHelpers.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

// Lovely code from our good friend John Tririet
// https://johnthiriet.com/mvvm-going-async-with-async-command


namespace MvvmHelpers.Commands
{
	/// <summary>
	/// Implementation of an Async Command
	/// </summary>
	public class AsyncCommand : IAsyncCommand
	{
		readonly Func<Task> execute;
		readonly Func<object, bool> canExecute;
		readonly Action<Exception> onException;
		readonly bool continueOnCapturedContext;
		readonly WeakEventManager weakEventManager = new WeakEventManager();

		public AsyncCommand(Func<Task> execute,
							Func<object, bool> canExecute = null,
							Action<Exception> onException = null,
							bool continueOnCapturedContext = false)
		{
			this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
			this.canExecute = canExecute;
			this.onException = onException;
			this.continueOnCapturedContext = continueOnCapturedContext;
		}

		public event EventHandler CanExecuteChanged
		{
			add { weakEventManager.AddEventHandler(value); }
			remove { weakEventManager.RemoveEventHandler(value); }
		}

		public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

		public Task ExecuteAsync() => execute();

		public void RaiseCanExecuteChanged() => weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));

		#region Explicit implementations
		void ICommand.Execute(object parameter) => ExecuteAsync().SafeFireAndForget(onException, continueOnCapturedContext);
		#endregion
	}
	/// <summary>
	/// Implementation of a generic Async Command
	/// </summary>
	public class AsyncCommand<T> : IAsyncCommand<T>
	{

		readonly Func<T, Task> execute;
		readonly Func<object, bool> canExecute;
		readonly Action<Exception> onException;
		readonly bool continueOnCapturedContext;
		readonly WeakEventManager weakEventManager = new WeakEventManager();

		public AsyncCommand(Func<T, Task> execute,
							Func<object, bool> canExecute = null,
							Action<Exception> onException = null,
							bool continueOnCapturedContext = false)
		{
			this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
			this.canExecute = canExecute;
			this.onException = onException;
			this.continueOnCapturedContext = continueOnCapturedContext;
		}

		public event EventHandler CanExecuteChanged
		{
			add { weakEventManager.AddEventHandler(value); }
			remove { weakEventManager.RemoveEventHandler(value); }
		}

		public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

		public Task ExecuteAsync(T parameter) => execute(parameter);

		public void RaiseCanExecuteChanged() => weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));

		#region Explicit implementations

		void ICommand.Execute(object parameter)
		{
			if (CommandUtils.IsValidCommandParameter<T>(parameter))
				ExecuteAsync((T)parameter).SafeFireAndForget(onException, continueOnCapturedContext);

		}
		#endregion
	}
}
