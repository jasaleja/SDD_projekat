﻿using EventGraphLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventGraph
{
    /// <summary>
    /// Prosirenje osnovnog simulatora koje dodaje instancu za belezenje aktivnosti
    /// u toku simulacije za kasniju analizu, IEventObserver objekta.
    /// </summary>
    public class EventBlockSimulator : EventSimulator<EventBlock>
    {
        public EventObserver Observer { get; private set; }

        public EventBlockSimulator() : base()
        {
            Observer = new EventObserver();
        }

        /// <summary>
        /// Podrska za povezivanje blokova EB simulatora.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public EventEdge<EventBlock> ConnectBlocks(EventBlock first, EventBlock second)
        {
            EventEdge<EventBlock> newEdge = new EventEdge<EventBlock>(first, second);
            EventGraph.AddVerticesAndEdge(newEdge);

            foreach(EventBlock ev in new EventBlock[] { first, second })
            {
                if (ev is IEventObservable<DataPairArgs> && !Observer.DataPairs.ContainsKey(ev))
                {
                    (ev as IEventObservable<DataPairArgs>).Attach(Observer.OnNotify);
                    Observer.DataPairs.Add(ev, new List<Tuple<double, double>>());
                }

                else if (ev is IEventObservable<DataValueArgs> && !Observer.DataValues.ContainsKey(ev))
                {
                    (ev as IEventObservable<DataValueArgs>).Attach(Observer.OnNotify);
                    Observer.DataValues.Add(ev, new List<double>());
                }
            }

            return newEdge;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();
            Observer.Clear();
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();
            foreach (EventBlock eb in EventGraph.Vertices)
                eb.Clear();
        }
    }
}
