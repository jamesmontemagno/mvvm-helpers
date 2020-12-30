
using System;

namespace MvvmHelpers.UnitTests
{
    public class PersonViewModel : BaseViewModel
    {

    }

   public class Person : ObservableObject
   {
        public Action Changed { get; set; }

        public Func<string, string, bool> Validate {get;set;}

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

    public class UndoablePerson : UndoableObject
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

        UndoablePerson mother;
        public UndoablePerson Mother
        {
            get { return mother; }
            set
            {
                SetProperty(ref mother, value);
            }
        }

        ObservableRangeCollection<UndoableContactInfo> contactInfos;
        public ObservableRangeCollection<UndoableContactInfo> ContactInfos
        {
            get { return contactInfos; }
            set
            {
                SetProperty(ref contactInfos, value);
            }
        }

        public string SortName
        {
            get { return FirstName[0].ToString().ToUpperInvariant(); }
        }

        public UndoablePerson()
        {
            ContactInfos = new ObservableRangeCollection<UndoableContactInfo>();
        }
    }

    public class UndoableContactInfo : UndoableObject
    {
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref name, value);
            }
        }
        string val;
        public string Value
        {
            get { return val; }
            set
            {
                SetProperty(ref val, value);
            }
        }
    }

}
