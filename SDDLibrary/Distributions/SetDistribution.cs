﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Distributions
{
    /// <summary>
    /// Klasa za podrsku unapred zadatom skupu slucajnih vrednosti.
    /// </summary>
    public class SetDistribution : Distribution<double>
    {
        private double[] setValues;
        private IEnumerator<double> enumerator;

        public SetDistribution(IEnumerable<double> values)
        {
            if (values == null || values.Count() == 0)
                throw new Exception("Input values must not be null or empty.");

            //Kopiramo skup vrednosti u lokalnu kolekciju.
            setValues = values.ToArray();
            enumerator = ((IEnumerable<double>)setValues).GetEnumerator();            
        }

        /// <summary>
        /// Sledeca zadata vrednost.
        /// </summary>
        /// <returns></returns>
        public override double NextValue()
        {
            if (enumerator.MoveNext())
                return enumerator.Current;

            throw new Exception("All values have been iterated.");
        }
    }
}
