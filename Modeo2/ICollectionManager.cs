using System;
using System.Collections.Generic;

namespace RTH.Modeo2
{
    public interface ICollectionManager
    {
        void Add<T>(T item);
        void AddCollection<T>(ICollection<T> collection);

        int Count<T>();
        IEnumerable<T> GetEnumerable<T>();
        IReadOnlyCollection<T> GetReadOnlyCollection<T>();

        // returns true if any items in the collection return true for the condition
        // returns false if no items exist
        bool Any<T>(Func<T, bool> condition);
        // returns true if all items in the collection return true for the condition
        // returns true if no items exist
        bool All<T>(Func<T, bool> condition);

        bool Remove<T>(T item);
        void RemoveAll<T>();
        int RemoveAll<T>(Predicate<T> predicate);

        T GetRandom<T>();
        Guid AddKeyed<T>(T item);
        void AddKeyed<K,T>(K key, T item);
    }
}