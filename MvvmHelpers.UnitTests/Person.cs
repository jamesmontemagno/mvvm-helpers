
using System;

namespace MvvmHelpers.UnitTests
{

	public class PersonViewModel : BaseViewModel
	{

	}

	public class Person : ObservableObject
	{
		public Action Changed { get; set; }

		public Func<string, string, bool> Validate { get; set; }

		string firstName;
		public string FirstName
		{
			get { return firstName; }
			set
			{
				SetProperty(ref firstName, value, onChanged: Changed, validateValue: Validate);
			}
		}
		string lastName;
		public string LastName
		{
			get { return lastName; }
			set
			{
				SetProperty(ref lastName, value, onChanged: Changed, validateValue: Validate);
			}
		}


		public string SortName
		{
			get { return FirstName[0].ToString().ToUpperInvariant(); }
		}

	}
}
