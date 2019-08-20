using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MvvmHelpers.UnitTests
{
	[TestClass]
	public class WeakEventManagerTests
	{
		static int count;

		static void Handler(object sender, EventArgs eventArgs)
		{
			count++;
		}

		internal class TestSource
		{
			public int Count = 0;
			public TestEventSource EventSource { get; set; }
			public TestSource()
			{
				EventSource = new TestEventSource();
				EventSource.TestEvent += EventSource_TestEvent;
			}
			public void Clean() => EventSource.TestEvent -= EventSource_TestEvent;

			public void Fire() => EventSource.FireTestEvent();

			void EventSource_TestEvent(object sender, EventArgs e) => Count++;
		}

		internal class TestEventSource
		{
			readonly WeakEventManager weakEventManager;

			public TestEventSource() => weakEventManager = new WeakEventManager();

			public void FireTestEvent() => OnTestEvent();

			internal event EventHandler TestEvent
			{
				add { weakEventManager.AddEventHandler(value); }
				remove { weakEventManager.RemoveEventHandler(value); }
			}

			void OnTestEvent() => weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(TestEvent));
		}

		internal class TestSubscriber
		{
			public void Subscribe(TestEventSource source) => source.TestEvent += SourceOnTestEvent;

			void SourceOnTestEvent(object sender, EventArgs eventArgs) => Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddHandlerWithEmptyEventNameThrowsException()
		{
			var wem = new WeakEventManager();
			wem.AddEventHandler((sender, args) => { }, "");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddHandlerWithNullEventHandlerThrowsException()
		{
			var wem = new WeakEventManager();
			wem.AddEventHandler(null, "test");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddHandlerWithNullEventNameThrowsException()
		{
			var wem = new WeakEventManager();
			wem.AddEventHandler((sender, args) => { }, null);
		}

		[TestMethod]
		public void CanRemoveEventHandler()
		{
			var source = new TestSource();
			_ = source.Count;
			source.Fire();

			Assert.IsTrue(source.Count == 1);
			source.Clean();
			source.Fire();
			Assert.IsTrue(source.Count == 1);
		}

		[TestMethod]
		public void CanRemoveStaticEventHandler()
		{
			var beforeRun = count;

			var source = new TestEventSource();
			source.TestEvent += Handler;
			source.TestEvent -= Handler;

			source.FireTestEvent();

			Assert.IsTrue(count == beforeRun);
		}

		[TestMethod]
		public void EventHandlerCalled()
		{
			var called = false;

			var source = new TestEventSource();
			source.TestEvent += (sender, args) => { called = true; };

			source.FireTestEvent();

			Assert.IsTrue(called);
		}

		[TestMethod]
		public void FiringEventWithoutHandlerShouldNotThrow()
		{
			var source = new TestEventSource();
			source.FireTestEvent();
		}

		[TestMethod]
		public void MultipleHandlersCalled()
		{
			var called1 = false;
			var called2 = false;

			var source = new TestEventSource();
			source.TestEvent += (sender, args) => { called1 = true; };
			source.TestEvent += (sender, args) => { called2 = true; };
			source.FireTestEvent();

			Assert.IsTrue(called1 && called2);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveHandlerWithEmptyEventNameThrowsException()
		{
			var wem = new WeakEventManager();
			wem.RemoveEventHandler((sender, args) => { }, "");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveHandlerWithNullEventHandlerThrowsException()
		{
			var wem = new WeakEventManager();
			wem.RemoveEventHandler(null, "test");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveHandlerWithNullEventNameThrowsException()
		{
			var wem = new WeakEventManager();
			wem.RemoveEventHandler((sender, args) => { }, null);
		}

		[TestMethod]
		public void RemovingNonExistentHandlersShouldNotThrow()
		{
			var wem = new WeakEventManager();
			wem.RemoveEventHandler((sender, args) => { }, "fake");
			wem.RemoveEventHandler(Handler, "alsofake");
		}

		[TestMethod]
		public void RemoveHandlerWithMultipleSubscriptionsRemovesOne()
		{
			var beforeRun = count;

			var source = new TestEventSource();
			source.TestEvent += Handler;
			source.TestEvent += Handler;
			source.TestEvent -= Handler;

			source.FireTestEvent();

			Assert.AreEqual(beforeRun + 1, count);
		}

		[TestMethod]
		public void StaticHandlerShouldRun()
		{
			var beforeRun = count;

			var source = new TestEventSource();
			source.TestEvent += Handler;

			source.FireTestEvent();

			Assert.IsTrue(count > beforeRun);
		}

		[TestMethod]
		public void VerifySubscriberCanBeCollected()
		{
			WeakReference wr = null;
			var source = new TestEventSource();
			new Action(() =>
			{
				var ts = new TestSubscriber();
				wr = new WeakReference(ts);
				ts.Subscribe(source);
			})();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.IsNotNull(wr);
			Assert.IsFalse(wr.IsAlive);

			// The handler for this calls Assert.Fail, so if the subscriber has not been collected
			// the handler will be called and the test will fail
			source.FireTestEvent();
		}
	}
}
