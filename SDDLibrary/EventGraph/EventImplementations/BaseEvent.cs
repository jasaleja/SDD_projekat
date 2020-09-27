using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventImplementations
{
    /// <summary>
    /// Implementacija event-a ako nije potreban ulazni parametar.
    /// </summary>
    public class BaseEvent : BaseEvent<object>
    {
        public BaseEvent(string name) : base(name) { }
    }

    /// <summary>
    /// Implementacija event-a sa generickim ulaznim parametrom.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseEvent<T> : AbstractEvent
    {
        public BaseEvent(string name) : base(name) { }
        public Action<T> EventAction { get; set; }

        public override void ActivateEvent(object eventPar)
        {            
            //Izvrsavanje event-a
            EventAction((T)eventPar);
            //Obavestenje o izvrsenju event-a
            OnEventFinished();
        }
    }
}
