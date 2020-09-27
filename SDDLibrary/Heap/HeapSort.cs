using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Heap
{
    public class HeapSort<K>
    {
        protected Comparison<K> comparer;

        public HeapSort(Comparison<K> comparer = null)
        {
            this.comparer = comparer;
        }

        public void Sort(K[] values)
        {
            PQMax<K> pq = new PQMax<K>(ref values, comparer);
            
            while(pq.Count > 0)
            {
                K val = pq.Remove();
                values[pq.Count] = val;
            }                        
        }
    }
}
