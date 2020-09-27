using EventGraphLib.EventGraph;
using SDDLibrary.Graph.Digraph;
using SDDLibrary.Heap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib
{
    /// <summary>
    /// Klasa koja objedinjuje komponente simulacije i implementira rad sa FEL listom.
    /// </summary>
    public abstract class EventSimulator<TEvent>
        where TEvent : AbstractEvent
    {
        public double Time { get; private set; }
        public PQMin<EventPar<TEvent>> FEL { get; private set; }
        public Digraph<TEvent, EventEdge<TEvent>> EventGraph { get; private set; }
        public double SimulationEndTime { get; set; }
        public EventPar<TEvent> CurrentEventPar { get; protected set; }

        public EventSimulator()
        {
            EventGraph = new Digraph<TEvent, EventEdge<TEvent>>();
            FEL = new PQMin<EventPar<TEvent>>();
            Time = 0.0;
        }

        public virtual void RunSimulation()
        {
            EventPar<TEvent> promotedEvent = null;
            EventPar<TEvent> eventPar = null;
            //Inicijalizacija pre pokretanja simulacije
            PreInitialize(); 

            while (FEL.Count > 0 || promotedEvent != null)
            {
                //Preuzimanje sledeceg event-a
                if (promotedEvent != null)
                {
                    eventPar = promotedEvent;
                    promotedEvent = null;
                }
                else
                {
                    eventPar = FEL.Remove();
                }
                CurrentEventPar = eventPar;

                //Trenutak aktivacije event-a
                Time = eventPar.Time;

                //Uslov za zavrsetak simulacije, trajanje simulacije
                if (Time >= SimulationEndTime)
                    break;

                //Aktivacija event-a
                var nextEvent = eventPar.Event;
                nextEvent.ActivateEvent(eventPar.Parameters);

                //Prolazak kroz sve otocne grane posmatranog event-a, ako je
                //uslov zadovoljen odgovarajuci skup parametara za aktivaciju
                //event-a se postavlja na FEL.
                foreach (var eventEdge in EventGraph.OutEdges(nextEvent))
                {
                    switch (eventEdge.ActivationType)
                    {
                        case ActivationType.Normal:
                            {
                                FEL.Insert(eventEdge.GetEventPar());
                            }
                            break;
                        case ActivationType.Immediate:
                            {
                                if (promotedEvent != null)
                                    throw new Exception("Only single immediate event can be triggered from current event.");

                                promotedEvent = eventEdge.GetEventPar();
                            }
                            break;
                    }                        
                }
            }

            //Inicijalizacija nakon zavrsetka simulacije
            PostInitialize();
        }

        /// <summary>
        /// Postavljanje pocetnog event-a simulacije.
        /// </summary>
        /// <param name="ev">Event koji se aktivira</param>
        /// <param name="time">Trenutak aktivacije</param>
        public virtual void StartEvent(TEvent ev, double time)
        {
            FEL.Insert(new EventPar<TEvent>(ev, time));
        }

        /// <summary>
        /// Postavljanje pocetnog event-a simulacije.
        /// </summary>
        /// <param name="eventPars">Skup parametara koji odredjuju aktivacije event-a</param>
        public virtual void StartEvents(params EventPar<TEvent>[] eventPars)
        {
            foreach(var evPar in eventPars)
                FEL.Insert(evPar);
        }

        /// <summary>
        /// Metoda za oslobadjanje resursa simulatora pre novog ciklusa simulacije. Overload
        /// metode treba da pozivaju ovu osnovnu implementaciju.
        /// </summary>
        protected virtual void PreInitialize()
        {
            Time = 0.0;
        }

        /// <summary>
        /// Metoda za oslobadjanje resursa simulatora posle izvrsenog ciklusa simulacije. Overload
        /// metode treba da pozivaju ovu osnovnu implementaciju.
        /// </summary>
        protected virtual void PostInitialize()
        {
            FEL.Clear();
        }
    }
}
