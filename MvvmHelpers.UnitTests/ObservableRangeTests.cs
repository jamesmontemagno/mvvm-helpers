using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace MvvmHelpers.UnitTests
{
	[TestClass]
	public class ObservableRangeTests
	{		
		[TestMethod]
		public void AddRange()
		{
			var collection = new ObservableRangeCollection<int>();
			var toAdd = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3 };

			collection.CollectionChanged += (s, e) =>
			{
				Assert.AreEqual(e.Action,
							   NotifyCollectionChangedAction.Add,
							   "AddRange didn't use Add like requested.");

				Assert.IsNull(e.OldItems, "OldItems should be null.");

				Assert.AreEqual(toAdd.Length,
								e.NewItems.Count,
								"Expected and actual OldItems don't match.");

				for (var i = 0; i < toAdd.Length; i++)
				{
					Assert.AreEqual(toAdd[i], (int)e.NewItems[i],
						"Expected and actual NewItems don't match.");
				}
			};
			collection.AddRange(toAdd);
		}

		[TestMethod]
		public void AddRangeEmpty()
		{
			var collection = new ObservableRangeCollection<int>();
			var toAdd = new int[0];

			collection.CollectionChanged += (s, e) =>
			{
				Assert.Fail("The event is raised.");
			};
			collection.AddRange(toAdd);
		}

		[TestMethod]
		public void ReplaceRange()
		{
			var collection = new ObservableRangeCollection<int>();
			var toAdd = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3 };
			var toRemove = new[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 0, 0 };
			collection.AddRange(toRemove);
			collection.CollectionChanged += (s, e) =>
			{
				Assert.AreEqual(e.Action,
								NotifyCollectionChangedAction.Reset,
								"ReplaceRange didn't use Remove like requested.");

				Assert.IsNull(e.OldItems, "OldItems should be null.");
				Assert.IsNull(e.NewItems, "NewItems should be null.");

				Assert.AreEqual(collection.Count, toAdd.Length, "Lengths are not the same");

				for (var i = 0; i < toAdd.Length; i++)
				{
					if (collection[i] != (int)toAdd[i])
						Assert.Fail("Expected and actual items don't match.");
				}
			};
			collection.ReplaceRange(toAdd);
		}
		
		[TestMethod]
		public void ReplaceRange_on_non_empty_collection_should_always_raise_collection_changes()
		{
			var collection = new ObservableRangeCollection<int>(new [] { 1 });
			var toAdd = new int[0];
			var eventRaised = false;
						
			collection.CollectionChanged += (s, e) =>
			{
				eventRaised = true;
			};
			
			collection.ReplaceRange(toAdd);
			Assert.IsTrue(eventRaised, "Collection Reset should be raised.");
		}
		
		[TestMethod]
		public void ReplaceRange_on_empty_collection_should_NOT_raise_collection_changes_when_empty()
		{
			var collection = new ObservableRangeCollection<int>();
			var toAdd = new int[0];
						
			collection.CollectionChanged += (s, e) =>
			{
				Assert.Fail("Collection changes should NOT be raised.");
			};
			
			collection.ReplaceRange(toAdd);
		}


		[TestMethod]
		public void RemoveRangeRemoveTestMethod()
		{
			var collection = new ObservableRangeCollection<int>();
			var toAdd = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3 };
			var toRemove = new[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 0, 0 };
			collection.AddRange(toAdd);
			collection.CollectionChanged += (s, e) =>
			{
				if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
					Assert.Fail("RemoveRange didn't use Remove like requested.");
				if (e.OldItems == null)
					Assert.Fail("OldItems should not be null.");
				var expected = new int[] { 1, 1, 2, 2, 3, 3, 4, 5, 5, 6, 7, 8, 9, 9 };
				if (expected.Length != e.OldItems.Count)
					Assert.Fail("Expected and actual OldItems don't match.");
				for (var i = 0; i < expected.Length; i++)
				{
					if (expected[i] != (int)e.OldItems[i])
						Assert.Fail("Expected and actual OldItems don't match.");
				}
			};
			collection.RemoveRange(toRemove, NotifyCollectionChangedAction.Remove);

		}

		[TestMethod]
		public void RemoveRangeEmpty()
		{
			var collection = new ObservableRangeCollection<int>();
			var toAdd = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3 };
			var toRemove = new int[0];
			collection.AddRange(toAdd);
			collection.CollectionChanged += (s, e) =>
			{
				Assert.Fail("The event is raised.");
			};
			collection.RemoveRange(toRemove, NotifyCollectionChangedAction.Remove);
		}
	}
}

