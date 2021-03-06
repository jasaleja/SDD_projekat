﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Structures
{
    /// <summary>
    /// FIFO red zasnovan na kruznom baferu.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularBuffer<T>
    {
        private int count;
        private int tail;
        private int head;
        private T[] buffer;
        private int capacity;

        public CircularBuffer(int capacity = 100)
        {
            if (capacity < 1)
                throw new ArgumentException("Capacity must be greater or equal to 1.");

            count = 0;
            tail = 0;
            head = 0;
            buffer = new T[capacity];
            this.capacity = capacity;
        }

        public int Count
        {
            get { return count; }
        }

        public T Peek()
        {
            if (count <= 0)
                throw new Exception("Buffer is empty.");

            return buffer[tail];
        }

        public void Enqueue(T item)
        {
            if (count == capacity)
                IncreaseCapacity();

            //Elementi koji se dodaju na kraj, dodaju se u smeru uvecavanja indexa.
            tail = (tail + 1) % capacity;
            buffer[tail] = item;
            count++;
        }

        public T Dequeue()
        {
            if (count <= 0)
                throw new Exception("Buffer is empty.");

            var returnValue = buffer[head];
            head = (head + 1) % capacity;
            count--;
            return returnValue;
        }

        protected void IncreaseCapacity()
        {
            head = 0;
            tail = count;
            capacity = count * 2;
            T[] newBuffer = new T[capacity];
            buffer.CopyTo(newBuffer, 0);
            buffer = newBuffer;
        }
    }
}
