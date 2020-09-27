using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Heap
{
    public class PQMin<V> : MaxHeap<V>
    {
        public PQMin(V[] a, Comparison<V> comparer = null)
            : base()
        {
            this.comparer = FetchComparer(comparer);
            a.CopyTo(values, 0);
            heapSize = a.Length;
            BuildMaxHeap();
        }

        public PQMin(ref V[] a, Comparison<V> comparer = null)
            : base()
        {
            this.comparer = FetchComparer(comparer);
            values = a;
            heapSize = a.Length;
            BuildMaxHeap();
        }

        public PQMin(Comparison<V> comparer = null)
            : base()
        {
            this.comparer = FetchComparer(comparer);
            BuildMaxHeap();
        }

        public PQMin(int size, Comparison<V> comparer = null)
            : base(size)
        {
            this.comparer = FetchComparer(comparer);
            BuildMaxHeap();
        }

        static Comparison<V> FetchComparer(Comparison<V> comparer)
        {
            Comparison<V> temp;
            if (comparer == null)
                temp = Comparer<V>.Default.Compare;
            else
                temp = comparer;

            return (x, y) => -temp(x, y);
        }
    }
}
