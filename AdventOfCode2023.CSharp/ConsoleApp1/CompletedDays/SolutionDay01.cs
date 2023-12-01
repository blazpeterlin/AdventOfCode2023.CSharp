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
    internal class SolutionDay01
    {
        public int Solve1(string input)
        {
            var lns = Regex.Split(input, NewLine).Where(_ => _ != "").ToList();
            //string s = NewLine;
            var res = lns.Select(ln => int.Parse("" + ln.First(ch => Char.IsDigit(ch)) + ln.Last(Char.IsDigit))).Sum();

            return res;
        }

        public int Solve2(string input)
        {
            var lns = Regex.Split(input, NewLine).Where(_ => _ != "").ToList();
            //string s = NewLine;
            var res = lns.Select(ln => {
                List<string> candidates = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
                List<string> digits = Enumerable.Range(0, 10).Select(d => d.ToString()).ToList();
                int firstDigit = -1;
                int lastDigit = -1;

                string lnLengthened = ln + "                  ";
                for (int i = 0; i < ln.Length; i++)
                {
                    if (digits.Contains("" + ln[i])) { firstDigit = int.Parse("" + ln[i]); break; }
                
                    if (candidates.FirstOrDefault(c => lnLengthened.Substring(i).StartsWith(c)) is string c)
                    {
                            firstDigit = 1 + candidates.IndexOf(c);
                            break;
                    }
                }

                for (int i = ln.Length-1; i >= 0; i--)
                {
                    if (digits.Contains("" + ln[i])) { lastDigit = int.Parse("" + ln[i]); break; }

                    if (candidates.FirstOrDefault(c => lnLengthened.Substring(i).StartsWith(c)) is string c)
                    {
                        lastDigit = 1 + candidates.IndexOf(c);
                        break;
                    }
                }


                return 10 * firstDigit + lastDigit;
                }).Sum();

            return res;
        }


        public int Solve2ButPrettier(string input)
        {
            var lns = Regex.Split(input, NewLine).Where(_ => _ != "").ToList();
            //string s = NewLine;
            var res = lns.Select(ln => {
                List<string> words = ["-", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
                List<string> digits = Enumerable.Range(0, 10).Select(d => d.ToString()).ToList();

                List<KeyValuePair<int, string>> wordsIndexed = words.Index().Skip(1).ToList();
                List<KeyValuePair<int, string>> digitsIndexed = digits.Index().ToList();

                Dictionary<string, int> candidatesIndexed = wordsIndexed.Union(digitsIndexed).ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

                string lnLengthened = ln + "                  ";

                string firstDigitStr =
                    Enumerable.Range(0, ln.Length)
                    .Select(idx => lnLengthened.Substring(idx))
                    .Select(subStr => candidatesIndexed.Keys.FirstOrDefault(subStr.StartsWith) is string key ? key : null)
                    .First(str => str != null)!;
                string lastDigitStr =
                    Enumerable.Range(0, ln.Length)
                    .Reverse()
                    .Select(idx => lnLengthened.Substring(idx))
                    .Select(subStr => candidatesIndexed.Keys.FirstOrDefault(subStr.StartsWith) is string key ? key : null)
                    .First(str => str != null)!;

                return 10 * candidatesIndexed[firstDigitStr] + candidatesIndexed[lastDigitStr];
            }).Sum();

            return res;
        }
    }
}
