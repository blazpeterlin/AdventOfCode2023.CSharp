using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Environment;
using Pos = (int x, int y);

namespace Aoc2023.ActiveDay
{
    internal class SolutionDay03
    {

        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();

        private Dictionary<Pos, char> MapChars(List<string> lns)
        {
            return lns.Index().SelectMany(kvp =>
            {
                int y = kvp.Key;
                return kvp.Value.ToCharArray().Index().Select(kvp2 => ((kvp2.Key, y), kvp2.Value));
            }).ToDictionary(kvp => kvp.Item1, kvp => kvp.Value);
        }

        public int Solve1(string input)
        {
            List<string> lns = SplitToLines(input);
            Dictionary<Pos, char> mappedChars = MapChars(lns);

            var symbolPos = mappedChars
                .Where(kvp => kvp.Value is char v && !char.IsDigit(v) && v != '.')
                .Select(kvp => kvp.Key)
                .SelectMany(ExpandAllDirections)
                .ToHashSet();


            // (?:^\d)+
            // @"[^\d]+"
            List<(Pos start, int len, string numStr)> numsWithLocs = GetLineTokensWithLocations(lns, @"(\d+)");

            List<int> schematicNums =
                numsWithLocs.Choose(nwl =>
                {
                    var ((start, y), len, numStr) = nwl;
                    int num = int.Parse(numStr);
                    var allPos = Enumerable.Range(start, len).Select(x => (x, y));
                    if (!allPos.Any(symbolPos.Contains)) { return (false, 0); }

                    return (true, num);
                }).ToList();

            int res = schematicNums.Sum();

            return res;
        }

        public int Solve2(string input)
        {
            List<string> lns = SplitToLines(input);
            Dictionary<Pos, char> mappedChars = MapChars(lns);

            Dictionary<Pos, List<Pos>> gearPosByAdjacentPos = mappedChars
                .Where(kvp => kvp.Value == '*')
                .Select(kvp => kvp.Key)
                .SelectMany(ExpandAllDirectionsKeepOrigPos)
                .GroupBy(x => x.expandedPos)
                .ToDictionary(grp => grp.Key, grp => grp.Select(val => val.origPos).ToList());

            List<(Pos start, int len, string numStr)> numsWithLocs = GetLineTokensWithLocations(lns, @"(\d+)");

            List<(Pos gearPos, int num)> numsWithGearPos =
                numsWithLocs.SelectMany(nwl =>
                {
                    var ((start, y), len, numStr) = nwl;
                    int num = int.Parse(numStr);
                    var allPos = Enumerable.Range(start, len).Select(x => (x, y));

                    List<Pos> allGearPos = 
                        allPos
                        .Where(gearPosByAdjacentPos.ContainsKey)
                        .SelectMany(ap => gearPosByAdjacentPos[ap])
                        .Distinct()
                        .ToList();
                    return allGearPos.Select(agp => (agp, num)).ToList();
                }).ToList();

            var validGears = numsWithGearPos.GroupBy(ngp => ngp.gearPos).Where(ngp => ngp.Count() == 2).ToList();

            int res = validGears.Select(vg => vg.Select(pair => pair.num).Aggregate((a, b) => a * b)).Sum();

            return res;
        }

        private List<(Pos p, int len, string str)> GetLineTokensWithLocations(List<string> lns, string tokenMatch)
        {
            return
                lns.Index()
                .SelectMany(tpl =>
                {
                    (int y, string ln) = tpl;

                    List<(Pos, int len, string str)> numWithLoc = 
                        Regex.Matches(ln, tokenMatch)
                        .SelectMany(mtch => 
                            mtch.Captures.Select(cptr => ((cptr.Index, y), cptr.Length, cptr.Value))
                        )
                        .ToList();

                    return numWithLoc;
                }).ToList();
        }

        private IEnumerable<Pos> ExpandAllDirections(Pos p)
        {
            List<int> dirs = [-1, 0, 1];
            return dirs.SelectMany(xdir => dirs.Select(ydir => (p.x + xdir, p.y + ydir)));
        }

        private IEnumerable<(Pos expandedPos, Pos origPos)> ExpandAllDirectionsKeepOrigPos(Pos p)
        {
            List<int> dirs = [-1, 0, 1];
            var res = dirs.SelectMany(xdir => dirs.Select(ydir => (p.x + xdir, p.y + ydir)));
            return res.Select(expandedPos => (expandedPos, p));
        }
    }
}
