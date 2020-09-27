using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Heap
{
    public class PQMax<V> : MaxHeap<V>
    {
        public PQMax(V[] a, Comparison<V> comparer = null)
            : base()
        {
            this.comparer = FetchComparer(comparer);
            a.CopyTo(values, 0);
            heapSize = a.Length;
            BuildMaxHeap();
        }

        public PQMax(ref V[] a, Comparison<V> comparer = null)
            : base()
        {
            this.comparer = FetchComparer(comparer);
            values = a;
            heapSize = a.Length;
            BuildMaxHeap();
        }

        public PQMax(Comparison<V> comparer = null)
            : base()
        {
            this.comparer = FetchComparer(comparer);
            BuildMaxHeap();
        }

        public PQMax(int size, Comparison<V> comparer = null)
            : base(size)
        {
            this.comparer = FetchComparer(comparer);
            BuildMaxHeap();
        }

        static Comparison<V> FetchComparer(Comparison<V> comparer)
        {
            if (comparer == null)
                comparer = Comparer<V>.Default.Compare;

            return comparer;
        }
    }
}
