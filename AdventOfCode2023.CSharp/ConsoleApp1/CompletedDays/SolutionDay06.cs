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
    internal class SolutionDay06
    {
        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();
        private List<string> Tokenize(string line, IEnumerable<char> splitChars) => line.Split(splitChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

        public long Solve1(string input)
        {
            var lns = SplitToLines(input);
            var times = Tokenize(lns[0], [' ']).Skip(1).Select(int.Parse).ToList();
            var dists = Tokenize(lns[1], [' ']).Skip(1).Select(int.Parse).ToList();

            var r = times.Index().Select(tm =>
            {
                var idx = tm.Key;
                var t = tm.Value;
                var bestAttempts = Enumerable.Range(1, t).Select(btnTime => (t - btnTime) * btnTime).Where(r => r > dists[idx]).Count();
                return bestAttempts;
            }).Aggregate((a, b) => a * b);

            return r;
        }

        public long Solve2(string input)
        {
            var lns = SplitToLines(input);
            var time = long.Parse(Tokenize(lns[0], [' ']).Skip(1).Aggregate((a, b) => a + b));
            var dist = long.Parse(Tokenize(lns[1], [' ']).Skip(1).Aggregate((a, b) => a + b));

            bool Check(long t)
            {
                return (time - t) * t > dist;
            }

            if (!Check(time / 2)) { throw new(); }
            if (Check(0)) { throw new(); }
            if (Check(time-1)) { throw new(); }

            var (b1a, b1b) = Bisect((0, time / 2), x => Check(x));
            var (b2a, b2b) = Bisect((time / 2, time - 1), x => !Check(x));

            var res = b2a - b1b + 1;
            return res;
        }

        private (long a, long b) Bisect((long a, long b) bisectTupleInp, Func<long, bool> check)
        {
            var bisectTuple = bisectTupleInp;
            while (bisectTuple.b - bisectTuple.a > 1)
            {
                var (a, b) = bisectTuple;
                var next = (a + b) / 2;

                bisectTuple = check(next) switch
                {
                    true => (a, next),
                    false => (next, b),
                };
            }
            return bisectTuple;
        }
    }
}
