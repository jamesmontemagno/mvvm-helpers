using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

// Lovely code from our good friend John Tririet
// https://johnthiriet.com/mvvm-going-async-with-async-command


namespace MvvmHelpers
{
    /// <summary>
    /// Implementation of an Async Command
    /// </summary>
    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        bool isExecuting;
        readonly Func<Task> execute;
        readonly Func<bool> canExecute;
        readonly IErrorHandler errorHandler;

        public AsyncCommand(
            Func<Task> execute,
            Func<bool> canExecute = null,
            IErrorHandler errorHandler = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.errorHandler = errorHandler;
        }

        public bool CanExecute() => !isExecuting && (canExecute?.Invoke() ?? true);

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    isExecuting = true;
                    await execute();
                }
                finally
                {
                    isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter) => ExecuteAsync().FireAndForgetSafeAsync(errorHandler);
        #endregion
    }

    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        public event EventHandler CanExecuteChanged;

        bool isExecuting;
        readonly Func<T, Task> execute;
        readonly Func<T, bool> canExecute;
        readonly IErrorHandler errorHandler;

        public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, IErrorHandler errorHandler = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.errorHandler = errorHandler;
        }

        public bool CanExecute(T parameter) => !isExecuting && (canExecute?.Invoke(parameter) ?? true);

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    isExecuting = true;
                    await execute(parameter);
                }
                finally
                {
                    isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter) => CanExecute((T)parameter);

        void ICommand.Execute(object parameter) => ExecuteAsync((T)parameter).FireAndForgetSafeAsync(errorHandler);
        #endregion
    }
}
