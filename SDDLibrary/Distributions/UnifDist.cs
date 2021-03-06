﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Distributions
{
    /// <summary>
    /// Implementacija Uniformne raspodele, U(a,b).
    /// </summary>
    public class UnifDist:Distribution<double>
    {
        protected double a = 0;
        protected double b = 0;

        public UnifDist(double leftLimit, double rightLimit, int seed = -1):base(seed)
        {
            this.a = leftLimit;
            this.b = rightLimit;
            this.distributionType = DistributionType.Uniform;
        }             

        /// <summary>
        /// Slucajna vrednost po Uniformnoj raspodeli.
        /// </summary>
        /// <returns></returns>
        public override double NextValue()
        {
            return a + (b - a) * randObj.NextDouble();        
        }
    }
}
