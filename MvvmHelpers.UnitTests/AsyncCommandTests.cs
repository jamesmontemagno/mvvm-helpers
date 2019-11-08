using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmHelpers.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmHelpers.UnitTests
{
	[TestClass]
	public class AsyncCommandTests
	{

		ICommand refreshCommand;
		public ICommand RefreshCommand => refreshCommand ??
				  (refreshCommand = new AsyncCommand<bool>((t) => ExecuteLoadCommand(true)));
		async Task ExecuteLoadCommand(bool forceRefresh)
		{
			await Task.Delay(1000);
		}

		#region Events
		protected event EventHandler TestEvent
		{
			add => TestWeakEventManager.AddEventHandler(value);
			remove => TestWeakEventManager.RemoveEventHandler(value);
		}
		#endregion

		#region Properties
		protected const int Delay = 500;
		protected WeakEventManager TestWeakEventManager { get; } = new WeakEventManager();
		#endregion

		#region Methods
		protected Task NoParameterTask() => Task.Delay(Delay);
		protected Task IntParameterTask(int delay) => Task.Delay(delay);
		protected Task StringParameterTask(string text) => Task.Delay(Delay);
		protected Task NoParameterImmediateNullReferenceExceptionTask() => throw new NullReferenceException();
		protected Task ParameterImmediateNullReferenceExceptionTask(int delay) => throw new NullReferenceException();

		protected async Task NoParameterDelayedNullReferenceExceptionTask()
		{
			await Task.Delay(Delay);
			throw new NullReferenceException();
		}

		protected async Task IntParameterDelayedNullReferenceExceptionTask(int delay)
		{
			await Task.Delay(delay);
			throw new NullReferenceException();
		}

		protected bool CanExecuteTrue(object parameter) => true;
		protected bool CanExecuteFalse(object parameter) => false;
		protected bool CanExecuteDynamic(object booleanParameter) => (bool)booleanParameter;
		#endregion

		[TestMethod]
		public void AsyncCommand_UsingICommand()
		{
			//Arrange
			RefreshCommand.Execute(true);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AsyncCommand_NullExecuteParameter()
		{
			//Arrange

			//Act

			//Assert
			new AsyncCommand(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AsyncCommandT_NullExecuteParameter()
		{
			//Arrange

			//Act

			//Assert
			new AsyncCommand<object>(null);
		}

		[TestMethod]
		public async Task AsyncCommand_ExecuteAsync_IntParameter_Test()
		{
			//Arrange
			var command = new AsyncCommand<int>(IntParameterTask);

			//Act
			await command.ExecuteAsync(500);
			await command.ExecuteAsync(default);

			//Assert

		}

		[TestMethod]
		public async Task AsyncCommand_ExecuteAsync_StringParameter_Test()
		{
			//Arrange
			var command = new AsyncCommand<string>(StringParameterTask);

			//Act
			await command.ExecuteAsync("Hello");
			await command.ExecuteAsync(default);

			//Assert

		}

		[TestMethod]
		public void AsyncCommand_Parameter_CanExecuteTrue_Test()
		{
			//Arrange
			var command = new AsyncCommand<int>(IntParameterTask, CanExecuteTrue);

			//Act

			//Assert
			Assert.IsTrue(command.CanExecute(null));
		}

		[TestMethod]
		public void AsyncCommand_Parameter_CanExecuteFalse_Test()
		{
			//Arrange
			var command = new AsyncCommand<int>(IntParameterTask, CanExecuteFalse);

			//Act

			//Assert
			Assert.IsFalse(command.CanExecute(null));
		}

		[TestMethod]
		public void AsyncCommand_NoParameter_CanExecuteTrue_Test()
		{
			//Arrange
			var command = new AsyncCommand(NoParameterTask, CanExecuteTrue);

			//Act

			//Assert
			Assert.IsTrue(command.CanExecute(null));
		}

		[TestMethod]
		public void AsyncCommand_NoParameter_CanExecuteFalse_Test()
		{
			//Arrange
			var command = new AsyncCommand(NoParameterTask, CanExecuteFalse);

			//Act

			//Assert
			Assert.IsFalse(command.CanExecute(null));
		}


		[TestMethod]
		public void AsyncCommand_CanExecuteChanged_Test()
		{
			//Arrange
			var canCommandExecute = false;
			var didCanExecuteChangeFire = false;

			var command = new AsyncCommand(NoParameterTask, commandCanExecute);
			command.CanExecuteChanged += handleCanExecuteChanged;

			void handleCanExecuteChanged(object sender, EventArgs e) => didCanExecuteChangeFire = true;
			bool commandCanExecute(object parameter) => canCommandExecute;

			Assert.IsFalse(command.CanExecute(null));

			//Act
			canCommandExecute = true;

			//Assert
			Assert.IsTrue(command.CanExecute(null));
			Assert.IsFalse(didCanExecuteChangeFire);

			//Act
			command.RaiseCanExecuteChanged();

			//Assert
			Assert.IsTrue(didCanExecuteChangeFire);
			Assert.IsTrue(command.CanExecute(null));
		}
	}
}
