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

        public void AddCollection<ICollection, T>(ICollection<T> collection)
        {
            var ts = Timer(() => DataStore.AddCollection<ICollection, T>(collection));
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
            return DataStore.GetReadOnlyCollection<T>();
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
    }
}
