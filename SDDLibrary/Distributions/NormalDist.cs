﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Distributions
{
    /// <summary>
    /// Implementacija Gausove(Normalne) raspodele, N(m,ksi).
    /// </summary>
    public class NormalDist : Distribution<double>
    {
        private double m;
        private double psi;
        private int iset = 0;
        private double gset = 0;

        public NormalDist(double m, double psi, int seed = -1) : base(seed)
        {
            if (psi <= 0.0)
                throw new Exception("Psi parameter must be strictly greater than zero.");

            this.m = m;
            this.psi = psi;
            distributionType = DistributionType.Normal;
        }

        /// <summary>
        /// Slucajna vrednost po Normalnoj raspodeli. Osnovni algoritam
        /// preko transformacije sa 2 uniformne slucajne promenljive.
        /// </summary>
        /// <returns></returns>
        public double NextValueLegacy()
        {
            double u1 = randObj.NextDouble();
            double u2 = randObj.NextDouble();

            return psi * Math.Cos(2 * u1 * Math.PI) * Math.Sqrt(-2 * Math.Log(u2)) + m;
        }

        /// <summary>
        /// Slucajna vrednost po Normalnoj raspodeli. Optimizovan algoritam.
        /// </summary>
        /// <returns></returns>
        public override double NextValue()
        {
            double fac, rsq, v1, v2;
            if (iset != 0)
            {
                iset = 0;
                return gset;
            }

            do
            {
                v1 = 2.0 * randObj.NextDouble() - 1.0;
                v2 = 2.0 * randObj.NextDouble() - 1.0;
                rsq = v1 * v1 + v2 * v2;
            } while (rsq >= 1.0 || rsq == 0.0);

            fac = Math.Sqrt(-2.0 * Math.Log(rsq) / rsq);
            gset = psi * v1 * fac + m;
            iset = 1;
            return psi * v2 * fac + m;
        }
    }
}
