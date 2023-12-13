using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static System.Environment;

namespace Aoc2023.ActiveDay
{
    internal class SolutionDay12
    {
        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();
        private List<string> Tokenize(string line, IEnumerable<char> splitChars) => line.Split(splitChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

        List<int> ParseNums(string s) => s.Split(',').Select(int.Parse).ToList();

        private IEnumerable<List<char>> AllInterpretations(List<char> chars, int position)
        {
            if (position == chars.Count) { yield return []; yield break; }

            foreach (var inner in AllInterpretations(chars, position + 1))
            {
                if (chars[position] == '?')
                {
                    yield return ['.', .. inner];
                    yield return ['#', .. inner];
                }
                else
                {
                    yield return [chars[position], .. inner];
                }
            }
        }

        public long Solve1(string input)
        {
            var lns = SplitToLines(input).Select(ln => ln.Split(" ") switch { [string str, string numS] => (str.ToCharArray().ToList(), ParseNums(numS)), _ => throw new() }).ToList();

            var validArrangements =
                lns
                .Select(tpl =>
                {
                    List<char> chars = tpl.Item1;
                    List<int> nums = tpl.Item2;

                    return AllInterpretations(chars, 0)
                        .Where(interpretation =>
                        {
                            List<string> tokens = Tokenize(new string(interpretation.ToArray()), ['.']);
                            bool isValid = 
                                tokens.Count == nums.Count 
                                && tokens.Zip(nums).All(zipped => zipped is (string tkn, int expectedLen) && tkn.Length == expectedLen);
                            if (isValid) { }
                            return isValid;
                        }).Count();
                }).ToList();

            long res = validArrangements.Sum();
            return res;
        }


        readonly record struct State(int position, int lastPassedPosition, int numCheckedTokens, string uncheckedTokenSoFar);

        class InterpretationGenerator(List<char> Chars, List<int> ReqNums)
        {
            bool IsValidSoFar (string currToken, int whichNum)
            {
                bool finishedToken = currToken.EndsWith(".");
                if (finishedToken)
                {
                    return ReqNums.Count > whichNum && ReqNums[whichNum] == currToken.Trim('.').Length;
                }
                else
                {
                    return ReqNums.Count > whichNum && ReqNums[whichNum] >= currToken.Length;
                }
            }


            Dictionary<State, long> DynamicProgrammingCache = new();

            public long CountValidInterpretations(State state)
            {
                if (DynamicProgrammingCache.TryGetValue(state, out long cachedValue)) { return cachedValue; }

                var (position, lastPassedPosition, numCheckedTokens, uncheckedTokenSoFar) = state;
                uncheckedTokenSoFar = uncheckedTokenSoFar.TrimStart('.');

                if (position == Chars.Count && uncheckedTokenSoFar.Length > 0) { uncheckedTokenSoFar += "."; }

                if (uncheckedTokenSoFar.Length > 0)
                {
                    if (IsValidSoFar(uncheckedTokenSoFar, numCheckedTokens))
                    {
                        if (uncheckedTokenSoFar.EndsWith('.'))
                        {
                            numCheckedTokens++;
                            uncheckedTokenSoFar = "";
                            lastPassedPosition = position;
                        }
                    }
                    else 
                    { 
                        return 0;
                    }
                }

                if (position == Chars.Count) { return numCheckedTokens == ReqNums.Count ? 1 : 0; }


                if (Chars[position] == '?')
                {
                    long validForDot = CountValidInterpretations(new State(position + 1, lastPassedPosition, numCheckedTokens, uncheckedTokenSoFar + "."));
                    long validForHash = CountValidInterpretations(new State(position + 1, lastPassedPosition, numCheckedTokens, uncheckedTokenSoFar + "#"));
                    long res = validForDot + validForHash;
                    DynamicProgrammingCache[state] = res;
                    return res;
                }
                else
                {
                    long res = CountValidInterpretations(new State(position + 1, lastPassedPosition, numCheckedTokens, uncheckedTokenSoFar + Chars[position]));
                    DynamicProgrammingCache[state] = res;
                    return res;
                }
            }
        }

        public long Solve2(string input)
        {
            var lns = SplitToLines(input).Select(ln => ln.Split(" ") switch { [string str, string numS] => (str.ToCharArray().ToList(), ParseNums(numS)), _ => throw new() }).ToList();

            var validArrangements =
                lns
                .Select(tpl =>
                {
                    List<char> chars = FoldListX5(tpl.Item1, ['?']);
                    List<int> nums = FoldListX5(tpl.Item2, []);

                    var intGen = new InterpretationGenerator(chars, nums);

                    return intGen.CountValidInterpretations(new State(0, 0, 0, ""));
                }).ToList();

            long res = validArrangements.Sum();

            return res;
        }

        private List<T> FoldListX5<T>(List<T> initialChars, List<T> separator)
        {
            return Enumerable.Range(0, 5).Select(i => initialChars).Aggregate((lst1, lst2) => [.. lst1, ..separator, .. lst2]);
        }
    }
}
