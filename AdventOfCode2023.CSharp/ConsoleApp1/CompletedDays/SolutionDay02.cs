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
    internal class SolutionDay02
    {
        record struct GameModel(int Id, Dictionary<string, int> MinCubesPerColor)
        {
            public static GameModel Parse(string ln)
            {
                var tkns = Regex.Split(ln, @"(?:\:\s|;\s|,\s)").ToList();

                Dictionary<string, int> req = new();
                int id = int.Parse(tkns[0].Split(new[] { ' ', ':' })[1]);

                foreach (var clrTkn in tkns.Skip(1).Select(c => c.Split(" ")))
                {
                    int num = int.Parse(clrTkn[0]);
                    string ty = clrTkn[1];
                    if (!req.ContainsKey(ty))
                    {
                        req[ty] = num;
                    }
                    else
                    {
                        req[ty] = Math.Max(req[ty], num);
                    }

                }

                return new(id, req);
            }
        }

        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();

        public int Solve1(string input)
        {
            var lns = SplitToLines(input);

            var parsedModels = lns.Select(GameModel.Parse);

            var res = parsedModels.Where(prs =>
            {
                var p = prs.MinCubesPerColor;
                return p["red"] <= 12
                    && p["green"] <= 13
                    && p["blue"] <= 14;
            }).Select(prs => prs.Id).Sum();

            return res;
        }

        public int Solve2(string input)
        {
            var lns = SplitToLines(input);

            var parsedModels = lns.Select(GameModel.Parse);

            var res = parsedModels.Select(pm => pm.MinCubesPerColor.Values.Aggregate((a, b) => a * b)).Sum();

            return res;
        }
    }
}
