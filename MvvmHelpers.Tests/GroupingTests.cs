using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmHelpers.Tests
{
    [TestFixture()]
    public class GroupingTests
    {
        [Test()]
        public void Grouping()
        {

            var grouped = new ObservableRangeCollection<Grouping<string, Person>>();
            var people = new[]
            {
                new Person { FirstName = "Joseph", LastName = "Hill" },
                new Person { FirstName = "James", LastName = "Montemagno" },
                new Person { FirstName = "Pierce", LastName = "Boggan" },
            };

            var sorted = from person in people
                         orderby person.FirstName
                         group person by person.SortName into personGroup
                         select new Grouping<string, Person>(personGroup.Key, personGroup);

            grouped.AddRange(sorted);

           

            Assert.AreEqual(2, grouped.Count, "There should be 2 groups");
            Assert.AreEqual("J", grouped[0].Key, "Key for group 0 should be J");
            Assert.AreEqual(2, grouped[0].Count, "There should be 2 items in group 0");
            Assert.AreEqual(1, grouped[1].Count, "There should be 1 items in group 1");


            Assert.AreEqual(2, grouped[0].Items.Count, "There should be 2 items in group 0");
            Assert.AreEqual(1, grouped[1].Items.Count, "There should be 1 items in group 1");

        }

        [Test()]
        public void GroupingSubKey()
        {

            var grouped = new ObservableRangeCollection<Grouping<string, string, Person>>();
            var people = new[]
            {
                new Person { FirstName = "Joseph", LastName = "Hill" },
                new Person { FirstName = "James", LastName = "Montemagno" },
                new Person { FirstName = "Pierce", LastName = "Boggan" },
            };

            var sorted = from person in people
                         orderby person.FirstName
                         group person by person.SortName into personGroup
                         select new Grouping<string, string, Person>(personGroup.Key, personGroup.Key, personGroup);

            grouped.AddRange(sorted);

            Assert.AreEqual(2, grouped.Count, "There should be 2 groups");
            Assert.AreEqual("J", grouped[0].SubKey, "Key for group 0 should be J");
            Assert.AreEqual("J", grouped[0].Key, "Key for group 0 should be J");
            Assert.AreEqual(2, grouped[0].Count, "There should be 2 items in group 0");
            Assert.AreEqual(1, grouped[1].Count, "There should be 1 items in group 1");
            Assert.AreEqual(2, grouped[0].Items.Count, "There should be 2 items in group 0");
            Assert.AreEqual(1, grouped[1].Items.Count, "There should be 1 items in group 1");

        }
    }
}
