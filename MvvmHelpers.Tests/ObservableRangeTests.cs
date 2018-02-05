using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;

namespace MvvmHelpers.Tests
{
    [TestFixture()]
    public class ObservableRangeTests
    {
        [Test()]
        public void AddRange()
        {
            ObservableRangeCollection<int> collection = new ObservableRangeCollection<int>();
            int[] toAdd = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3 };

            collection.CollectionChanged += (s, e) =>
            {
                Assert.AreEqual(e.Action,
                               NotifyCollectionChangedAction.Add,
                               "AddRange didn't use Add like requested.");

                Assert.IsNull(e.OldItems, "OldItems should be null.");

                Assert.AreEqual(toAdd.Length,
                                e.NewItems.Count,
                                "Expected and actual OldItems don't match.");

                for (int i = 0; i < toAdd.Length; i++)
                {
                    Assert.AreEqual(toAdd[i], (int)e.NewItems[i],
                        "Expected and actual NewItems don't match.");
                }
            };
            collection.AddRange(toAdd);
        }

        [Test]
        public void ReplaceRange()
        {
            int[] toRem = new[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 0, 0 };
            int[] toAdd = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3 };
            ObservableRangeCollection<int> collection = new ObservableRangeCollection<int>(toRem.ToList());

            var expectedCanges = toAdd.Where((item, index) => toRem[index] != item);
            var expectedRaiseCount = expectedCanges.Count();
            if (toRem.Length != toAdd.Length)
                expectedRaiseCount++;

            var changes = new List<int>();
            var raiseCount = 0;
            collection.CollectionChanged += (sender, e) =>
            {
                raiseCount++;
                if (e.Action == NotifyCollectionChangedAction.Replace)
                    changes.Add(e.NewItems.Cast<int>().Single());

                if (raiseCount <= expectedCanges.Count())
                    Assert.AreEqual(NotifyCollectionChangedAction.Replace, e.Action);
                else
                {
                    if (toAdd.Length == toRem.Length) Assert.Fail("Shouldn't be here.");
                    var isRemove = toRem.Length > toAdd.Length;
                    var expectedAction = toRem.Length > toAdd.Length
                ? NotifyCollectionChangedAction.Remove
                : NotifyCollectionChangedAction.Add;

                    Assert.AreEqual(expectedAction, e.Action);
                    if (isRemove)
                        Assert.AreEqual(e.OldItems.Count, toRem.Length - toAdd.Length);
                    else
                        Assert.AreEqual(e.NewItems.Count, toAdd.Length - toRem.Length);
                }
            };

            collection.ReplaceRange(toAdd.ToList());

            Assert.True(toAdd.SequenceEqual(collection), "Collections do not match 1");
            Assert.True(expectedCanges.SequenceEqual(changes), "Collections do not match 2");

            Assert.AreEqual(expectedRaiseCount, raiseCount);
            // TODO add another case replacing with a larger collection - the last event should be of adding
            // TODO add another case replacing with a collection of identical length, only replace events should be raised
        }


        [Test()]
        public void RemoveRangeRemoveTest()
        {
            int[] toAdd = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3 };
            int[] toRem = new[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 0, 0 };
            List<int> original = toAdd.ToList();
            ObservableRangeCollection<int> collection = new ObservableRangeCollection<int>(toAdd.ToList());

            int expectedRaiseCount = 4,
                raiseCount = 0;

            var actualRemovedItems = new List<int>();
            collection.CollectionChanged += (sender, e) =>
              {
                  raiseCount++;
                  actualRemovedItems.AddRange(e.OldItems.Cast<int>());
              };

            var expectedRemovedItems = new List<int>();
            foreach (var item in toRem)
                if (original.Remove(item))
                    expectedRemovedItems.Add(item);
            collection.RemoveRange(toRem);
            actualRemovedItems.Sort();

            Assert.AreEqual(original.Count, collection.Count, "Collection count doesn't match");
            Assert.IsTrue(original.SequenceEqual(collection), "Collections don't match");
            Assert.AreEqual(expectedRaiseCount, raiseCount);
            Assert.IsTrue(expectedRemovedItems.SequenceEqual(actualRemovedItems));
        }

    }
}

