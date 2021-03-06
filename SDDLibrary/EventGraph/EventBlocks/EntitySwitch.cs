﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventGraph
{
    /// <summary>
    /// Implementacija bloka za routing/raporedjivanje entiteta na
    /// jedan od vise mogucih izlaza. Routing operacije se izvrsavaju odmah nakon 
    /// event-a koji ih je aktivirao.
    /// </summary>
    public class EntitySwitch : EventBlock
    {
        public EntitySwitch(string description) : base(description) { }

        public override bool IsBusy
        {
            get
            {
                foreach (var edge in OutEdges())
                {
                    if (!edge.Target.IsBusy)
                        return false;
                }
                return true;
            }
        }

        public override EventEdge<EventBlock> ConnectToBlock(EventBlock block)
        {
            if (block as EntitySwitch != null)
                throw new Exception("Switch can't be connected to other switch blocks.");

            return base.ConnectToBlock(block);
        }

        public override void ActivateEvent(object eventPar)
        {
            //Prenos entiteta na neblokirani izlaz
            var outputEdge = OutEdges().First(e => !e.Target.IsBusy);
            outputEdge.SetEvent(Simulator.Time, eventPar, ActivationType.Immediate);
        }
    }
}
