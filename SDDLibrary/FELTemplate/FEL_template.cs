using SDDLibrary.Heap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.FEL_template
{
    public class FEL_template<TEvent>
        where TEvent : class, IEvent
    {
        PQMin<TEvent> FEL = new PQMin<TEvent>();
        //Pocetno vreme je uvek nula
        double time = 0.0;
        TEvent immediateEvent;

        public void Run(IEventHandler<TEvent> eventHandler, double tSim = double.PositiveInfinity)
        {
            time = 0.0;
            immediateEvent = null;
            eventHandler.AddEvent += AddEvent;
            eventHandler.Initialize();

            while (FEL.Count > 0 || immediateEvent != null)
            {
                TEvent nextEvent;
                if (immediateEvent != null)
                {
                    nextEvent = immediateEvent;
                    immediateEvent = null;
                }
                else
                {
                    nextEvent = FEL.Remove();
                    time = nextEvent.Time;
                }

                if (nextEvent.Time > tSim)
                {
                    eventHandler.Finish(nextEvent.Time);
                    return;
                }

                //Aktivacija posmatranog dogadjaja
                eventHandler.ProcessEvent(nextEvent, time);
            }

            eventHandler.Finish(time);
        }

        protected void AddEvent(TEvent newEvent)
        {
            //Vreme aktivacije sledeceg eventa ne moze biti ranije od trenutnog vremena simulacije
            if (newEvent.Time < time)
                throw new ArgumentException("Vreme aktivacije sledeceg eventa ne moze biti ranije od trenutnog vremena simulacije.", newEvent.ToString());

            if (newEvent.Time == time && immediateEvent == null)
                immediateEvent = newEvent;
            else
                FEL.Insert(newEvent);
        }
    }
}
