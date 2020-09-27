using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventImplementations
{
    /// <summary>
    /// Simulator za osnovni EG algoritam simulacije diskretnih dogadjaja.
    /// </summary>
    public class BaseSimulator : EventSimulator<AbstractEvent>
    {
        public BaseSimulator() : base() { }

        public void AddEventAndEdge(EventEdge<AbstractEvent> edge)
        {
            EventGraph.AddVerticesAndEdge(edge);
        }

        public void AddEventsAndEdges(IEnumerable<EventEdge<AbstractEvent>> edges)
        {
            foreach (EventEdge<AbstractEvent> edge in edges)
                EventGraph.AddVerticesAndEdge(edge);
        }
    }
}
