using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.Interfaces
{
    /// <summary>
    /// Interfejs koji odredjuje nacin/protokol na koji event blok obavestava
    /// IEventObserver o promenama stanja.
    /// </summary>
    /// <typeparam name="T">Tip klase koja implementira event</typeparam>
    public interface IEventObservable<T>
        where T:EventArgs
    {
        event EventHandler<T> NotifyChanged;
        void Attach(EventHandler<T> handler);        
        void Detach(EventHandler<T> handler); 
    }
}
