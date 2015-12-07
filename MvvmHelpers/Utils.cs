using System;
using System.Threading.Tasks;

namespace MvvmHelpers
{
    public static class Utils
    {
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

