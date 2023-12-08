using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static System.Environment;

namespace Aoc2023.ActiveDay
{
    internal class SolutionDay08
    {
        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();
        private List<string> Tokenize(string line, IEnumerable<char> splitChars) => line.Split(splitChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

        private (char[] ins, Dictionary<string, (string b, string c)> map) Parse (string input)
        {
            List<string> lns = SplitToLines(input);

            char[] ins = lns[0].ToCharArray();

            Dictionary<string, (string b, string c)> map =
                lns
                .Skip(1)
                .Select(ln => Tokenize(ln, [' ', '=', ',', '(', ')']) switch { [var a, var b, var c] => (a, b, c), _ => throw new() })
                .ToDictionary(x => x.a, x => (x.b, x.c));

            return (ins, map);
        }

        private long CountStepsUntilEndState(string initialState, Predicate<string> isEndState, char[] ins, Dictionary<string, (string b, string c)> map)
        {
            string state = initialState;
            for (int i = 0; ; i++)
            {
                var (l, r) = map[state];
                state = ins[i % ins.Length] switch { 'L' => l, 'R' => r, _ => throw new(), };
                if (isEndState(state)) { return i + 1; }
            }
        }

        public long Solve1(string input)
        {
            var (ins, map) = Parse(input);

            return CountStepsUntilEndState("AAA", (s) => s == "ZZZ", ins, map);
        }

        public long Solve2(string input)
        {
            var (ins, map) = Parse(input);

            List<string> startStates = map.Keys.Where(k => k.EndsWith("A")).ToList();
            List<long> times2wait = startStates.Select(state => CountStepsUntilEndState(state, s => s.EndsWith("Z"), ins, map)).ToList();
            long res = times2wait.Aggregate(CommonMath.Lcm);

            return res;
        }
    }
}
