using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.FEL_template
{
    public delegate void NewEvent<TEvent>(TEvent newEvent) where TEvent : IEvent;

    public interface IEventHandler<TEvent> 
        where TEvent:IEvent
    {
        event NewEvent<TEvent> AddEvent;
        void Initialize();
        void ProcessEvent(TEvent currentEvent, double time);        
        void Finish(double endTime);
    }
}
