﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventGraph
{
    /// <summary>
    /// Klasa koja opisuje entitet. Druge implementacije entiteta treba da nasledjuju ovu klasu.
    /// </summary>
    public class Entity : IComparable<Entity>
    {
        protected static int instanceCount;
        public string Id { get; set; }
        public double QueueEnterTime { get; set; }
        public double WaitingTime { get; set; }

        public Entity()
        {
            Id = (instanceCount++).ToString();
        }

        /// <summary>
        /// Tekstualni opis entiteta.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Entity id: ", Id);
        }

        public virtual int CompareTo(Entity other)
        {
            return QueueEnterTime.CompareTo(other.QueueEnterTime);
        }
    }
}
