using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MvvmHelpers
{
    public class Grouping<T, TV> : ObservableCollection<TV>
    {
        public T Key { get; private set; }

        public Grouping(T key, IList<TV> items)
        {
            Key = key;
            foreach (var item in items)
                Items.Add(item);
        }
    }
}

