using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc2023.ActiveDay
{
    internal static class CommonMath
    {
        public static long Gcf(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static long Lcm(long a, long b)
        {
            return (a / Gcf(a, b)) * b;
        }
    }
}
