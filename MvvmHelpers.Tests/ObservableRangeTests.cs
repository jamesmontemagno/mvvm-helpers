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

        [Test()]
        public void RemoveRange_RemoveTest()
        {
            ObservableRangeCollection<int> collection = new ObservableRangeCollection<int>();
            int[] toAdd = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3 };
            int[] toRemove = new[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 0, 0 };
            collection.AddRange(toAdd);
            collection.CollectionChanged += (s, e) =>
            {
                if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    Assert.Fail("RemoveRange didn't use Remove like requested.");
                if (e.OldItems == null)
                    Assert.Fail("OldItems should not be null.");
                int[] expected = new int[] { 1, 1, 2, 2, 3, 3, 4, 5, 5, 6, 7, 8, 9, 9 };
                if (expected.Length != e.OldItems.Count)
                    Assert.Fail("Expected and actual OldItems don't match.");
                for (int i = 0; i < expected.Length; i++)
                {
                    if (expected[i] != (int)e.OldItems[i])
                        Assert.Fail("Expected and actual OldItems don't match.");
                }
            };
            collection.RemoveRange(toRemove, System.Collections.Specialized.NotifyCollectionChangedAction.Remove);
        }
    }
}

