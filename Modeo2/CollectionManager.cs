using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RTH.Modeo2
{
    public class CollectionManager : ICollectionManager
    {
        // Manages collections in a Hashtable. 
        // A given type T can only be a list or a dictionary, not both.
        private Hashtable ht = new Hashtable();

        public void Add<T>(T item)
        {
            List<T> collection = GetCollection<List<T>, T>();
            lock(collection) collection.Add(item);
            
        }

        // adds an item to a dictionary and returns a generated guid as a key
        public Guid AddKeyed<T>(T item)
        {
            var key = Guid.NewGuid();
            AddKeyed<Guid, T>(key, item);
            return key;
        }

        // adds a keyed item to a dictionary
        public void AddKeyed<K, T>(K key, T item)
        {
            Dictionary<K, T> collection = GetCollection<Dictionary<K, T>, T>();
            lock(collection) collection.Add(key, item);
        }

        private Z GetCollection<Z, T>() where Z : ICollection, new() 
        {
            lock (ht)
            {
                var collection = (Z) (ht[typeof(T)] ?? new Z());
                if (collection.Count == 0) ht[typeof(T)] = collection;
                return collection;
            }
        }
        public void AddCollection<T>(ICollection<T> collection)
        {
            foreach (var item in collection) Add<T>(item);
        }

        public IReadOnlyCollection<T> GetReadOnlyCollection<T>()
        {
            var collection = (List<T>)ht[typeof(T)];
            if (collection == null) return new List<T>();
            lock (collection) return new List<T>(collection);
        }

        public IEnumerable<T> GetEnumerable<T>()
        {
            var collection = (List<T>)ht[typeof(T)];
            if (collection == null) return new List<T>();            
            lock(collection) return new List<T>(collection);
            
        }

        public int Count<T>()
        {
            return (ht[typeof(T)] as ICollection)?.Count ?? 0;
        }

        public bool Any<T>(Func<T,bool> f)
        {
            return (ht[typeof(T)] as ICollection<T>)?.Any<T>(f) ?? false;
        }

        public bool Remove<T>(T item)
        {
            var collection = (List<T>)ht[typeof(T)];
            lock (collection) return collection.Remove(item);
        }
        public int RemoveAll<T>(Predicate<T> predicate)
        {
            var collection = (List<T>)ht[typeof(T)];
            if (collection == null) return 0;
            lock (collection) return collection.RemoveAll(predicate);
        }

        private static Random rand = new Random();
        public T GetRandom<T>()
        {
            var list = ht[typeof(T)] as List<T>;
            if (list == null) return default(T);
            var ix = rand.Next(list.Count);
            return list[ix];
        }

        public T GetRandom<T>(IEnumerable<T> coll)
        {
            return coll.ElementAt(rand.Next(coll.Count()));
        }

        public bool All<T>(Func<T, bool> condition)
        {
            return (ht[typeof(T)] as ICollection<T>)?.All<T>(condition) ?? true;
        }

        public void RemoveAll<T>()
        {
            lock(ht)
            {
                var list = ht[typeof(T)] as List<T>;
                lock(list)
                {
                    if (list != null)
                    {
                        list.Clear();
                    }
                }
            }
        }
    }
}
