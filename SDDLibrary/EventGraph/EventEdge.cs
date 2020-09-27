using EventGraphLib.EventImplementations;
using SDDLibrary.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib
{
    public class BaseEventEdge : EventEdge<AbstractEvent>
    {
        public BaseEventEdge(AbstractEvent sourceEvent, AbstractEvent targetEvent) 
            : base(sourceEvent, targetEvent) {  }
    }

    public enum ActivationType
    {
        None,
        Normal,
        Immediate
    }

    /// <summary>
    /// Genericka varijanta EventEdge implementacije, u slucaju da postoje dodatni
    /// parametri koje je potrebno proslediti event-u u trenutku aktivacije.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventEdge<TEvent>: Edge<TEvent>
    {
        public object Parameters { get; protected set; }
        public double ActivationTime { get; protected set; }
        public ActivationType ActivationType { get; protected set; }

        public EventEdge(TEvent sourceEvent, TEvent targetEvent)
            : base(sourceEvent, targetEvent) { }

        public void SetEvent(double activationTime, object parameters, ActivationType activationType = ActivationType.Normal)
        {
            Parameters = parameters;
            ActivationTime = activationTime;
            ActivationType = activationType;
        }

        public void SetEvent(double activationTime, ActivationType activationType = ActivationType.Normal)
        {
            Parameters = null;
            ActivationTime = activationTime;            
            ActivationType = activationType;
        }

        public EventPar<TEvent> GetEventPar()
        {
            ActivationType = ActivationType.None;
            var newPar = new EventPar<TEvent>(this, Parameters);
            Parameters = null;
            return newPar;
        }
    }
}
