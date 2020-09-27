using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SDDLibrary.Utils
{
    //Citljiviji delagati
    public delegate double t_y_i_(double t, double y, int i);
    public delegate double t_y_(double t, double y);
    public delegate double x_y_(double x, double y);

    public static class HelperMethods
    {
        private static CultureInfo usInfo = new CultureInfo("en-US");

        /// <summary>
        /// Metoda vraca indeks prvog elementa u nizu toSearch koji je jednak toFind.
        /// Ako element ne postoji vraca se vrednost -1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="n"></param>
        /// <param name="toFind"></param>
        /// <returns></returns>
        public static int FindFirst<T>(this IEnumerable<T> n, T toFind) where T : IEquatable<T>
        {
            if (n == null)
                throw new Exception("Input values must not be null.");

            int count = 0;
            foreach(T el in n)
            {
                if (el.Equals(toFind))
                    return count;

                count++;
            }
            return -1;
        }

        /// <summary>
        /// Metoda vraca kumulativnu sumu niza kao novi niz.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double[] CumSum(this IEnumerable<double> n)
        {
            if (n == null)
                throw new Exception("Input values must not be null.");

            double[] result = new double[n.Count()];

            if (result.Length == 0)
                return result;

            IEnumerator<double> enumN = n.GetEnumerator();
            enumN.MoveNext();
            result[0] = enumN.Current;
            int count = 1;

            while (enumN.MoveNext())
            {                
                result[count] = result[count - 1] + enumN.Current;
                count++;
            }
            return result;
        }

        /// <summary>
        /// Metoda vraca razliku susednih elemenata niza kao novi niz.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double[] Diff(this IEnumerable<double> n)
        {
            if (n == null)
                throw new Exception("Input value must not be null.");

            double[] result = new double[Math.Max(n.Count() - 1, 0)];

            if (result.Length == 0)
                return result;

            IEnumerator<double> enumN = n.GetEnumerator();
            enumN.MoveNext();
            double prev = enumN.Current;
            int count = 0;

            while (enumN.MoveNext())
            {
                result[count] = enumN.Current - prev;
                prev = enumN.Current;
                count++;
            }
            return result;
        }

        /// <summary>
        /// Zbir elemenata dva niza kao novi niz.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] Add(this IEnumerable<double> a, IEnumerable<double> b)
        {
            return ElementWiseOperation(a, b, (x, y) => x + y);
        }

        /// <summary>
        /// Razlika elemenata dva niza kao novi niz.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] Subtract(this IEnumerable<double> a, IEnumerable<double> b)
        {
            return ElementWiseOperation(a, b, (x, y) => x - y);
        }

        /// <summary>
        /// Proizvod elemenata dva niza kao novi niz.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] Multiply(this IEnumerable<double> a, IEnumerable<double> b)
        {
            return ElementWiseOperation(a, b, (x, y) => x * y);
        }

        /// <summary>
        /// Kolicnik elemenata dva niza kao novi niz.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] Divide(this IEnumerable<double> a, IEnumerable<double> b)
        {
            return ElementWiseOperation(a, b, (x, y) => x / y);
        }

        /// <summary>
        /// Prvi niz stepenovan drugim kao novi niz.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] Raise(this IEnumerable<double> a, IEnumerable<double> b)
        {
            return ElementWiseOperation(a, b, (x, y) => Math.Pow(x, y));
        }

        /// <summary>
        /// Metoda za primenu genericke operacije nad parovima elemenata dva niza, kao novi niz.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static double[] ElementWiseOperation(this IEnumerable<double> a, IEnumerable<double> b, x_y_ operation)
        {
            if (a == null || b == null || a.Count() != b.Count())
                throw new Exception("Arrays must be of equal length and not null.");

            double[] result = new double[a.Count()];
            IEnumerator<double> enumA = a.GetEnumerator();
            IEnumerator<double> enumB = b.GetEnumerator();
            int count = 0;

            while (enumA.MoveNext() && enumB.MoveNext())
            {
                result[count] = operation(enumA.Current, enumB.Current);
                count++;
            }
            return result;
        }

        /// <summary>
        /// Metoda omogucava spajanje i uredjivanje 2 kolekcije uz pomoc odgovarajuceg poredjenja.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="distinct"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<T> MergeOrderly<T>(this IEnumerable<T> a, IEnumerable<T> b, 
            bool distinct = false, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            var result = a.Concat(b).OrderBy(k => k, comparer);

            if (distinct)
                return result.Distinct();
            else
                return result;
        }

        /// <summary>
        /// Metoda vraca niz ekvidistantnih tacaka u granicama [left,right] duzine length.
        /// Desna granica moze biti manja ili veca od leve.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static double[] Linspace(double left, double right, int length)
        {
            if (length < 1)
                throw new Exception("Number of intervals must be greater than 0.");

            if (length == 1 && left != right)
                throw new Exception("Array of length 1 is only allowed if left and right boundaries are equal.");

            double[] result = new double[length];
            double step = (right - left) / (length == 1 ? 1 : length - 1);

            for (int i = 0; i < length; i++)
            {
                result[i] = i * step + left;
            }

            return result;
        }

        /// <summary>
        /// Metoda vraca niz u rasponu od left do right sa korakom step.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static double[] Span(double left, double right, double step)
        {
            int count = (int)Math.Floor(((right - left) / step));

            if (count < 0)
                throw new Exception("Boundaries not compatible with step value.");

            double[] temp = new double[count + 1];

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = i * step + left;
            }

            return temp;
        }

        /// <summary>
        /// Metoda generise niz slucajnih brojeva tipa int, float ili double.
        /// </summary>
        /// <typeparam name="T">tip elementa u nizu</typeparam>
        /// <param name="numEl">broj elemenata u nizu</param>
        /// <param name="left">leva granicna vrednost</param>
        /// <param name="right">desna granicna vrednost</param>
        /// <param name="seed">ako je vrednost veca od 0 generisu se brojevi specificni za taj seed</param>
        /// <returns></returns>
        public static T[] RandomSpan<T>(int numEl, T left, T right, int seed = -1) where T : IComparable<T>, IConvertible
        {
            T[] temp = new T[numEl];
            double range = (right.ToDouble(usInfo) - left.ToDouble(usInfo));
            double leftAsDbl = left.ToDouble(usInfo);

            Random randObj;
            if (seed < 0)
                randObj = new Random(DateTime.Now.Millisecond);
            else
                randObj = new Random(seed);

            for (int i = 0; i < numEl; i++)
            {
                temp[i] = (T)((IConvertible)(randObj.NextDouble() * range + leftAsDbl)).
                    ToType(typeof(T), usInfo);
            }

            return temp;
        }

        /// <summary>
        /// Tekstualni zapis matrice.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="columnDistance"></param>
        /// <param name="rowDistance"></param>
        /// <param name="decimalCount"></param>
        /// <returns></returns>
        public static string ShowMatrix<T>(T[][] matrix, int columnDistance = 10, int rowDistance = 1, int decimalCount = 2)
        {
            string matrixString = null;
            string dist = null;

            while (rowDistance-- > 0)
                dist += "\n";

            foreach (T[] row in matrix)
            {
                foreach (T element in row)
                {
                    matrixString += string.Format("{0," + columnDistance + ":G" +
                        (decimalCount != 2 ? decimalCount.ToString() : "") + "}", element);
                }

                matrixString += dist;
            }

            return matrixString;
        }
    }
}
