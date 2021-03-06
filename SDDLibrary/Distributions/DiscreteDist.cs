﻿using SDDLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Distributions
{
    /// <summary>
    /// Implementacija genericke diskretne raspodele, za zadati konacan
    /// skup elemenata i njihovih verovatnoca.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DiscreteDistr<T> : Distribution<T>
    {
        private T[] values;
        //Niz probabilityFun sadrzi zbirnu sumu pojedinacnih verovatnoca kako
        //bi se primenom binarne pretrage efikasno pronasla sledeca vrednost.
        private double[] probabilityFun;
        private double probabilitySum;

        public DiscreteDistr(T[] inValues, double[] probabilities, int seed = -1) : base(seed)
        {
            //Provera ulaznih parametara
            if (inValues == null || probabilities == null || probabilities.Length == 0
                || inValues.Length != probabilities.Length)
            {
                throw new ArgumentException("Probability set and values are not compatibile.");
            }

            probabilityFun = new double[probabilities.Length];
            values = new T[probabilities.Length];
            distributionType = DistributionType.Discrete;

            //Kopiranje vrednosti u lokalne kolekcije i sortiranje
            probabilities.CopyTo(probabilityFun, 0);
            inValues.CopyTo(values, 0);
            Array.Sort(probabilityFun, values);

            if (probabilityFun[0] < 0.0)
                throw new Exception("Probability can not be negative.");

            for (int i = 0; i < probabilityFun.Length; i++)
            {
                probabilitySum += probabilityFun[i];
                probabilityFun[i] = probabilitySum;
            }

            if (probabilitySum == 0.0)
                throw new Exception("Total probability must be greater than zero.");
        }

        /// <summary>
        /// Slucajna vrednost po diskretnoj raspodeli.
        /// </summary>
        /// <returns></returns>
        public override T NextValue()
        {
            double u = randObj.NextDouble() * probabilitySum;
            int i = 0;

            if (!BinarySearch.Supremum(probabilityFun, u, out i))
            {
                throw new Exception("Error while generating next random discrete value.");
            }

            return values[i];
        }
    }
}
