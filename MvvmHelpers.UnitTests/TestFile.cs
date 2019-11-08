using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


#nullable enable

namespace MvvmHelpers.UnitTests
{
	public class TestFile
	{
		public AsyncCommand<string> MyCommand { get; }

		public TestFile()
		{
			MyCommand = new AsyncCommand<string>(Test, null);
		}
		
		Task Test(string test)
		{
		
			return Task.CompletedTask;
		}
	}
}
