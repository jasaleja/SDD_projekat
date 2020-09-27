using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Utils
{
    public static class BinarySearch
    {
        /// <summary>
        /// Metoda za binarnu pretragu niza. Niz mora biti
        /// sortiran po rastucem redosledu.
        /// </summary>
        /// <param name="a">Niz koji pretrazujemo</param>
        /// <param name="value">Vrednost koju trazimo</param>
        /// <param name="pos">Indeks prvog pronadjenog elementa</param>
        /// <returns>Da li je element pronadjen</returns>
        public static bool Search<T>(this IList<T> a, T value, out int pos, Comparison<T> customComparer = null)
        {
            pos = 0;

            if (a == null)
                throw new ArgumentException("Valule can not be null", "a");

            //Preuzimanje comparer metode
            Comparison<T> comparer = FetchComparer(customComparer);
            int left = 0;
            int right = a.Count - 1;
            int compareValue = 0;

            while (right >= left)
            {
                //trazimo srednji element
                pos = (right + left) / 2;
                compareValue = comparer(value, a[pos]);

                //uporedjujemo
                if (compareValue < 0)
                {
                    //uzimamo LEVI podniz
                    right = pos - 1;
                }
                else if (compareValue > 0)
                {
                    //uzimamo DESNI podniz
                    left = pos + 1;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Metoda za trazenje indeksa i takvog da {min(i), a(i) > value} pod
        /// pretpostavkom da je niz uredjen po rastucem poretku.
        /// </summary>
        /// <param name="a">Niz koji pretrazujemo</param>
        /// <param name="value">Element ciju granicu trazimo</param>
        /// <param name="pos">Indeks granice zadatog elementa</param>
        /// <returns>Da li sup postoji</returns>
        public static bool StrictUpperBound<T>(this IList<T> a, T value, out int pos, Comparison<T> customComparer = null)
        {
            pos = 0;

            if (a == null)
                throw new ArgumentException("Value can not be null", "a");

            if (a.Count == 0)
                return false;

            //Preuzimanje comparer metode
            Comparison<T> comparer = FetchComparer(customComparer);
            int left = 0;
            int right = a.Count - 1;            

            while (left < right)
            {
                //Pristupamo srednjem
                pos = (right + left) / 2;

                if (comparer(value, a[pos]) < 0)
                {
                    //Uzimamo LEVI podniz, ali ukljucujemo srednji element
                    //koji moze da se ispostavi kao gornje ogranicenje
                    right = pos;
                }
                else
                {
                    //Uzimamo DESNI podniz
                    left = pos + 1;
                }
            }

            //Element kome poslednjem pristupamo moze da se ispostavi kao gornje ogranicenje
            if (comparer(value, a[left]) < 0)
            {
                pos = left;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Metoda za trazenje indeksa i takvog da {max(i), a(i) je manje od value} pod
        /// pretpostavkom da je niz uredjen po rastucem poretku.
        /// </summary>
        /// <param name="a">Niz koji pretrazujemo</param>
        /// <param name="value">Element ciju granicu trazimo</param>
        /// <param name="pos">Indeks granice zadatog elementa</param>
        /// <returns>Da li granica postoji</returns>
        public static bool StrictLowerBound<T>(this IList<T> a, T value, out int pos, Comparison<T> customComparer = null)
        {
            pos = 0;

            if (a == null)
                throw new ArgumentException("Value can not be null", "a");

            if (a.Count == 0)
                return false;

            //Preuzimanje comparer metode
            Comparison<T> comparer = FetchComparer(customComparer);
            int left = 0;
            int right = a.Count - 1;

            while (left < right)
            {
                //Pristupamo srednjem
                pos = (left + right + 1) / 2;

                if (comparer(value, a[pos]) > 0)
                {
                    //Uzimamo DESNI podniz, ali ukljucujemo srednji element
                    //koji moze da se ispostavi kao donje ogranicenje
                    left = pos;
                }
                else
                {
                    //Uzimamo LEVI podniz
                    right = pos - 1;
                }
            }

            //Element kome poslednjem pristupamo moze da se ispostavi kao donje ogranicenje
            if (comparer(value, a[left]) > 0)
            {
                pos = left;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Metoda za trazenje indeksa i takvog da {min(i), a(i) >= value} pod
        /// pretpostavkom da je niz uredjen po rastucem poretku (i je indeks supremuma od value u a)
        /// </summary>
        /// <param name="a">Niz koji pretrazujemo</param>
        /// <param name="value">Element ciji sup trazimo</param>
        /// <param name="pos">Indeks sup zadatog elementa</param>
        /// <returns>Da li sup postoji</returns>
        public static bool Supremum<T>(this IList<T> a, T value, out int pos, Comparison<T> customComparer = null)
        {
            //Trazimo strogo donje ogranicenje
            if (StrictLowerBound(a, value, out pos, customComparer))
            {
                //Sledeci element u nizu moze biti supremum, ako postoji
                if (pos + 1 < a.Count)
                {
                    pos++;
                    return true;
                }
            }
            else
            {
                //Ako donje ogranicenje ne postoji tada su svi elementi veci ili jednaki value
                //pa je prvi element supremum od value.
                pos = 0;
                return true;                
            }

            return false;
        }

        /// <summary>
        /// Metoda za trazenje indeksa i takvog da {max(i), a(i) je manje ili jednako value} pod
        /// pretpostavkom da je niz uredjen po rastucem poretku (i je indeks infimuma value u a)
        /// </summary>
        /// <param name="a">Niz koji pretrazujemo</param>
        /// <param name="value">Element ciji inf trazimo</param>
        /// <param name="pos">Indeks inf zadatog elementa</param>
        /// <returns>Da li inf postoji</returns>
        public static bool Infimum<T>(this IList<T> a, T value, out int pos, Comparison<T> customComparer = null)
        {
            //Trazimo strogo donje ogranicenje
            if (StrictUpperBound(a, value, out pos, customComparer))
            {
                //Prethodni element u nizu moze biti infimum, ako postoji
                if (pos > 0)
                {
                    pos--;
                    return true;
                }
            }
            else
            {
                //Ako gornje ogranicenje ne postoji tada su svi elementi manji ili jednaki value
                //pa je poslednji element supremum od value.
                pos = a.Count - 1;
                return true;
            }

            return false;
        }

        private static Comparison<T> FetchComparer<T>(Comparison<T> comparer)
        {
            if (comparer == null)
                return Comparer<T>.Default.Compare;
            else
                return comparer;
        }
    }
}
