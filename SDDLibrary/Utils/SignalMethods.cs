using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Utils
{
    using SDDLibrary.Interpolation;
    using DoublePair = Tuple<double, double>;

    public enum Interpolation
    {
        ZeroOrder,
        FirstOrder
    }

    public static class SignalMethods
    {
        public static IEnumerable<DoublePair> Integrate(this IEnumerable<DoublePair> x, Interpolation order = Interpolation.ZeroOrder)
        {
            if (x == null)
                throw new ArgumentException("x and y data must not be null, and must have same length.");

            //Formiramo interpolante za oba signala
            var interpolant = FetchInterpolant(order)(x.Select(p => p.Item1), x.Select(p => p.Item2));
            return x.Select(p => new DoublePair(p.Item1, interpolant.Integrate(p.Item1)));
        }

        public static double[] Integrate(this IEnumerable<double> x, IEnumerable<double> y, Interpolation order = Interpolation.ZeroOrder)
        {
            if (x == null || y == null || x.Count() != y.Count())
                throw new ArgumentException("x and y data must not be null, and must have same length.");

            //Formiramo interpolante za oba signala
            var interpolant = FetchInterpolant(order)(x, y);
            return x.Select(p => interpolant.Integrate(p)).ToArray();
        }

        public static IEnumerable<DoublePair> SignalOperation(this IEnumerable<DoublePair> a, t_y_i_ operation)
        {
            return a.Select((p, i) => new DoublePair(p.Item1, operation(p.Item1, p.Item2, i)));
        }

        public static IEnumerable<DoublePair> SignalOperation(this IEnumerable<DoublePair> a, t_y_ operation)
        {
            return a.Select(p => new DoublePair(p.Item1, operation(p.Item1, p.Item2)));
        }

        public static IEnumerable<DoublePair> SignalElementWise(this IEnumerable<DoublePair> a, IEnumerable<DoublePair> b,
            x_y_ operation, Interpolation order = Interpolation.ZeroOrder)
        {
            if (a == null || b == null)
                throw new Exception("Data can not be null.");

            //Formiramo interpolante za oba signala
            var interpolate = FetchInterpolant(order);
            var interpA = interpolate(a.Select(p => p.Item1), a.Select(p => p.Item2));
            var interpB = interpolate(b.Select(p => p.Item1), b.Select(p => p.Item2));

            //Kombinujemo vremenske komponente oba signala i pozivamo operaciju nad zavisnim vrednostima
            foreach (var valX in a.Select(d => d.Item1).MergeOrderly(b.Select(d => d.Item1), true))
            {
                yield return new DoublePair(valX, operation(interpA.Interpolate(valX), interpB.Interpolate(valX)));
            }

            yield break;
        }

        static Func<IEnumerable<double>, IEnumerable<double>, IInterpolation> FetchInterpolant(Interpolation order)
        {
            Func<IEnumerable<double>, IEnumerable<double>, IInterpolation> interpolate;

            switch (order)
            {
                case Interpolation.ZeroOrder:
                    interpolate = StepInterpolation.Interpolate;
                    break;
                case Interpolation.FirstOrder:
                    interpolate = LinearSpline.Interpolate;
                    break;
                default:
                    throw new ArgumentException("Interpolation option not implemented.", order.ToString());
            }

            return interpolate;
        }
    }
}
