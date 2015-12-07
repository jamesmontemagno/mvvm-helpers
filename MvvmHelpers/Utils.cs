using System;
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
        /// <returns>The timeout.</returns>
        /// <param name="task">Task.</param>
        /// <param name="duration">Duration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async static Task<T> WithTimeout<T>(this Task<T> task, int duration)
        {
            var retTask = await Task.WhenAny(task, Task.Delay(duration))
                .ConfigureAwait(false);

            if (retTask is Task<T>) 
                return task.Result;
            
            return default(T);
        }
    }
}

