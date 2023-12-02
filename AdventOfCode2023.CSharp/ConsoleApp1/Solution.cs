using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Environment;

namespace Aoc2023.ActiveDay
{
    internal class Solution
    {
        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();

        public int Solve1(string input)
        {
            //var lns = SplitToLines(input);

            return 0;
        }

        public int Solve2(string input)
        {
            //var lns = SplitToLines(input);

            return 0;
        }
    }
}
