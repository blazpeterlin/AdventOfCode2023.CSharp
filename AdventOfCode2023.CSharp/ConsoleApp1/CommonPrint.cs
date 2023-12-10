using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc2023.ActiveDay
{
    internal class CommonPrint
    {
        public static void PrintMap(HashSet<(int x, int y)> m)
        {
            var maxX = m.Max(t => t.x);
            var maxY = m.Max(t => t.y);

            for (int y = 0; y <= maxY; y++)
            {
                Console.WriteLine();
                for (int x = 0; x <= maxX; x++)
                {
                    if (m.Contains((x, y)))
                    {
                        Console.Write("█");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
            }
        }

    }
}
