using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MvvmHelpers.Tests
{
    [TestFixture()]
    public class ObservableObjectTests
    {
        Person person;
        [SetUp]
        public void Setup()
        {
            person = new Person();
            person.FirstName = "James";
            person.LastName = "Montemagno";
        }

        [Test()]
        public void OnPropertyChanged()
        {
            PropertyChangedEventArgs updated = null;
            person.PropertyChanged += (sender, args) =>
            {
                updated = args;
            };

            person.FirstName = "Motz";

            Task.Delay(100);

            Assert.IsNotNull(updated, "Property changed didn't raise");
            Assert.AreEqual(updated.PropertyName, nameof(person.FirstName), "Correct Property name didn't get raised");
        }

        [Test()]
        public void OnDidntChange()
        {
            PropertyChangedEventArgs updated = null;
            person.PropertyChanged += (sender, args) =>
            {
                updated = args;
            };

            person.FirstName = "James";

            Task.Delay(100);

            Assert.IsNull(updated, "Property changed was raised, but shouldn't have been");
        }

        [Test()]
        public void OnChangedEvent()
        {

            var triggered = false;
            person.Changed = () =>
            {
                triggered = true;
            };

            person.FirstName = "Motz";

            Task.Delay(100);

            Assert.IsTrue(triggered, "OnChanged didn't raise");
        }
    }
}

