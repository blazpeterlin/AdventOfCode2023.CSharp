using MoreLinq;
using MoreLinq.Extensions;
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
    internal class SolutionDay04
    {
        record struct Card(int CardNum, HashSet<int> WinningNumbers, List<int> YourNumbers)
        {
            public static Card Parse(string line)
            {
                List<string> tokenizedLine = line.Split(new[] { ':', '|' }).ToList();
                int currentCardNum = int.Parse(tokenizedLine[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(1).Trim());

                var numsLeft = new HashSet<int>(tokenizedLine[1].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
                var numsRight = tokenizedLine[2].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                return new Card(currentCardNum, numsLeft, numsRight);
            }

            public int CountMatches() => YourNumbers.Distinct().Where(WinningNumbers.Contains).Count();
        }

        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();

        public int Solve1(string input)
        {
            var lns = SplitToLines(input);
            var cards = lns.Select(Card.Parse).ToList();

            int res = cards.Select(card =>
            {
                if (card.CountMatches() == 0) { return 0; }
                return (int)Math.Pow(2, card.CountMatches() - 1);
            }).Sum();

            return res;
        }

        public int Solve2(string input)
        {
            var lns = SplitToLines(input);
            var cards = lns.Select(Card.Parse).ToList();

            Dictionary<int, int> wonCopies = Enumerable.Range(1, lns.Count).ToDictionary(i => i, i => 1);
            foreach(var card in cards)
            {
                int numCopies = wonCopies[card.CardNum];

                for (int i = card.CardNum + 1; i < card.CardNum + 1 + card.CountMatches(); i++)
                {
                    if (wonCopies.ContainsKey(i)) { wonCopies[i]+=numCopies; }
                }
            };

            int res = wonCopies.Values.Sum();

            return res;
        }
    }
}

