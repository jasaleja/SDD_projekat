﻿using EventGraphLib.Interfaces;
using SDDLibrary.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventGraph
{
    /// <summary>
    /// Osnovna implementacija servisa koji moze da opsluzuje 1 entiet u
    /// nekom trenutku (kapacitet jednak 1).
    /// </summary>
    public class EntityService : EventBlock, IEventObservable<DataPairArgs>
    {
        protected Distribution<double> distribution;
        protected bool isProcessing;
        protected object entity;
        protected EventEdge<EventBlock> selfEdge;
        protected EventEdge<EventBlock> callbackEdge;
        public event EventHandler<DataPairArgs> NotifyChanged;

        public EntityService(string description, Distribution<double> distribution) : base(description)
        {
            this.distribution = distribution;

            //Pravimo selfEdge
            selfEdge = base.ConnectToBlock(this);
        }

        public override bool IsBusy
        {
            get { return isProcessing; }
        }

        public override void ActivateEvent(object eventPar)
        {
            if (IsBusy)
            {
                if (eventPar != null)
                    throw new Exception("Service can't receive entities if it's busy.");
            }
            else if(eventPar == null)
            {
                throw new Exception("Service can't be called without entity if it's not active. Possible couse: service was set as start event.");
            }

            //Pocetak servisiranja, ako je entitet != null
            if (eventPar != null)
            {
                double serviceTime = distribution.NextValue();
                selfEdge.SetEvent(Simulator.Time + serviceTime);
                OnNotifyChanged(new DataPairArgs(Simulator.Time, serviceTime));
                entity = eventPar;
                isProcessing = true;
            }
            else
            {
                //Kraj servisiranja, ako je entitet null
                var outputEdge = OutEdges().ElementAt(1);

                if (outputEdge.Target.IsBusy)
                    throw new Exception("Output can't be blocked on service.");

                outputEdge.SetEvent(Simulator.Time, entity);

                //Izvrsavamo callback
                if (callbackEdge != null)
                    callbackEdge.SetEvent(Simulator.Time);

                isProcessing = false;
            }
        }

        /// <summary>
        /// Callback metoda kojom servis moze da javi odgovarajucem bloku da je spreman
        /// za opsluzivanje sledeceg entiteta. Pravilno postavljanje ovog signala je
        /// pod kontrolom korisnika.
        /// </summary>
        /// <param name="block"></param>
        public void SetCallback(EventBlock block)
        {
            if (callbackEdge != null)
                throw new Exception("Entity service can have only one callback.");

            callbackEdge = Simulator.ConnectBlocks(this, block);
        }

        public override EventEdge<EventBlock> ConnectToBlock(EventBlock block)
        {
            if (OutEdges().Count() > 1)
                throw new Exception("Entity service can have only one output.");

            return base.ConnectToBlock(block);
        }

        public override void Clear()
        {
            isProcessing = false;
            entity = null;
        }

        #region IObservable implementation
        public void Attach(EventHandler<DataPairArgs> handler)
        {
            NotifyChanged += handler;
        }

        public void Detach(EventHandler<DataPairArgs> handler)
        {
            NotifyChanged -= handler;
        }

        protected virtual void OnNotifyChanged(DataPairArgs e)
        {
            if(NotifyChanged != null)
                NotifyChanged.Invoke(this, e);
        }
        #endregion
    }
}
