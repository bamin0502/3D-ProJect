using System;
using System.Collections.Generic;
using System.Text;

namespace MNF
{
    public class TObjectPool<TObject> where TObject : class, new()
    {
        private object poolLock = null;
        private LinkedList<TObject> pool = null;

        public int Count
        {
            get { return pool.Count; }
        }

        public TObjectPool()
        {
            poolLock = new object();
            pool = new LinkedList<TObject>();
        }

        public TObject alloc()
        {
            TObject tObject = default(TObject);
            lock (poolLock)
            {
                if (pool.Count > 0)
                {
                    tObject = pool.First.Value;
                    pool.RemoveFirst();
                }
                else
                {
                    tObject = new TObject();
                }
            }
            return tObject;
        }

        public void free(TObject tObject)
        {
            lock (poolLock)
            {
                pool.AddLast(tObject);
            }
        }
    }
}
