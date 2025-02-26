using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eunjung
{
    public static class Extensions
    {
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)//<t>는 type 지정되지않은거
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                //0보다 크거나 같고 int32.MaxValue 보다 작은 32비트 부호 있는
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
