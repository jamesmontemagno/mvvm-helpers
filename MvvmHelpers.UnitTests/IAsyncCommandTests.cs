using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmHelpers.Commands;
using MvvmHelpers.Interfaces;

namespace MvvmHelpers.UnitTests
{
	[TestClass]
	public class IAsyncCommandTests
	{
		[TestMethod]
		public void IAsyncCommand_CanRaiseCanExecuteChanged()
		{
			IAsyncCommand command = new AsyncCommand(null);
			command.RaiseCanExecuteChanged();
		}

		[TestMethod]
		public void IAsyncCommandT_CanRaiseCanExecuteChanged()
		{
			IAsyncCommand<string> command = new AsyncCommand<string>(null);
			command.RaiseCanExecuteChanged();
		}
	}
}