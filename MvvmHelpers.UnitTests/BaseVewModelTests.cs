using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace MvvmHelpers.UnitTests
{
	[TestClass]
	public class BaseViewModelTests
	{

		[TestMethod]
		public void TitleTestMethod()
		{
			PropertyChangedEventArgs updated = null;
			var vm = new PersonViewModel();

			vm.PropertyChanged += (sender, args) =>
			{
				updated = args;
			};

			vm.Title = "Hello";
			Assert.IsNotNull(updated, "Property changed didn't raise");
			Assert.AreEqual(updated.PropertyName, nameof(vm.Title), "Correct Property name didn't get raised");
		}

		[TestMethod]
		public void SubTitle()
		{
			PropertyChangedEventArgs updated = null;
			var vm = new PersonViewModel();

			vm.PropertyChanged += (sender, args) =>
			{
				updated = args;
			};

			vm.Subtitle = "Hello";
			Assert.IsNotNull(updated, "Property changed didn't raise");
			Assert.AreEqual(updated.PropertyName, nameof(vm.Subtitle), "Correct Property name didn't get raised");
		}
		[TestMethod]
		public void CanLoadMore()
		{
			PropertyChangedEventArgs updated = null;
			var vm = new PersonViewModel();

			vm.PropertyChanged += (sender, args) =>
			{
				updated = args;
			};

			vm.CanLoadMore = false;
			Assert.IsNotNull(updated, "Property changed didn't raise");
			Assert.AreEqual(updated.PropertyName, nameof(vm.CanLoadMore), "Correct Property name didn't get raised");
		}

		[TestMethod]
		public void Icon()
		{
			PropertyChangedEventArgs updated = null;
			var vm = new PersonViewModel();

			vm.PropertyChanged += (sender, args) =>
			{
				updated = args;
			};

			vm.Icon = "Hello";
			Assert.IsNotNull(updated, "Property changed didn't raise");
			Assert.AreEqual(updated.PropertyName, nameof(vm.Icon), "Correct Property name didn't get raised");
		}

		[TestMethod]
		public void IsBusy()
		{
			PropertyChangedEventArgs updated = null;
			var vm = new PersonViewModel();

			vm.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == "IsBusy")
					updated = args;
			};

			vm.IsBusy = true;
			Assert.IsNotNull(updated, "Property changed didn't raise");
			Assert.AreEqual(updated.PropertyName, nameof(vm.IsBusy), "Correct Property name didn't get raised");

			Assert.IsFalse(vm.IsNotBusy, "Is Not Busy didn't change.");
		}

		[TestMethod]
		public void IsNotBusy()
		{
			PropertyChangedEventArgs updated = null;
			var vm = new PersonViewModel();

			vm.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == "IsNotBusy")
					updated = args;
			};

			vm.IsNotBusy = false;
			Assert.IsNotNull(updated, "Property changed didn't raise");
			Assert.AreEqual(updated.PropertyName, nameof(vm.IsNotBusy), "Correct Property name didn't get raised");

			Assert.IsTrue(vm.IsBusy, "Is Busy didn't change.");
		}
	}
}

