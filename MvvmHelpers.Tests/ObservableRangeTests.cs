using NUnit.Framework;
using System;
using System.Linq;

namespace MvvmHelpers.Tests
{
    [TestFixture()]
    public class ObservableRangeTests
    {
        class Person
        {
            public string FirstName {get;set;}
            public string LastName {get;set;}
            public string SortName 
            {
                get { return FirstName[0].ToString().ToUpperInvariant(); }
            }
        }

        [Test()]
        public void GroupingTestCase()
        {

            var grouped = new ObservableRangeCollection<Grouping<string, Person>>();
            var people = new[]
            { 
                new Person { FirstName = "Joseph", LastName = "Hill" },
                new Person { FirstName = "James", LastName = "Montemagno" },
                new Person { FirstName = "Pierce", LastName = "Boggan" },
            };

            var sorted = from person in people
                                  orderby person.LastName
                                  group person by person.SortName into personGroup
                                  select new Grouping<string, Person>(personGroup.Key, personGroup);

            grouped.AddRange(sorted);
           
        }
    }
}

