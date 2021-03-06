﻿using EventGraphLib.Interfaces;
using SDDLibrary.Heap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventGraph
{
    /// <summary>
    /// Implementacija prioritetnog reda cekanja.
    /// </summary>
    public class EntityPQueue : EventBlock, IEventObservable<DataPairArgs>
    {
        protected PQMin<Entity> queue;
        public event EventHandler<DataPairArgs> NotifyChanged;

        public EntityPQueue(string description, int capacity = int.MaxValue) : base(description)
        {
            Capacity = capacity;
            //Bitno je primetiti da je osnovno poredjenje zamenjeno virtuelnom metodom za poredjenje.
            queue = new PQMin<Entity>((x, y) => x.CompareTo(y));
        }

        public int Capacity { get; protected set; }

        public override bool IsBusy
        {
            get { return queue.Count >= Capacity; }
        }

        public int QueueLength
        {
            get { return queue.Count; }
        }

        public override void ActivateEvent(object eventPar)
        {
            if (IsBusy && eventPar != null)
                throw new Exception("PQueue can't receive entities if capacity is exceeded.");

            if (eventPar != null)
            {
                Entity entity = (Entity)eventPar;
                entity.QueueEnterTime = Simulator.Time;
                queue.Insert(entity);
                OnNotifyChanged(new DataPairArgs(Simulator.Time, QueueLength));
            }

            //Prosledjujemo entitet sledecem bloku
            var outputEdge = OutEdges().First();

            if (queue.Count > 0 && !outputEdge.Target.IsBusy)
            {
                Entity entity = queue.Remove();
                entity.WaitingTime += Simulator.Time - entity.QueueEnterTime;
                outputEdge.SetEvent(Simulator.Time, entity, ActivationType.Immediate);
                OnNotifyChanged(new DataPairArgs(Simulator.Time, QueueLength));
            }
        }

        public override EventEdge<EventBlock> ConnectToBlock(EventBlock block)
        {
            if (OutEdges().Count() > 0)
                throw new Exception("Entity queue can have only one output.");

            return base.ConnectToBlock(block);
        }

        public void Enqueue(IEnumerable<Entity> entitites)
        {
            foreach (var entity in entitites)
                queue.Insert(entity);
        }

        public override void Clear()
        {
            queue.Clear();
        }

        #region IObservable implementation
        public void Attach(EventHandler<DataPairArgs> handler)
        {
            NotifyChanged += handler;
        }

        public void Detach(EventHandler<DataPairArgs> handler)
        {
            NotifyChanged -= handler;
        }

        protected virtual void OnNotifyChanged(DataPairArgs e)
        {
            if (NotifyChanged != null)
                NotifyChanged.Invoke(this, e);
        }
        #endregion
    }
}
