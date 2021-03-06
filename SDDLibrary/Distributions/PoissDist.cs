﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Distributions
{
    /// <summary>
    /// Implementacija Poasonove raspodele, P(lambda).
    /// </summary>
    public class PoissDist:Distribution<double>
    {
        private double lambda;

        public PoissDist(double lambda, int seed = -1):base(seed)
        {
            if (lambda <= 0)
                throw new Exception("Lambda parameter must be strictly greater than zero.");

            this.lambda = lambda;
            distributionType = DistributionType.Poisson;
        }

        /// <summary>
        /// Slucajna vrednost po Poasonovoj raspodeli.
        /// </summary>
        /// <returns></returns>
        public override double NextValue()
        {
            double u = randObj.NextDouble();

            if (u == 1)
                u = 1.0 - double.Epsilon;

            double p = Math.Exp(-lambda);
            double f = p;
            int i = 0;

            while (f < u)
            {
                p = p * lambda / (i + 1);
                f = f + p;
                i++;
            }

            return i;
        }
    }
}
