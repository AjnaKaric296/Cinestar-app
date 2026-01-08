using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Cinestar_app;

public static class ObservableCollectionExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        if (items == null) return;

        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
