
using System;

namespace MvvmHelpers.Tests
{

    public class PersonViewModel : BaseViewModel
    {

    }

    public class Person : ObservableObject
    {
        public Action Changed { get; set; }
        string firstName;
        public string FirstName
        {
            get { return firstName; }
            set
            {
                SetProperty(ref firstName, value, onChanged: Changed);
            }
        }
        string lastName;
        public string LastName
        {
            get { return lastName; }
            set
            {
                SetProperty(ref lastName, value, onChanged: Changed);
            }
        }
        public string SortName
        {
            get { return FirstName[0].ToString().ToUpperInvariant(); }
        }

    }
}
