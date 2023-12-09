using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static System.Environment;

namespace Aoc2023.ActiveDay
{
    internal class SolutionDay09
    {
        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();
        private List<string> Tokenize(string line, IEnumerable<char> splitChars) => line.Split(splitChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();


        IEnumerable<long> GetDerivatives(IEnumerable<long> nums) => nums.Pairwise((a, b) => b - a);

        long ExtrapolateNextNum(IEnumerable<long> nums)
        {
            if (nums.All(n => n == 0)) { return 0; }
            return nums.Last() + ExtrapolateNextNum(GetDerivatives(nums));
        }

        public long Solve1(string input)
        {
            List<string> lns = SplitToLines(input);
            List<List<long>> lnNums = lns.Select(ln => Tokenize(ln, " ").Select(long.Parse).ToList()).ToList();

            List<long> resNums = lnNums.Select(ExtrapolateNextNum).ToList();
            long res = resNums.Sum();

            return res;
        }

        public long Solve2(string input)
        {
            List<string> lns = SplitToLines(input);
            List<List<long>> lnNums = lns.Select(ln => Tokenize(ln, " ").Select(long.Parse).ToList()).ToList();

            List<long> resNums = lnNums.Select((IEnumerable<long> lnNum) => lnNum.Reverse()).Select(ExtrapolateNextNum).ToList();
            long res = resNums.Sum();

            return res;
        }
    }
}
