using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib
{
    /// <summary>
    /// Abstraktna klasa koju nasledjuju sve implementacije event klasa. Bitno je uociti da jedna
    /// instanca ove klase jednoznacno odredjuje jedan dogadjaj tako da vise pojava istog dogadjaja
    /// u FEL moze da se razlikuje jedino na osnovu ulaznih parametara eventPar.
    /// </summary>
    public abstract class AbstractEvent
    {
        public string Description { get; protected set; }
        public event EventHandler EventFinished;

        protected void OnEventFinished()
        {
            if (EventFinished != null)
                EventFinished(this, EventArgs.Empty);
        }

        public AbstractEvent(string description)
        {
            Description = description;
        }        

        public abstract void ActivateEvent(object eventPar);

        public override string ToString()
        {
            return Description;
        }
    }
}
