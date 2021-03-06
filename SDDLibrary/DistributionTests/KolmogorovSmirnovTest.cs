﻿using SDDLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.DistributionTests
{
    /// <summary>
    /// Implementacija Kolmogorov-Smirnov testa za proveru raspodele eksperimentalnog uzorka.
    /// </summary>
    public static class KolmogorovSmirnovTest
    {        
        private static readonly double[] constant = new double[] { 1.22, 1.36, 1.51, 1.63 };
        public static readonly double[] Alpha = new double[] { 0.1, 0.05, 0.02, 0.01 };

        private static double[] range = new double[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24,  25, 26, 27,
                28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40
            };

        public static readonly double[][] Table = new double[][]
        {
            new double[] {0.95000,0.97500,0.99000,0.99500},
            new double[] {0.77639,0.84189,0.90000,0.92929},
            new double[] {0.63604,0.70760,0.78456,0.82900},
            new double[] {0.56522,0.62394,0.68887,0.73424},
            new double[] {0.50945,0.56328,0.62718,0.66853},
            new double[] {0.46799,0.51926,0.57741,0.61661},
            new double[] {0.43607,0.48342,0.53844,0.57581},
            new double[] {0.40962,0.45427,0.50654,0.54179},
            new double[] {0.38746,0.43001,0.47960,0.51332},
            new double[] {0.36866,0.40925,0.45662,0.48893},
            new double[] {0.35242,0.39122,0.43670,0.46770},
            new double[] {0.33815,0.37543,0.41918,0.44905},
            new double[] {0.32549,0.36143,0.40362,0.43247},
            new double[] {0.31417,0.34890,0.38970,0.41762},
            new double[] {0.30397,0.33760,0.37713,0.40420},
            new double[] {0.29472,0.32733,0.36571,0.39201},
            new double[] {0.28627,0.31796,0.35528,0.38086},
            new double[] {0.27851,0.30936,0.34569,0.37062},
            new double[] {0.27136,0.30143,0.33685,0.36117},
            new double[] {0.26473,0.29408,0.32866,0.35241},
            new double[] {0.25858,0.28724,0.32104,0.34427},
            new double[] {0.25283,0.28087,0.31394,0.33666},
            new double[] {0.24746,0.27490,0.30728,0.32954},
            new double[] {0.24242,0.26931,0.30104,0.32286},
            new double[] {0.23768,0.26404,0.29516,0.31657},
            new double[] {0.23320,0.25907,0.28962,0.31064},
            new double[] {0.22898,0.25438,0.28438,0.30502},
            new double[] {0.22497,0.24993,0.27942,0.29971},
            new double[] {0.22117,0.24571,0.27471,0.29466},
            new double[] {0.21756,0.24170,0.27023,0.28987},
            new double[] {0.21412,0.23788,0.26596,0.28530},
            new double[] {0.21085,0.23424,0.26189,0.28094},
            new double[] {0.20771,0.23076,0.25801,0.27677},
            new double[] {0.20472,0.22743,0.25429,0.27279},
            new double[] {0.20185,0.22425,0.26073,0.26897},
            new double[] {0.19910,0.22119,0.24732,0.26532},
            new double[] {0.19646,0.21826,0.24404,0.26180},
            new double[] {0.19392,0.21544,0.24089,0.25843},
            new double[] {0.19148,0.21273,0.23786,0.25518},
            new double[] {0.18913,0.21012,0.23494,0.25205},
        };        

        /// <summary>
        /// Provera kriticnih vednosti empirijske raspodele u odnosu na H0 hipotezu.
        /// </summary>
        /// <param name="numSamples">broj uzoraka</param>
        /// <param name="alphaZeroBasedIndex">indeks alpha parametra iz niza Alpha, 0-based</param>
        /// <returns></returns>
        public static double CriticalValue(uint numSamples, uint alphaZeroBasedIndex)
        {
            if (numSamples < 1 || alphaZeroBasedIndex >= Alpha.Length)
                throw new Exception("Input values are not valid.");

            if (numSamples > range.Last())
            {
                return constant[alphaZeroBasedIndex] / Math.Sqrt(numSamples);
            }
            else
            {
                int row;
                if (!BinarySearch.Infimum(range, numSamples, out row))
                    throw new Exception("Error occured while extracting critical value from Kolmogorov-Smirnov table.");

                return Table[row][alphaZeroBasedIndex];
            }
        }
    }
}
