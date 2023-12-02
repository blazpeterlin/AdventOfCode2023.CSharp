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


        public int Solve1(string input)
        {
            var lns = Regex.Split(input, NewLine)
                .Where(ln => ln != "")
                .Select(ln => Regex.Split(ln, @"(?:\:\s|;\s|,\s)").ToList())
                .ToList();


            var parsed = lns.Select(tkns =>
            {
                Dictionary<string, int> req = new();
                int id = int.Parse(tkns[0].Split(new[] { ' ', ':' })[1]);

                var coloured = tkns.Skip(1).ToList();
                foreach(var clrTkn in coloured.Select(c => c.Split(" ")))
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

                return (id, req);
            });

            var res = parsed.Where(prs =>
            {
                var p = prs.req;
                return p["red"] <= 12
                    && p["green"] <= 13
                    && p["blue"] <= 14;
            }).Select(prs => prs.id).Sum();

            // not 369
            // not 1681
            return res;
        }

        public int Solve2(string input)
        {
            var lns = Regex.Split(input, NewLine)
                .Where(ln => ln != "")
                .Select(ln => Regex.Split(ln, @"(?:\:\s|;\s|,\s)").ToList())
                .ToList();


            var parsed = lns.Select(tkns =>
            {
                Dictionary<string, int> req = new();
                int id = int.Parse(tkns[0].Split(new[] { ' ', ':' })[1]);

                var coloured = tkns.Skip(1).ToList();
                foreach (var clrTkn in coloured.Select(c => c.Split(" ")))
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

                return (id, req);
            });

            var res = parsed.Select(prs =>
            {
                var p = prs.req;
                return p["red"] * p["green"] * p["blue"];
            }).Sum();

            return res;
        }
    }
}
