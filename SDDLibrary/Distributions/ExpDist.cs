﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Distributions
{
    /// <summary>
    /// Implementacija Eksponencijalne raspodele, E(a).
    /// </summary>
    public class ExpDist:Distribution<double>
    {
        private double a = 0;

        public ExpDist(double a, int seed = -1):base(seed)
        {
            this.a = a;
            this.distributionType = DistributionType.Exponential;
        }

        /// <summary>
        /// Slucajna vrednost po Eksponencijalnoj raspodeli.
        /// </summary>
        /// <returns></returns>
        public override double NextValue()
        {
            double u = randObj.NextDouble();

            //Vrsimo proveru posto log(0) nije definisano.
            if (u == 0.0)
                u = double.Epsilon;

            return -1 / a * Math.Log(u);
        }
    }
}
