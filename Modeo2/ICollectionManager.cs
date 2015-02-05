using System;
using System.Collections.Generic;

namespace RTH.Modeo2
{
    public interface ICollectionManager
    {
        void Add<T>(T item);
        void AddCollection<ICollection, T>(ICollection<T> collection);
        int Count<T>();
        IEnumerable<T> GetEnumerable<T>();
        IReadOnlyCollection<T> GetReadOnlyCollection<T>();
        bool Remove<T>(T item);
        int RemoveAll<T>(Predicate<T> predicate);
    }
}