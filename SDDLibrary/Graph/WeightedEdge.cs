using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph
{
    public class WeightedEdge<TVertex>:Edge<TVertex>
    {
        private double weight;
        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public WeightedEdge(TVertex source, TVertex target, double weight):base(source, target)
        {
            this.weight = weight;
        }
    }
}
