﻿using EventGraphLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventGraph
{
    /// <summary>
    /// Apstraktna klasa koja sadrzi osnovne funkcionalnosti EventBlock skupa klasa.
    /// </summary>
    public abstract class EventBlock : AbstractEvent
    {
        public abstract bool IsBusy { get; }
        public static EventBlockSimulator Simulator { get; set; }

        public EventBlock(string description) : base(description) { }

        /// <summary>
        /// Povezivanje sa drugim event blokom. Poziv metode automatski popunjava
        /// graf event blokova.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public virtual EventEdge<EventBlock> ConnectToBlock(EventBlock block)
        {
            return Simulator.ConnectBlocks(this, block);
        }

        /// <summary>
        /// Enumeracija svih otocnih blokova posmatranog bloka.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EventBlock> OutBlocks()
        {
            return !Simulator.EventGraph.ContainsVertex(this) ? new EventBlock[0] : Simulator.EventGraph.OutEdges(this).Select(e => e.Target);
        }

        /// <summary>
        /// Enumeracija svih otocnih grana posmatranog bloka.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EventEdge<EventBlock>> OutEdges()
        {
            return !Simulator.EventGraph.ContainsVertex(this) ? new EventEdge<EventBlock>[0] : Simulator.EventGraph.OutEdges(this);
        }

        /// <summary>
        /// Svaka konkretna klasa iz EventBlock skupa mora obavezno
        /// da implementira ovu metodu, koja odredjuje aktivnost event-a.
        /// </summary>
        /// <param name="eventPar"></param>
        public override void ActivateEvent(object eventPar)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Metoda za oslobadjanje resursa EventBlock instance, neophodno
        /// pre poziva novog ciklusa simulacije.
        /// </summary>
        public virtual void Clear() { }
    }
}
