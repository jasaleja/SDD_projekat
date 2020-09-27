using SDDLibrary.FEL_template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace klasicna_raskrsnica
{
    enum EventType
    {
        TrafficLightChange,
        Arrival,
        StartCrossing,
        EndCrossing,
        EnterIntersection,
        ExitIntersection
    }

    /// <summary>
    /// Event klasa implementira IComparable interfejs kako bismo
    /// omogucili uredjivanje dogadjaja u FEL listi.
    /// </summary>
    class Event : AbstractEvent
    {
        public EventType EventType;
        public int Index;

        public Event(EventType eventType, double time, int index = 0) : base(time)
        {
            EventType = eventType;
            Index = index;
        }

        /// <summary>
        /// Tekstualni opis event-a.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Type: {0}, Time: {1,5:0.00}", EventType, Time);
        }
    }
}
