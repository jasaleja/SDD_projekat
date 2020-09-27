using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.Interfaces
{
    /// <summary>
    /// Protokol koji treba da podrzava Observer instanca za pracenje promena
    /// statnja event-a.
    /// </summary>
    /// <typeparam name="T">Informacija o promeni stanja</typeparam>
    public interface IEventObserver<T>
        where T : EventArgs
    {
        void OnNotify(object sender, T args);
    }
}
