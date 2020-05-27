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

#pragma warning disable CS8603 // Possible null reference return.
			return retTask is Task<T> ? task.Result : default;
#pragma warning restore CS8603 // Possible null reference return.
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
		/// <summary>
		/// Attempts to await on the task and catches exception
		/// </summary>
		/// <param name="task">Task to execute</param>
		/// <param name="onException">What to do when method has an exception</param>
		/// <param name="continueOnCapturedContext">If the context should be captured.</param>
		public static async void SafeFireAndForget(this Task task, Action<Exception>? onException = null, bool continueOnCapturedContext = false)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
		{
			try
			{
				await task.ConfigureAwait(continueOnCapturedContext);
			}
			catch (Exception ex) when (onException != null)
			{
				onException.Invoke(ex);
			}
		}	

	}
}

