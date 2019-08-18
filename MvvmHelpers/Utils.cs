using MvvmHelpers.Exceptions;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MvvmHelpers
{
	/// <summary>
	/// Extension Utils
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Task extension to add a timeout.
		/// </summary>
		/// <returns>The task with timeout.</returns>
		/// <param name="task">Task.</param>
		/// <param name="timeoutInMilliseconds">Timeout duration in Milliseconds.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public async static Task<T> WithTimeout<T>(this Task<T> task, int timeoutInMilliseconds)
		{
			var retTask = await Task.WhenAny(task, Task.Delay(timeoutInMilliseconds))
				.ConfigureAwait(false);

			return retTask is Task<T> ? task.Result : default;
		}

		/// <summary>
		/// Task extension to add a timeout.
		/// </summary>
		/// <returns>The task with timeout.</returns>
		/// <param name="task">Task.</param>
		/// <param name="timeout">Timeout Duration.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout) =>
			WithTimeout(task, (int)timeout.TotalMilliseconds);

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
		public static async void SafeFireAndForgetAsync(this Task task, Action<Exception> onException = null, bool continueOnCapturedContext = false)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
		{
			try
			{
				await task.ConfigureAwait(continueOnCapturedContext);
			}
			catch (Exception ex)
			{
				onException?.Invoke(ex);
			}
		}

		internal static bool IsValidParameter<T>(object o)
		{
			bool valid;
			if (o != null)
			{
				// The parameter isn't null, so we don't have to worry whether null is a valid option
				valid = o is T;

				if (!valid)
					throw new InvalidCommandParameterException(typeof(T), null);

				return valid;
			}

			var t = typeof(T);

			// The parameter is null. Is T Nullable?
			if (Nullable.GetUnderlyingType(t) != null)
			{
				return true;
			}

			// Not a Nullable, if it's a value type then null is not valid
			valid = !t.GetTypeInfo().IsValueType;

			if (!valid)
				throw new InvalidCommandParameterException(typeof(T), null);

			return valid;
		}

	}
}

