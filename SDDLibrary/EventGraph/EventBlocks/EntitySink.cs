﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventGraph
{
    /// <summary>
    /// Blok koji zatvara kretanje entiteta u sistemu.
    /// </summary>
    public class EntitySink : EventBlock
    {
        protected LinkedList<Entity> receivedEntities;

        public EntitySink(string description) : base(description)
        {
            receivedEntities = new LinkedList<Entity>();
        }

        public override bool IsBusy
        {
            get { return false; }
        }

        public override EventEdge<EventBlock> ConnectToBlock(EventBlock block)
        {
            throw new Exception("Sink block can't be connected to other blocks.");
        }

        public override void ActivateEvent(object eventPar)
        {
            Entity asEntity;
            if (eventPar == null || (asEntity = eventPar as Entity) == null)
                throw new Exception("Sink must receive valid entity object.");

            receivedEntities.AddLast(asEntity);
        }

        public override void Clear()
        {
            receivedEntities.Clear();
        }
    }
}
