using SDDLibrary.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventGraph
{
    /// <summary>
    /// Osnovna implementacija generatira entiteta. Druge implementacije
    /// mogu da nasledjuju ovu implementaciju i dodaju nove funkcionalnosti.
    /// </summary>
    public class EntityGenerator : EventBlock
    {
        protected Distribution<double> distribution;
        protected EventEdge<EventBlock> selfEdge;


        public EntityGenerator(string description, Distribution<double> distribution) : base(description)
        {
            this.distribution = distribution;

            //Connecting this block to itself
            selfEdge = base.ConnectToBlock(this);
        }

        public override bool IsBusy
        {
            get { return false; }
        }

        public override void ActivateEvent(object eventPar)
        {
            //Gledamo izlaznu granu
            var outputEdge = OutEdges().ElementAt(1);

            if (outputEdge.Target.IsBusy)
                throw new Exception("Output can't be blocked on generator.");

            //Postavljamo trenutak proizvodnje sledeceg entiteta
            selfEdge.SetEvent(Simulator.Time + distribution.NextValue());

            //Prosledjujemo entitet sledecem bloku
            Entity newEntity = EntityCreator();
            outputEdge.SetEvent(Simulator.Time, newEntity);
        }

        /// <summary>
        /// Virtualna metoda koja omogucava da postavimo korisnicku implementaciju
        /// za stvaranje novih entiteta. Bazna implementacija samo poziva osnovni
        /// konstruktor klase Entity.
        /// </summary>
        /// <returns></returns>
        public virtual Entity EntityCreator()
        {
            return new Entity();
        }

        public override EventEdge<EventBlock> ConnectToBlock(EventBlock block)
        {
            if (OutEdges().Count() > 1)
                throw new Exception("Entity generator can have only one output.");

            return base.ConnectToBlock(block);
        }
    }
}
