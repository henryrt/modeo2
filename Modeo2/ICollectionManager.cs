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

        // returns true if any items in the collection return true for the condition
        bool Any<T>(Func<T, bool> condition);
        bool All<T>(Func<T, bool> condition);
        bool Remove<T>(T item);
        int RemoveAll<T>(Predicate<T> predicate);
        T GetRandom<T>();
        Guid AddKeyed<T>(T item);
        void AddKeyed<K,T>(K key, T item);
    }
}