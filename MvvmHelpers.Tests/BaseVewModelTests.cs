using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MvvmHelpers.Tests
{
    [TestFixture()]
    public class BaseViewModelTests
    {

        [Test()]
        public void TitleTest()
        {
            PropertyChangedEventArgs updated = null;
            var vm = new PersonViewModel();

            vm.PropertyChanged += (sender, args) =>
            {
                updated = args;
            };

            vm.Title = "Hello";
            Task.Delay(100);
            Assert.IsNotNull(updated, "Property changed didn't raise");
            Assert.AreEqual(updated.PropertyName, nameof(vm.Title), "Correct Property name didn't get raised");
        }

        [Test()]
        public void SubTitle()
        {
            PropertyChangedEventArgs updated = null;
            var vm = new PersonViewModel();

            vm.PropertyChanged += (sender, args) =>
            {
                updated = args;
            };

            vm.Subtitle = "Hello";
            Task.Delay(100);
            Assert.IsNotNull(updated, "Property changed didn't raise");
            Assert.AreEqual(updated.PropertyName, nameof(vm.Subtitle), "Correct Property name didn't get raised");
        }
        [Test()]
        public void CanLoadMore()
        {
            PropertyChangedEventArgs updated = null;
            var vm = new PersonViewModel();

            vm.PropertyChanged += (sender, args) =>
            {
                updated = args;
            };

            vm.CanLoadMore = false;
            Task.Delay(100);
            Assert.IsNotNull(updated, "Property changed didn't raise");
            Assert.AreEqual(updated.PropertyName, nameof(vm.CanLoadMore), "Correct Property name didn't get raised");
        }

        [Test()]
        public void Icon()
        {
            PropertyChangedEventArgs updated = null;
            var vm = new PersonViewModel();

            vm.PropertyChanged += (sender, args) =>
            {
                updated = args;
            };

            vm.Icon = "Hello";
            Task.Delay(100);
            Assert.IsNotNull(updated, "Property changed didn't raise");
            Assert.AreEqual(updated.PropertyName, nameof(vm.Icon), "Correct Property name didn't get raised");
        }

        [Test()]
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
            Task.Delay(100);
            Assert.IsNotNull(updated, "Property changed didn't raise");
            Assert.AreEqual(updated.PropertyName, nameof(vm.IsBusy), "Correct Property name didn't get raised");

            Assert.IsFalse(vm.IsNotBusy, "Is Not Busy didn't change.");
        }

        [Test()]
        public void IsNotBusy()
        {
            PropertyChangedEventArgs updated = null;
            var vm = new PersonViewModel();

            vm.PropertyChanged += (sender, args) =>
            {
                if(args.PropertyName == "IsNotBusy")
                    updated = args;
            };

            vm.IsNotBusy = false;
            Task.Delay(100);
            Assert.IsNotNull(updated, "Property changed didn't raise");
            Assert.AreEqual(updated.PropertyName, nameof(vm.IsNotBusy), "Correct Property name didn't get raised");

            Assert.IsTrue(vm.IsBusy, "Is Busy didn't change.");
        }
    }
}

