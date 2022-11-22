﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace XIVSlothCombo.Extensions
{
    internal static class ListExtensions
    {
        private static readonly Random rng = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static bool EqualsAny<T>(this T obj, params T[] values)
        {
            return values.Any(x => x.Equals(obj));
        }
    }
}
