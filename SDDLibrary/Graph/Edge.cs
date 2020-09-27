using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph
{
    public class Edge<TVertex> : IEdge<TVertex>
    {
        protected TVertex source;
        protected TVertex target;
        protected string value;

        public TVertex Source
        {
            get
            {
                return source;
            }
        }

        public TVertex Target
        {
            get
            {
                return target;
            }
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public Edge(TVertex source, TVertex target, string value = null)
        {
            this.source = source;
            this.target = target;
            this.value = value;
        }

        /// <summary>
        /// Tekstualna predstava grane grafa.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("(V1: {0}, V2: {1}).", source, target);
        }
    }
}
