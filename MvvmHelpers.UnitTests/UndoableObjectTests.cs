using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MvvmHelpers.Tests
{
    [TestClass]
    public class UndoableObjectTests
    {
        UndoablePerson person;

        [TestInitialize]
        public void Setup()
        {
            person = new UndoablePerson();
            person.FirstName = "James";
            person.LastName = "Montemagno";
        }

        [TestMethod]
        public void OnPropertyChanged()
        {
            PropertyChangedEventArgs updated = null;
            person.PropertyChanged += (sender, args) =>
            {
                updated = args;
            };

            person.FirstName = "Motz";


            Assert.IsNotNull(updated, "Property changed didn't raise");
            Assert.AreEqual(updated.PropertyName, nameof(person.FirstName), "Correct Property name didn't get raised");
        }

        [TestMethod]
        public void OnDidntChange()
        {
            PropertyChangedEventArgs updated = null;
            person.PropertyChanged += (sender, args) =>
            {
                updated = args;
            };

            person.FirstName = "James";


            Assert.IsNull(updated, "Property changed was raised, but shouldn't have been");
        }

        [TestMethod]
        public void OnChangedEvent()
        {

            var triggered = false;
            person.Changed = () =>
            {
                triggered = true;
            };

            person.FirstName = "Motz";

            Assert.IsTrue(triggered, "OnChanged didn't raise");
        }

        [TestMethod]
        public void ValidateEvent()
        {
            var contol = "Motz";
            var triggered = false;
            person.Validate = (oldValue, newValue) =>
            {
                triggered = true;
                return oldValue != newValue;
            };

            person.FirstName = contol;

            Assert.IsTrue(triggered, "ValidateValue didn't raise");
            Assert.AreEqual(person.FirstName, contol, "Value was not set correctly.");

        }

        [TestMethod]
        public void NotValidateEvent()
        {
            var contol = person.FirstName;
            var triggered = false;
            person.Validate = (oldValue, newValue) =>
            {
                triggered = true;
                return false;
            };

            person.FirstName = "Motz";

            Assert.IsTrue(triggered, "ValidateValue didn't raise");
            Assert.AreEqual(person.FirstName, contol, "Value should not have been set.");

        }

        [TestMethod]
        public void ValidateEventException()
        {
            person.Validate = (oldValue, newValue) =>
            {
                throw new ArgumentOutOfRangeException();
                return false;
            };

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => person.FirstName = "Motz", "Should throw ArgumentOutOfRangeException");

        }

        [TestMethod]
        public void Undo()
        {
            person.FirstName = "Frank";
            person.Undo();
            Assert.AreEqual("James", person.FirstName, "Failed to undo property");
        }

        [TestMethod]
        public void UndoRedo()
        {
            person.FirstName = "Frank";
            person.Undo();
            Assert.AreEqual("James", person.FirstName, "Failed to undo property");
            person.Redo();
            Assert.AreEqual("Frank", person.FirstName, "Failed to redo property");
        }

        [TestMethod]
        public void UndoUndoRedoRedo()
        {
            person.FirstName = "Frank";
            person.FirstName = "Charles";
            person.Undo();
            Assert.AreEqual("Frank", person.FirstName, "Failed to undo property");
            person.Undo();
            Assert.AreEqual("James", person.FirstName, "Failed to undo property");
            person.Redo();
            Assert.AreEqual("Frank", person.FirstName, "Failed to redo property");
            person.Redo();
            Assert.AreEqual("Charles", person.FirstName, "Failed to redo property");
        }


        [TestMethod]
        public void UndoActNoRedo()
        {
            person.FirstName = "Frank";
            person.FirstName = "Charles";
            person.Undo();
            Assert.AreEqual("Frank", person.FirstName, "Failed to undo property");
            person.FirstName = "Lizzy";
            person.Redo();
            Assert.AreEqual("Lizzy", person.FirstName, "Failed to redo property");
        }

    }
}
