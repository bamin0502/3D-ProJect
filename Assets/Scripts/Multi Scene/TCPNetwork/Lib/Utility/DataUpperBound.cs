using System;

namespace MNF
{
#if !NETFX_CORE
    public class DataUpperBound<T>
    {
        private int[] ranges = null;
        private T[] datas = null;
        private int insertCount = 0;

        public DataUpperBound(int count)
        {
            this.ranges = new int[count];
            this.datas = new T[count];
        }

        public bool insertData(int range, T data)
        {
            for (int i = 0; i < ranges.Length; ++i)
            {
                if (ranges[i].CompareTo(data) == 0)
                    return false;
            }

            ranges[insertCount] = range;
            datas[insertCount] = data;
            ++insertCount;

            return true;
        }

        public T Lookup(int range)
        {
            int position = Array.BinarySearch(ranges, range);
            
            if (position < 0)
                position = ~position;

            if (position >= datas.Length)
                position = datas.Length - 1;

            return datas[position];
        }
    }
#endif
}
