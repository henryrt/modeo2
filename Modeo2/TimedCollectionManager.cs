using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class TimedCollectionManager : ICollectionManager
    {
        private ICollectionManager DataStore;
        public StringBuilder Log = new StringBuilder();

        public TimedCollectionManager(ICollectionManager cm)
        {
            DataStore = cm;
        }

        public void Add<T>(T item)
        {
            var ts = Timer( () => DataStore.Add<T>(item));
            Log.AppendLine(FormattedLine("Add", ts));
        }

        public void AddCollection<T>(ICollection<T> collection)
        {
            var ts = Timer(() => DataStore.AddCollection<T>(collection));
            Log.AppendLine(FormattedLine("AddCollection", ts));
        }

        public int Count<T>()
        {
            int ret = 0;
            var ts = Timer(() => { ret = DataStore.Count<T>(); });
            Log.AppendLine(FormattedLine("Count", ts));
            return ret;
        }

        public IEnumerable<T> GetEnumerable<T>()
        {
            IEnumerable<T> ret = null;
            var ts = Timer(() => { ret = DataStore.GetEnumerable<T>(); });
            Log.AppendLine(FormattedLine("GetEnumerable", ts));
            return ret;
        }

        public IReadOnlyCollection<T> GetReadOnlyCollection<T>()
        {
            IReadOnlyCollection<T> ret = null;
            var ts = Timer(() => { ret = DataStore.GetReadOnlyCollection<T>(); });
            Log.AppendLine(FormattedLine("GetReadOnlyCollection", ts));
            return ret;
        }

        public bool Remove<T>(T item)
        {
            bool ret = false;
            var ts = Timer(() => { ret = DataStore.Remove<T>(item); });
            Log.AppendLine(FormattedLine("Remove", ts));
            return ret;
        }

        public int RemoveAll<T>(Predicate<T> predicate)
        {
            int ret = 0;
            var ts = Timer(() => { ret = DataStore.RemoveAll<T>(predicate); });
            Log.AppendLine(FormattedLine("RemoveAll", ts));
            return ret;
        }

        public static long Timer(Action a)
        {
            var startTime = DateTime.Now.Ticks;
            a();
            return DateTime.Now.Ticks - startTime;
        }

        public string FormattedLine(string method, long ticks)
        {
            return String.Format("{0,-20}: {1} ticks.", method, ticks);
        }

        public T GetRandom<T>()
        {
            T ret = default(T);
            var ts = Timer(() => { ret = DataStore.GetRandom<T>(); });
            Log.AppendLine(FormattedLine("GetRandom", ts));
            return ret;
        }

        public Guid AddKeyed<T>(T item)
        {
            Guid ret = default(Guid);
            var ts = Timer(() => { ret = DataStore.AddKeyed<T>(item); });
            Log.AppendLine(FormattedLine("AddKeyed<T>", ts));
            return ret;
        }

        public void AddKeyed<K, T>(K key, T item)
        {
            var ts = Timer( () => DataStore.AddKeyed<K, T>(key, item) );
            Log.AppendLine(FormattedLine("AddKeyed<K,T>", ts));
        }

        public bool Any<T>(Func<T, bool> condition)
        {
            var ret = false;
            var ts = Timer(() => ret = DataStore.Any<T>(condition));
            Log.AppendLine(FormattedLine("Any<T>", ts));
            return ret;
        }

        public bool All<T>(Func<T, bool> condition)
        {
            var ret = false;
            var ts = Timer(() => ret = DataStore.All<T>(condition));
            Log.AppendLine(FormattedLine("All<T>", ts));
            return ret;
        }

        public void RemoveAll<T>()
        {
            DataStore.RemoveAll<T>();
        }
    }
}
