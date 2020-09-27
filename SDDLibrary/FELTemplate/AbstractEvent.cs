using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.FEL_template
{
    public abstract class AbstractEvent : IEvent
    {
        public double Time { get; private set; }

        public AbstractEvent(double time)
        {
            Time = time;
        }

        public virtual int CompareTo(IEvent other)
        {
            return Time.CompareTo(other.Time);
        }

        /// <summary>
        /// Tekstualni opis event-a.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Time: {0,5:0.00}", Time);
        }
    }
}
