using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmHelpers.Commands;
using MvvmHelpers.Exceptions;
using System;

namespace MvvmHelpers.UnitTests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void Constructor()
        {
            var cmd = new Command(() => { });
            Assert.IsTrue(cmd.CanExecute(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsWithNullConstructor()
        {
            new Command((Action)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsWithNullParameterizedConstructor()
        {
            new Command((Action<object>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsWithNullCanExecute()
        {
            new Command(() => { }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsWithNullParameterizedCanExecute()
        {
             new Command(o => { }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsWithNullExecuteValidCanExecute()
        {
            new Command(null, () => true);
        }

        [TestMethod]
        public void Execute()
        {
            bool executed = false;
            var cmd = new Command(() => executed = true);

            cmd.Execute(null);
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void ExecuteParameterized()
        {
            object executed = null;
            var cmd = new Command(o => executed = o);

            var expected = new object();
            cmd.Execute(expected);

            Assert.AreEqual(expected, executed);
        }

        [TestMethod]
        public void ExecuteWithCanExecute()
        {
            bool executed = false;
            var cmd = new Command(() => executed = true, () => true);

            cmd.Execute(null);
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void CanExecute()
        {
            bool canExecuteRan = false;
            var cmd = new Command(() => { }, () => {
                canExecuteRan = true;
                return true;
            });

            Assert.AreEqual(true, cmd.CanExecute(null));
            Assert.IsTrue(canExecuteRan);
        }

        [TestMethod]
        public void ChangeCanExecute()
        {
            bool signaled = false;
            var cmd = new Command(() => { });

            cmd.CanExecuteChanged += (sender, args) => signaled = true;

            cmd.RaiseCanExecuteChanged();
            Assert.IsTrue(signaled);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenericThrowsWithNullExecute()
        {
             new Command<string>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenericThrowsWithNullExecuteAndCanExecuteValid()
        {
            new Command<string>(null, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenericThrowsWithValidExecuteAndCanExecuteNull()
        {
            new Command<string>(s => { }, null);
        }

        [TestMethod]
        public void GenericExecute()
        {
            string result = null;
            var cmd = new Command<string>(s => result = s);

            cmd.Execute("Foo");
            Assert.AreEqual("Foo", result);
        }

        [TestMethod]
        public void GenericExecuteWithCanExecute()
        {
            string result = null;
            var cmd = new Command<string>(s => result = s, s => true);

            cmd.Execute("Foo");
            Assert.AreEqual("Foo", result);
        }

        [TestMethod]
        public void GenericCanExecute()
        {
            string result = null;
            var cmd = new Command<string>(s => { }, s => {
                result = s;
                return true;
            });

            Assert.AreEqual(true, cmd.CanExecute("Foo"));
            Assert.AreEqual("Foo", result);
        }

        class FakeParentContext
        {
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class FakeChildContext
        {
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCommandParameterException))]
        public void CanExecuteReturnsFalseIfParameterIsWrongReferenceType()
        {
            var command = new Command<FakeChildContext>(context => { }, context => true);

            command.CanExecute(new FakeParentContext());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCommandParameterException))]
        public void CanExecuteReturnsFalseIfParameterIsWrongValueType()
        {
            var command = new Command<int>(context => { }, context => true);

            command.CanExecute(10.5);
        }

        [TestMethod]
        public void CanExecuteUsesParameterIfReferenceTypeAndSetToNull()
        {
            var command = new Command<FakeChildContext>(context => { }, context => true);

            Assert.IsTrue(command.CanExecute(null), "null is a valid value for a reference type");
        }

        [TestMethod]
        public void CanExecuteUsesParameterIfNullableAndSetToNull()
        {
            var command = new Command<int?>(context => { }, context => true);

            Assert.IsTrue(command.CanExecute(null), "null is a valid value for a Nullable<int> type");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCommandParameterException))]
        public void CanExecuteIgnoresParameterIfValueTypeAndSetToNull()
        {
            var command = new Command<int>(context => { }, context => true);

            command.CanExecute(null);
        }

        [TestMethod]
        public void ExecuteDoesNotRunIfParameterIsWrongReferenceType()
        {
            int executions = 0;
            var command = new Command<FakeChildContext>(context => executions += 1);

            Assert.IsTrue(executions == 0, "the command should not have executed");
        }

        [TestMethod]
        public void ExecuteDoesNotRunIfParameterIsWrongValueType()
        {
            int executions = 0;
            var command = new Command<int>(context => executions += 1);

            Assert.IsTrue(executions == 0, "the command should not have executed");
        }

        [TestMethod]
        public void ExecuteRunsIfReferenceTypeAndSetToNull()
        {
            int executions = 0;
            var command = new Command<FakeChildContext>(context => executions += 1);
            command.Execute(null);
            Assert.IsTrue(executions == 1, "the command should have executed");
        }

        [TestMethod]
        public void ExecuteRunsIfNullableAndSetToNull()
        {
            int executions = 0;
            var command = new Command<int?>(context => executions += 1);
            command.Execute(null);
            Assert.IsTrue(executions == 1, "the command should have executed");
        }

        [TestMethod]
        public void ExecuteDoesNotRunIfValueTypeAndSetToNull()
        {
            int executions = 0;
            var command = new Command<int>(context => executions += 1);

            Assert.IsTrue(executions == 0, "the command should not have executed");
        }
    }
}
