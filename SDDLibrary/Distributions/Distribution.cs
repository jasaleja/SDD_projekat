using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Distributions
{
    public enum DistributionType
    {
        Discrete,
        Exponential,
        Uniform,
        Normal,
        Poisson
    }

    /// <summary>
    /// Abstraktna klasa koje je osnova za konkretne
    /// klase diskretnih i kontinualnih raspodela.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Distribution<T>
    {
        protected DistributionType distributionType;
        protected Random randObj;

        public Distribution(int seed = -1)
        {
            randObj = new Random(seed == -1 ? DateTime.Now.Millisecond : seed);
        }

        public DistributionType DType
        {
            get { return distributionType; }
        }

        /// <summary>
        /// Metoda koji implementiramo u izvedenim klasama.
        /// </summary>
        /// <returns></returns>
        public abstract T NextValue();
    }
}
