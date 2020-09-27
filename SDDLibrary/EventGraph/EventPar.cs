using EventGraphLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib
{
    public class BaseEventPar : EventPar<AbstractEvent>
    {
        public BaseEventPar(EventEdge<AbstractEvent> eventEdge):base(eventEdge)
        {
            Event = eventEdge.Target;
            Time = eventEdge.ActivationTime;
        }

        public BaseEventPar(EventEdge<AbstractEvent> eventEdge, object parameters) : this(eventEdge)
        {
            Parameters = parameters;
        }
    }

    /// <summary>
    /// Genericka varijanta event parametara, podrska za event-e koji zahtevaju
    /// dodatne ulazne parametre prilikom aktivacije.
    /// </summary>
    /// <typeparam name="TPar"></typeparam>
    public class EventPar<TEvent>: IComparable<EventPar<TEvent>>        
    {
        public object Parameters { get; set; }

        public EventPar(TEvent ev, double time)
        {
            Event = ev;
            Time = time;
        }

        public EventPar(EventEdge<TEvent> eventEdge, object parameters = null)
        {
            Event = eventEdge.Target;
            Time = eventEdge.ActivationTime;
            Parameters = parameters;
            ActivationEvent = eventEdge.Source;
        }

        /// <summary>
        /// Referenca na event koji je planiran za aktivaciju.
        /// </summary>
        public TEvent Event { get; protected set; }

        /// <summary>
        /// Referenca na event koji je inicirao trenutni event.
        /// </summary>
        public TEvent ActivationEvent { get; protected set; }

        /// <summary>
        /// Trenutak aktivacije event-a.
        /// </summary>
        public double Time { get; protected set; }

        public int CompareTo(EventPar<TEvent> other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}
