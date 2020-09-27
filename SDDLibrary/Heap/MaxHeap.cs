using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Heap
{
    public abstract class MaxHeap<V>
    {
        protected Comparison<V> comparer;
        protected V[] values;
        protected int heapSize;

        public int Count
        {
            get { return heapSize; }
        }

        public MaxHeap()
        {
            values = new V[100];
        }

        public MaxHeap(int size = 100)
        {
            values = new V[size];
        }

        public MaxHeap(V[] values)
        {
            this.values = values;
        }        

        public void BuildMaxHeap()
        {
            for (int i = (heapSize - 1) / 2; i >= 0; i--)
                MaxHeapify(heapSize, i);
        }

        public void MaxHeapify(int heapSize, int i)
        {
            int l = i * 2 + 1;
            int r = i * 2 + 2;
            int largest = i;

            if (l < heapSize && comparer(values[l], values[i]) > 0)
                largest = l;

            if (r < heapSize && comparer(values[r], values[largest]) > 0)
                largest = r;

            if (largest != i)
            {
                Swap(ref values[largest], ref values[i]);
                MaxHeapify(heapSize, largest);
            }
        }

        public void Insert(V val)
        {
            if (heapSize >= values.Length)
                IncreaseCapacity();

            values[heapSize] = val;
            heapSize = heapSize + 1;
            IncreaseKey(heapSize - 1, val);
        }

        public void IncreaseKey(int i, V val)
        {
            if (comparer(val, values[i]) < 0)
                throw new Exception(string.Format("New key can't be less than original. Index: {0}, value: {1}", i, val));

            int p = (i - 1) / 2;
            while (i > 0 && comparer(val, values[p]) > 0)
            {
                values[i] = values[p];
                i = p;
                p = (i - 1) / 2;
            }

            values[i] = val;
        }

        public V Peek()
        {
            if (heapSize == 0)
                throw new Exception("Heap is empty!");

            return values[0];
        }

        public V Remove()
        {
            if (heapSize == 0)
                throw new Exception("Heap is empty!");

            heapSize = heapSize - 1;
            V max = values[0];
            values[0] = values[heapSize];
            MaxHeapify(heapSize, 0);
            return max;
        }

        public void Clear()
        {
            heapSize = 0;
        }

        protected void IncreaseCapacity()
        {
            V[] newHeap = new V[values.Length * 2];
            values.CopyTo(newHeap, 0);
            values = newHeap;
        }

        protected static void Swap<T>(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }
    }
}
