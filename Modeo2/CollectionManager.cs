using System.Collections;
using System.Collections.Generic;

namespace RTH.Modeo2
{
    public class CollectionManager
    {
        // Manages collections in a Hashtable. One List per Type.
        Hashtable ht = new Hashtable();

        public void Add<T>(T item)
        {
            if (ht[typeof(T)] == null)
            {
                ht.Add(typeof(T), new List<T>());
            }
            (ht[typeof(T)] as List<T>).Add(item);
        }

        public void AddCollection<ICollection, T>(ICollection<T> collection)
        {
            foreach (var item in collection) Add<T>(item);
        }

        public IReadOnlyCollection<T> GetReadOnlyCollection<T>()
        {
            return (IReadOnlyCollection<T>)ht[typeof(T)]; // use a cast so type-mismatch will throw exception
        }

        public IEnumerable<T> GetEnumerable<T>()
        {
            return (IEnumerable<T>)ht[typeof(T)];
        }
    }
}
