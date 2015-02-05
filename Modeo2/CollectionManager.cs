using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RTH.Modeo2
{
    public class CollectionManager
    {
        // Manages collections in a Hashtable. 
        private Hashtable ht = new Hashtable();

        public void Add<T>(T item)
        {
            List<T> collection;
            lock (ht)
            {
                collection = ht[typeof(T)] as List<T> ?? new List<T>();
                if (collection.Count == 0) ht[typeof(T)] = collection;
            }
            lock(collection) collection.Add(item);
            
        }

        public void AddCollection<ICollection, T>(ICollection<T> collection)
        {
            foreach (var item in collection) Add<T>(item);
        }

        public IReadOnlyCollection<T> GetReadOnlyCollection<T>()
        {
            var collection = (List<T>)ht[typeof(T)];
            lock(collection) return new List<T>(collection);
        }

        public IEnumerable<T> GetEnumerable<T>()
        {
            var collection = (List<T>)ht[typeof(T)];
            lock(collection) return new List<T>(collection);
            
        }

        public int Count<T>()
        {
            return (ht[typeof(T)] as ICollection<T>).Count;
        }

        public bool Remove<T>(T item)
        {
            var collection = (List<T>)ht[typeof(T)];
            lock (collection) return collection.Remove(item);
        }
        public int RemoveAll<T>(Predicate<T> predicate)
        {
            var collection = (List<T>)ht[typeof(T)];
            lock (collection) return collection.RemoveAll(predicate);
        }

    }
}
